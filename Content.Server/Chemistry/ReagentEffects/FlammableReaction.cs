using System.Collections.Generic;
using Content.Server.Atmos.Components;
using Content.Server.Atmos.EntitySystems;
using Content.Shared.Administration.Logs;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Database;
using Content.Shared.FixedPoint;
using JetBrains.Annotations;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Set;

namespace Content.Server.Chemistry.ReagentEffects
{
    [UsedImplicitly]
    public class FlammableReaction : ReagentEffect
    {
        [DataField("multiplier")]
        public float Multiplier = 0.05f;
        
        public override bool ShouldLog => true;
        public override LogImpact LogImpact => LogImpact.Medium;

        public override void Effect(ReagentEffectArgs args)
        {
            if (!args.EntityManager.TryGetComponent(args.SolutionEntity, out FlammableComponent? flammable)) return;

            EntitySystem.Get<FlammableSystem>().AdjustFireStacks(args.SolutionEntity, args.Quantity.Float() * Multiplier, flammable);
            args.Source?.RemoveReagent(args.Reagent.ID, args.Quantity);
        }
    }
}
