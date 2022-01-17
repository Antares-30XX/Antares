using Content.Client.CharacterInterface;
using Content.Client.HUD.UI;
using Content.Client.Stylesheets;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;

namespace Content.Client.CharacterInfo.Components
{
    [RegisterComponent]
    public sealed class CharacterInfoComponent : Component, ICharacterUI
    {
        public override string Name => "CharacterInfo";

        public CharacterInfoControl Control = default!;

        public Control Scene { get; set; } = default!;
        public UIPriority Priority => UIPriority.Info;

        public void Opened()
        {
            EntitySystem.Get<CharacterInfoSystem>().RequestCharacterInfo(Owner);
        }

        public sealed class CharacterInfoControl : BoxContainer
        {
            public SpriteView SpriteView { get; }
            public RichTextLabel NameLabel { get; }

            public TabContainer Tabs { get; }

            public BoxContainer ObjectivesContainer { get; }

            public BoxContainer InfoContainer { get; }

            public BoxContainer AllegianceContainer { get; }

            public BoxContainer TraitContainer { get; }

            public CharacterInfoControl()
            {
                IoCManager.InjectDependencies(this);

                Orientation = LayoutOrientation.Vertical;

                AddChild(Tabs = new TabContainer());

                InfoContainer = new BoxContainer
                {
                    Orientation = LayoutOrientation.Vertical,
                    Children =
                    {
                        (SpriteView = new SpriteView { OverrideDirection = Direction.South, Scale = (2, 2) }),
                        (NameLabel = new RichTextLabel()
                            { HorizontalAlignment = HAlignment.Center, Margin = new(0, 5, 0, 0) }),
                        (AllegianceContainer = new BoxContainer() { Orientation = LayoutOrientation.Vertical }),
                    },
                    Margin = new(0, 0, 0, 15),
                };

                Tabs.AddChild(InfoContainer);
                TabContainer.SetTabTitle(InfoContainer, "Info");

                TraitContainer = new BoxContainer
                {
                    Orientation = LayoutOrientation.Vertical
                };
                Tabs.AddChild(TraitContainer);
                TabContainer.SetTabTitle(TraitContainer, "Traits");

                ObjectivesContainer = new BoxContainer
                {
                    Orientation = LayoutOrientation.Vertical
                };
                Tabs.AddChild(ObjectivesContainer);
                TabContainer.SetTabTitle(ObjectivesContainer, "Objectives");
            }
        }
    }
}
