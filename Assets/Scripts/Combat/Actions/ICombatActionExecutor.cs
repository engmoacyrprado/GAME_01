namespace Game01.Combat.Actions
{
    public interface ICombatActionExecutor
    {
        CombatActionResult Execute(CombatActionCommand command);
    }
}
