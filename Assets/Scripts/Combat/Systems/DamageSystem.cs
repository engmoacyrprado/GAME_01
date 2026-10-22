using UnityEngine;

namespace Game01.Combat.Systems
{
    public sealed class DamageSystem
    {
        public float CalculateBasicDamage(float attack, float defense, float power)
        {
            return Mathf.Max(1f, attack + power - defense);
        }
    }
}
