using Content.Shared.Roles;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Access.Components
{
    [RegisterComponent]
    public class PresetIdCardComponent : Component
    {
        public override string Name => "PresetIdCard";

        [DataField("job", customTypeSerializer:typeof(PrototypeIdSerializer<JobPrototype>))]
        public readonly string? JobName;
    }
}
