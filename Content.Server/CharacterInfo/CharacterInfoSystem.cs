using System.Collections.Generic;
using Content.Server.Mind.Components;
using Content.Server.Roles;
using Content.Shared.CharacterInfo;
using Content.Shared.Objectives;
using Content.Shared.Roles;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Server.CharacterInfo;

public class CharacterInfoSystem : EntitySystem
{
    [Dependency] private readonly IPrototypeManager _prototype = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeNetworkEvent<RequestCharacterInfoEvent>(OnRequestCharacterInfoEvent);
    }

    private void OnRequestCharacterInfoEvent(RequestCharacterInfoEvent msg, EntitySessionEventArgs args)
    {
        if (!args.SenderSession.AttachedEntity.HasValue
            || args.SenderSession.AttachedEntity != msg.EntityUid)
            return;

        var entity = args.SenderSession.AttachedEntity.Value;
        if (!TryComp(entity, out MindComponent? mindComponent) || mindComponent.Mind == null)
            return;

        var mind = mindComponent.Mind;

        // Objectives
        var conditions = new Dictionary<string, List<ConditionInfo>>();
        var jobTitle = "No Profession";
        var jobDesc = "You don't do anything.";
        var briefing = "!!ERROR: No Briefing!!"; //should never show on the UI unless there's a bug


        // Get objectives
        foreach (var objective in mind.AllObjectives)
        {
            if (!conditions.ContainsKey(objective.Prototype.Issuer))
                conditions[objective.Prototype.Issuer] = new List<ConditionInfo>();
            foreach (var condition in objective.Conditions)
            {
                conditions[objective.Prototype.Issuer].Add(new ConditionInfo(condition.Title,
                    condition.Description, condition.Icon, condition.Progress));
            }
        }

        var allegiances = new Dictionary<string, string>();

        // Get job title, desc & allegiances
        foreach (var role in mind.AllRoles)
        {
            if (role is not Job job)
                continue;

            jobTitle = job.Prototype.Name;
            jobDesc = job.Prototype.Description;

            foreach (var allegiance in job.Prototype.Allegiances)
            {
                if (!_prototype.TryIndex<AllegiancePrototype>(allegiance, out var proto))
                    continue;

                allegiances.Add(proto.Name, proto.Description);
            }

            break;
        }

        // Get briefing
        briefing = mind.Briefing;

        RaiseNetworkEvent(new CharacterInfoEvent(entity, (jobTitle, jobDesc), conditions, briefing, allegiances, mind.Traits),
            Filter.SinglePlayer(args.SenderSession));
    }
}
