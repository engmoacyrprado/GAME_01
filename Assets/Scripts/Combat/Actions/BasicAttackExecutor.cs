using Game01.Combat.Systems;

namespace Game01.Combat.Actions
{
    public sealed class BasicAttackExecutor : ICombatActionExecutor
    {
        private readonly CombatResolver _resolver;

        public BasicAttackExecutor(CombatResolver resolver)
        {
            _resolver = resolver;
        }

        public CombatActionResult Execute(CombatActionCommand command)
        {
            return _resolver.Resolve(command);
        }
    }
}
