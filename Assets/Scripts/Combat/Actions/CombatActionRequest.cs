using Game01.Combat.Data;
using Game01.Combat.Runtime;

namespace Game01.Combat.Actions
{
    public sealed class CombatActionRequest
    {
        public CombatantRuntime Actor;
        public CombatantRuntime Target;
        public CombatActionDefinition Action;
    }
}
