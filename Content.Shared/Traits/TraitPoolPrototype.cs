using System.Collections.Generic;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Traits;

[Prototype("traitPool")]
public class TraitPoolPrototype : IPrototype
{
    [DataField("id", required: true)]
    public string ID { get; } = default!;

    [DataField("selections", required: true)]
    public List<string> PossibleSelections = default!;
}
