using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Client.Doors;

public class BayAirlockVisualizer : AppearanceVisualizer
{
    [DataField("doorColor")]
    public Color? DoorColor;

    [DataField("stripeColor")]
    public Color? StripeColor;

    public override void InitializeEntity(EntityUid entity)
    {
        base.InitializeEntity(entity);

        var e = IoCManager.Resolve<IEntityManager>();
        if (!e.TryGetComponent<SpriteComponent>(entity, out var sprite))
            return;

        if (DoorColor != null)
        {
            if (sprite.LayerMapTryGet(BayDoorVisualLayers.Color, out var colorLayer))
            {
                sprite.LayerSetColor(colorLayer, DoorColor.Value);
            }
            if (sprite.LayerMapTryGet(BayDoorVisualLayers.ColorFill, out var colorFillLayer))
            {
                sprite.LayerSetColor(colorFillLayer, DoorColor.Value);
            }
        }
        if (StripeColor != null)
        {
            if (sprite.LayerMapTryGet(BayDoorVisualLayers.Stripe, out var stripeLayer))
            {
                sprite.LayerSetColor(stripeLayer, StripeColor.Value);
            }
            if (sprite.LayerMapTryGet(BayDoorVisualLayers.StripeFill, out var stripeFillLayer))
            {
                sprite.LayerSetColor(stripeFillLayer, StripeColor.Value);
            }
        }
    }

    public override void OnChangeData(AppearanceComponent component)
    {
        base.OnChangeData(component);
    }
}

public enum BayDoorVisualLayers
{
    BaseFill,
    Color,
    ColorFill,
    Stripe,
    StripeFill,
    Bolts,
    Deny,
    Lights,
    Welded
}
