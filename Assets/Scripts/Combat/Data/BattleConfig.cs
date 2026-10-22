using Game01.Combat.Core;
using UnityEngine;

namespace Game01.Combat.Data
{
    [CreateAssetMenu(menuName = "GAME_01/Combat/Battle Config", fileName = "BattleConfig")]
    public class BattleConfig : ScriptableObject
    {
        [Min(1f)] public float atbThreshold = CombatConstants.DefaultAtbThreshold;
        [Min(0.01f)] public float tickScale = CombatConstants.DefaultTickScale;
        [Min(0.5f)] public float decisionTimeLimit = CombatConstants.DefaultDecisionTimeLimit;
        public bool pauseTimelineDuringPlayerDecision = true;
        [Min(0f)] public float actionPresentationDuration = 0.55f;
    }
}
