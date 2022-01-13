using System;
using Content.Shared.Doors;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Client.Doors;

public class BayAirlockVisualizer : AppearanceVisualizer, ISerializationHooks
{
    [Dependency] private readonly IEntityManager e = default!;

    private const string AnimationKey = "airlock_animation";

    [DataField("doorColor")]
    public Color? DoorColor;

    [DataField("stripeColor")]
    public Color? StripeColor;

    [DataField("delay")]
    public float Delay = 0.6f;

    public Animation CloseAnimation = default!;
    public Animation OpenAnimation = default!;
    public Animation DenyAnimation = default!;

    void ISerializationHooks.AfterDeserialization()
    {
        CloseAnimation = new Animation {Length = TimeSpan.FromSeconds(Delay)};
        {
            foreach (var val in Enum.GetValues<BayDoorAnimatedLayers>())
            {
                var flick = new AnimationTrackSpriteFlick();
                CloseAnimation.AnimationTracks.Add(flick);
                flick.LayerKey = val;
                flick.KeyFrames.Add(new AnimationTrackSpriteFlick.KeyFrame("closing", 0f));
            }
        }

        OpenAnimation = new Animation {Length = TimeSpan.FromSeconds(Delay)};
        {
            foreach (var val in Enum.GetValues<BayDoorAnimatedLayers>())
            {
                var flick = new AnimationTrackSpriteFlick();
                OpenAnimation.AnimationTracks.Add(flick);
                flick.LayerKey = val;
                flick.KeyFrames.Add(new AnimationTrackSpriteFlick.KeyFrame("opening", 0f));
            }
        }
    }

    public override void InitializeEntity(EntityUid entity)
    {
        base.InitializeEntity(entity);

        IoCManager.InjectDependencies(this);

        if (!e.TryGetComponent<SpriteComponent>(entity, out var sprite))
            return;

        e.EnsureComponent<AnimationPlayerComponent>(entity);

        if (DoorColor != null)
        {
            if (sprite.LayerMapTryGet(BayDoorAnimatedLayers.Color, out var colorLayer))
            {
                sprite.LayerSetColor(colorLayer, DoorColor.Value);
            }
            if (sprite.LayerMapTryGet(BayDoorAnimatedLayers.ColorFill, out var colorFillLayer))
            {
                sprite.LayerSetColor(colorFillLayer, DoorColor.Value);
            }
        }
        if (StripeColor != null)
        {
            if (sprite.LayerMapTryGet(BayDoorAnimatedLayers.Stripe, out var stripeLayer))
            {
                sprite.LayerSetColor(stripeLayer, StripeColor.Value);
            }
            if (sprite.LayerMapTryGet(BayDoorAnimatedLayers.StripeFill, out var stripeFillLayer))
            {
                sprite.LayerSetColor(stripeFillLayer, StripeColor.Value);
            }
        }
    }

    public override void OnChangeData(AppearanceComponent component)
    {
        base.OnChangeData(component);

        var sprite = e.GetComponent<SpriteComponent>(component.Owner);
        var animPlayer = e.GetComponent<AnimationPlayerComponent>(component.Owner);
        if (!component.TryGetData(DoorVisuals.VisualState, out DoorVisualState state))
        {
            state = DoorVisualState.Closed;
        }

        if (animPlayer.HasRunningAnimation(AnimationKey))
        {
            animPlayer.Stop(AnimationKey);
        }

        void SetAnimatedTo(string st)
        {
            foreach (var val in Enum.GetValues<BayDoorAnimatedLayers>())
            {
                if (sprite!.LayerMapTryGet(val, out var lay))
                {
                    sprite.LayerSetState(lay, st);
                }
            }
        }

        var welded = false;

        switch (state)
        {
            case DoorVisualState.Open:
                SetAnimatedTo("open");
                break;
            case DoorVisualState.Closed:
                SetAnimatedTo("closed");
                break;
            case DoorVisualState.Opening:
                animPlayer.Play(OpenAnimation, AnimationKey);
                break;
            case DoorVisualState.Closing:
                animPlayer.Play(CloseAnimation, AnimationKey);
                break;
            case DoorVisualState.Deny:
                //if (!animPlayer.HasRunningAnimation(AnimationKey))
                //    animPlayer.Play(DenyAnimation, AnimationKey);
                break;
            case DoorVisualState.Welded:
                welded = true;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (sprite.LayerMapTryGet(BayDoorAncillaryLayers.Welded, out var layer))
        {
            sprite.LayerSetVisible(layer, welded);
        }
    }
}

public enum BayDoorAnimatedLayers
{
    Base,
    Color,
    ColorFill,
    Stripe,
    StripeFill,
}

public enum BayDoorAncillaryLayers
{
    Bolts,
    Lights,
    Welded
}
