namespace Game01.Combat.Actions
{
    public sealed class DefendActionExecutor : ICombatActionExecutor
    {
        public CombatActionResult Execute(CombatActionCommand command)
        {
            command.Actor.IsDefending = true;
            command.Actor.ConsumeTimeCost(command.Action.timeCost);

            return new CombatActionResult
            {
                DidHit = false,
                TimeCostApplied = command.Action.timeCost,
            };
        }
    }
}
