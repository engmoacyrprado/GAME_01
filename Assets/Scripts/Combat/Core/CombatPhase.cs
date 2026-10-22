namespace Game01.Combat.Core
{
    public enum CombatPhase
    {
        None = 0,
        StartCombat = 1,
        TimelineUpdate = 2,
        ActorReady = 3,
        DecisionPhase = 4,
        ResolveAction = 5,
        Aftermath = 6,
        CheckEnd = 7,
        BattleEnded = 8,
    }
}
