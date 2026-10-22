using Game01.Combat.Core;
using UnityEngine;

namespace Game01.Combat.Data
{
    [CreateAssetMenu(menuName = "GAME_01/Combat/Action Definition", fileName = "ActionDefinition")]
    public class CombatActionDefinition : ScriptableObject
    {
        public string actionId = "action.basic_attack";
        public string displayName = "Ataque Basico";
        public CombatActionType actionType = CombatActionType.BasicAttack;
        public TargetType targetType = TargetType.SingleEnemy;
        [Min(0f)] public float timeCost = 100f;
        [Min(0f)] public float manaCost = 0f;
        [Min(0f)] public float basePower = 10f;
    }
}
