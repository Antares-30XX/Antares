using System;
using System.Collections.Generic;
using Content.Shared.Objectives;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.CharacterInfo;

[Serializable, NetSerializable]
public class RequestCharacterInfoEvent : EntityEventArgs
{
    public readonly EntityUid EntityUid;

    public RequestCharacterInfoEvent(EntityUid entityUid)
    {
        EntityUid = entityUid;
    }
}

[Serializable, NetSerializable]
public class CharacterInfoEvent : EntityEventArgs
{
    public readonly EntityUid EntityUid;
    public readonly (string Name, string Desc) JobInfo;
    public readonly Dictionary<string, List<ConditionInfo>> Objectives;
    public readonly Dictionary<string, string> Allegiances;
    public readonly string Briefing;
    public List<string> Traits;

    public CharacterInfoEvent(EntityUid entityUid, (string, string) jobInfo, Dictionary<string, List<ConditionInfo>> objectives, string briefing, Dictionary<string, string> allegiances, List<string> traits)
    {
        EntityUid = entityUid;
        JobInfo = jobInfo;
        Objectives = objectives;
        Briefing = briefing;
        Allegiances = allegiances;
        Traits = traits;
    }
}
