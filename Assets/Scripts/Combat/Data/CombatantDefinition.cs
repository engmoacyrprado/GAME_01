using System.Collections.Generic;
using UnityEngine;

namespace Game01.Combat.Data
{
    [CreateAssetMenu(menuName = "GAME_01/Combat/Combatant Definition", fileName = "CombatantDefinition")]
    public class CombatantDefinition : ScriptableObject
    {
        public string combatantId = "combatant.hero";
        public string displayName = "Heroi";
        public CombatStatsDefinition stats = new CombatStatsDefinition();
        public List<CombatActionDefinition> availableActions = new List<CombatActionDefinition>();
    }
}
