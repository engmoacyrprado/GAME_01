using System.Collections.Generic;
using Game01.Combat.Data;

namespace Game01.Combat.Runtime
{
    public sealed class ActionSelectionData
    {
        public CombatantRuntime Actor;
        public List<CombatActionDefinition> Actions = new List<CombatActionDefinition>();
        public CombatActionDefinition DefaultAction;
    }
}
