using System;
using System.Collections.Generic;
using Content.Shared.Construction;
using Content.Shared.Examine;
using JetBrains.Annotations;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Construction.Conditions
{
    [UsedImplicitly]
    [DataDefinition]
    public class AnyConditions : IGraphCondition
    {
        [DataField("conditions")]
        public IGraphCondition[] Conditions { get; } = Array.Empty<IGraphCondition>();

        public bool Condition(EntityUid uid, IEntityManager entityManager)
        {
            foreach (var condition in Conditions)
            {
                if (condition.Condition(uid, entityManager))
                    return true;
            }

            return false;
        }

        public bool DoExamine(ExaminedEvent args)
        {
            args.PushMarkup(Loc.GetString("construction-examine-condition-any-conditions"));

            foreach (var condition in Conditions)
            {
                condition.DoExamine(args);
            }

            return true;
        }

        public IEnumerable<ConstructionGuideEntry> GenerateGuideEntry()
        {
            yield return new ConstructionGuideEntry()
            {
                Localization = "construction-guide-condition-any-conditions",
            };

            foreach (var condition in Conditions)
            {
                foreach (var entry in condition.GenerateGuideEntry())
                {
                    entry.Padding += 4;
                    yield return entry;
                }
            }
        }
    }
}
