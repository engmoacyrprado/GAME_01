namespace Game01.Combat.Actions
{
    public sealed class CombatActionResult
    {
        public bool DidHit;
        public float DamageAmount;
        public float ManaSpent;
        public float TimeCostApplied;
        public bool TargetDefeated;
    }
}
