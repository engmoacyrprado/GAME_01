using Game01.Combat.Core;

namespace Game01.Combat.Actions
{
    public sealed class WaitActionExecutor : ICombatActionExecutor
    {
        private readonly float _threshold;

        public WaitActionExecutor(float threshold = CombatConstants.DefaultAtbThreshold)
        {
            _threshold = threshold;
        }

        public CombatActionResult Execute(CombatActionCommand command)
        {
            var previous = command.Actor.CurrentATB;
            command.Actor.RetainAtb(_threshold, CombatConstants.WaitRetainedAtbRatio);

            return new CombatActionResult
            {
                DidHit = false,
                TimeCostApplied = previous - command.Actor.CurrentATB,
            };
        }
    }
}
