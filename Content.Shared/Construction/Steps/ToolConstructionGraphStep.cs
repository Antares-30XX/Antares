using System.Linq;
using Content.Shared.Examine;
using Content.Shared.Tools;
using Content.Shared.Tools.Components;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Utility;

namespace Content.Shared.Construction.Steps
{
    [DataDefinition]
    public class ToolConstructionGraphStep : ConstructionGraphStep
    {
        [DataField("tool", required:true, customTypeSerializer:typeof(PrototypeIdSerializer<ToolQualityPrototype>))]
        public string Tool { get; } = string.Empty;

        [DataField("fuel")] public float Fuel { get; } = 10;

        [DataField("examine")] public string ExamineOverride { get; } = string.Empty;

        public override void DoExamine(ExaminedEvent examinedEvent)
        {
            if (!string.IsNullOrEmpty(ExamineOverride))
            {
                examinedEvent.PushMarkup(Loc.GetString(ExamineOverride));
                return;
            }

            if (string.IsNullOrEmpty(Tool) || !IoCManager.Resolve<IPrototypeManager>().TryIndex(Tool, out ToolQualityPrototype? quality))
                return;

            examinedEvent.PushMarkup(Loc.GetString("construction-use-tool-entity", ("toolName", Loc.GetString(quality.ToolName))));

        }

        public override ConstructionGuideEntry GenerateGuideEntry()
        {
            var quality = IoCManager.Resolve<IPrototypeManager>().Index<ToolQualityPrototype>(Tool);

            return new ConstructionGuideEntry()
            {
                Localization = "construction-presenter-tool-step",
                Arguments = new (string, object)[]{("tool", quality.ToolName)},
                Icon = quality.Icon,
            };
        }
    }
}
