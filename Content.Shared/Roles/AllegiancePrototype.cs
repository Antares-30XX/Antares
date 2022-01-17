using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Roles;

/// <summary>
///     Defines an "allegiance" that you have.
/// </summary>
[Prototype("allegiance")]
public class AllegiancePrototype : IPrototype
{
    [DataField("id", required: true)]
    public string ID { get; set; } = default!;

    [DataField("name", required: true)]
    public string Name = default!;

    [DataField("description", required: true)]
    public string Description = default!;
}
