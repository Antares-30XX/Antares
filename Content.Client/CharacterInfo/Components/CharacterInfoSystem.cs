using System.Linq;
using Content.Shared.CharacterInfo;
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

        UpdateUI(characterInfoComponent, msg, metadata.EntityName);
        if (EntityManager.TryGetComponent(msg.EntityUid, out ISpriteComponent? spriteComponent))
        {
            characterInfoComponent.Control.SpriteView.Sprite = spriteComponent;
        }
    }

    private void UpdateUI(CharacterInfoComponent comp, CharacterInfoEvent msg, string entityName)
    {
        string FirstLetterToUpper(string str)
        {
            return char.ToUpper(str[0]) + str.Substring(1);
        }

        comp.Control.NameLabel.SetMessage(FormattedMessage.FromMarkup($"[color=#cccccc]{FirstLetterToUpper(msg.JobInfo.Name)} {entityName}"));

        // Allegiances
        // Job desc first.
        comp.Control.AllegianceContainer.RemoveAllChildren();
        var jobLabel = new RichTextLabel() { Margin = new Thickness(0, 10, 0, 10), MaxWidth = 500};
        jobLabel.SetMessage(FormattedMessage.FromMarkup($"[color=#ffffff]{msg.JobInfo.Desc}[/color]"));
        comp.Control.AllegianceContainer.AddChild(jobLabel);
        foreach (var (name, desc) in msg.Allegiances)
        {
            var allegianceLabel = new RichTextLabel() { Margin = new Thickness(0, 0, 0, 10), MaxWidth = 500};
            allegianceLabel.SetMessage(FormattedMessage.FromMarkup($"[color=#ffffff]As a member of[/color] [color=#cccccc]{name}[/color]: [color=#ffffff]{desc}[/color]"));
            comp.Control.AllegianceContainer.AddChild(allegianceLabel);
        }

        // Traits
        comp.Control.TraitContainer.RemoveAllChildren();
        foreach (var trait in msg.Traits)
        {
            var label = new RichTextLabel() { Margin = new(10, 5, 0, 5)};
            label.SetMessage(FormattedMessage.FromMarkup($"[color=#cccccc]{trait}[/color]"));
            comp.Control.TraitContainer.AddChild(label);
        }

        // Objectives
        comp.Control.ObjectivesContainer.RemoveAllChildren();

        if (msg.Objectives.Any())
        {
            TabContainer.SetTabVisible(comp.Control.ObjectivesContainer, true);
            comp.Control.ObjectivesContainer.AddChild(new Label
            {
                Text = Loc.GetString("character-info-objectives-label"),
                HorizontalAlignment = Control.HAlignment.Center
            });
        }
        else
        {
            TabContainer.SetTabVisible(comp.Control.ObjectivesContainer, false);
        }
        foreach (var (groupId, objectiveConditions) in msg.Objectives)
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

            // Briefing
            var briefinghBox = new BoxContainer
            {
                Orientation = BoxContainer.LayoutOrientation.Horizontal
            };

            briefinghBox.AddChild(new Label
            {
                Text = msg.Briefing,
                Modulate = Color.Yellow
            });

            vbox.AddChild(briefinghBox);
            comp.Control.ObjectivesContainer.AddChild(vbox);
        }
    }
}
