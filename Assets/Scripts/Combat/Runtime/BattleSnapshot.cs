using System.Collections.Generic;
using Game01.Combat.Core;

namespace Game01.Combat.Runtime
{
    public sealed class BattleSnapshot
    {
        public CombatPhase Phase;
        public List<CombatantRuntime> Allies = new List<CombatantRuntime>();
        public List<CombatantRuntime> Enemies = new List<CombatantRuntime>();
    }
}
