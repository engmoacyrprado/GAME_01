using UnityEngine;

namespace Game01.Combat.Data
{
    [CreateAssetMenu(menuName = "GAME_01/Combat/Status Effect", fileName = "StatusEffect")]
    public class StatusEffectDefinition : ScriptableObject
    {
        public string effectId = "status.none";
        public string displayName = "Status";
        [Min(0f)] public float speedFlatPenalty = 0f;
        [Min(0f)] public float speedMultiplier = 1f;
        public bool blocksActions = false;
    }
}
