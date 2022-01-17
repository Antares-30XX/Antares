using System.Collections.Generic;
using System.Linq;
using Content.Shared.CharacterInfo;
using Content.Shared.Objectives;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.Utility;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Utility;

namespace Content.Client.CharacterInfo.Components;

public class CharacterInfoSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeNetworkEvent<CharacterInfoEvent>(OnCharacterInfoEvent);
        SubscribeLocalEvent<CharacterInfoComponent, ComponentAdd>(OnComponentAdd);
    }

    private void OnComponentAdd(EntityUid uid, CharacterInfoComponent component, ComponentAdd args)
    {
        component.Scene = component.Control = new CharacterInfoComponent.CharacterInfoControl();
    }

    public void RequestCharacterInfo(EntityUid entityUid)
    {
        RaiseNetworkEvent(new RequestCharacterInfoEvent(entityUid));
    }

    private void OnCharacterInfoEvent(CharacterInfoEvent msg, EntitySessionEventArgs args)
    {
        if (!EntityManager.TryGetComponent(msg.EntityUid, out CharacterInfoComponent characterInfoComponent))
            return;

        if (!EntityManager.TryGetComponent(msg.EntityUid, out MetaDataComponent metadata))
            return;

        UpdateUI(characterInfoComponent, msg.JobInfo, msg.Objectives, msg.Allegiances, msg.Briefing, metadata.EntityName);
        if (EntityManager.TryGetComponent(msg.EntityUid, out ISpriteComponent? spriteComponent))
        {
            characterInfoComponent.Control.SpriteView.Sprite = spriteComponent;
        }
    }

    private void UpdateUI(CharacterInfoComponent comp, (string Name, string Desc) jobInfo, Dictionary<string, List<ConditionInfo>> objectives, Dictionary<string, string> allegiances, string briefing, string entityName)
    {
        string FirstLetterToUpper(string str)
        {
            return char.ToUpper(str[0]) + str.Substring(1);
        }

        comp.Control.NameLabel.SetMessage(FormattedMessage.FromMarkup($"[color=#cccccc]{FirstLetterToUpper(jobInfo.Name)} {entityName}"));

        // Allegiances
        // Job desc first.
        comp.Control.AllegianceContainer.RemoveAllChildren();
        var jobLabel = new RichTextLabel() { Margin = new Thickness(0, 10, 0, 10), MaxWidth = 500};
        jobLabel.SetMessage(FormattedMessage.FromMarkup($"[color=#ffffff]{jobInfo.Desc}[/color]"));
        comp.Control.AllegianceContainer.AddChild(jobLabel);
        foreach (var (name, desc) in allegiances)
        {
            var allegianceLabel = new RichTextLabel() { Margin = new Thickness(0, 0, 0, 10), MaxWidth = 500};
            allegianceLabel.SetMessage(FormattedMessage.FromMarkup($"[color=#ffffff]As a member of[/color] [color=#cccccc]{name}[/color]: [color=#ffffff]{desc}[/color]"));
            comp.Control.AllegianceContainer.AddChild(allegianceLabel);
        }

        // Objectives
        comp.Control.ObjectivesContainer.RemoveAllChildren();

        if (objectives.Any())
        {
            comp.Control.ObjectivesContainer.AddChild(new Label
            {
                Text = Loc.GetString("character-info-objectives-label"),
                HorizontalAlignment = Control.HAlignment.Center
            });
        }
        foreach (var (groupId, objectiveConditions) in objectives)
        {
            var vbox = new BoxContainer
            {
                Orientation = BoxContainer.LayoutOrientation.Vertical,
                Modulate = Color.Gray
            };

            vbox.AddChild(new Label
            {
                Text = groupId,
                Modulate = Color.LightSkyBlue
            });

            foreach (var objectiveCondition in objectiveConditions)
            {
                var hbox = new BoxContainer
                {
                    Orientation = BoxContainer.LayoutOrientation.Horizontal
                };
                hbox.AddChild(new ProgressTextureRect
                {
                    Texture = objectiveCondition.SpriteSpecifier.Frame0(),
                    Progress = objectiveCondition.Progress,
                    VerticalAlignment = Control.VAlignment.Center
                });
                hbox.AddChild(new Control
                {
                    MinSize = (10,0)
                });
                hbox.AddChild(new BoxContainer
                    {
                        Orientation = BoxContainer.LayoutOrientation.Vertical,
                        Children =
                        {
                            new Label{Text = objectiveCondition.Title},
                            new Label{Text = objectiveCondition.Description}
                        }
                    }
                );
                vbox.AddChild(hbox);
            }
            var briefinghBox = new BoxContainer
            {
                Orientation = BoxContainer.LayoutOrientation.Horizontal
            };

            briefinghBox.AddChild(new Label
            {
                Text = briefing,
                Modulate = Color.Yellow
            });

            vbox.AddChild(briefinghBox);
            comp.Control.ObjectivesContainer.AddChild(vbox);
        }
    }
}
