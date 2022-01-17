using System;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Shared.Traits;

public class TraitSystem : EntitySystem
{
    [Dependency] private readonly IPrototypeManager _prototype = default!;
    [Dependency] private readonly IRobustRandom _random = default!;

    public string SelectTrait(string type="Normal", int variation=0)
    {
        if (!_prototype.TryIndex<TraitPoolPrototype>($"{type}{variation}", out var pool))
            return String.Empty;

        return _random.Pick(pool.PossibleSelections);
    }

    public string SelectJobTrait(string job)
    {
        if (!_prototype.TryIndex<TraitPoolPrototype>($"{job}", out var pool))
            return String.Empty;

        return _random.Pick(pool.PossibleSelections);
    }
}
