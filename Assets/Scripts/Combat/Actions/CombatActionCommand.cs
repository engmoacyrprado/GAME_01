using Game01.Combat.Data;
using Game01.Combat.Runtime;

namespace Game01.Combat.Actions
{
    public sealed class CombatActionCommand
    {
        public CombatActionCommand(CombatantRuntime actor, CombatantRuntime target, CombatActionDefinition action)
        {
            Actor = actor;
            Target = target;
            Action = action;
        }

        public CombatantRuntime Actor { get; }
        public CombatantRuntime Target { get; }
        public CombatActionDefinition Action { get; }
    }
}
