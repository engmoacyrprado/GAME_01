using Game01.Combat.Core;
using Game01.Combat.Runtime;

namespace Game01.Combat.StateMachine
{
    public sealed class CheckEndState : IBattleState
    {
        private readonly BattleStateMachine _stateMachine;

        public CheckEndState(BattleStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void Enter(BattleContext context)
        {
            if (context.VictoryConditionSystem.TryGetWinner(context, out var winner))
            {
                context.Winner = winner;
                _stateMachine.ChangeState(context, CombatPhase.BattleEnded);
                return;
            }

            _stateMachine.ChangeState(context, CombatPhase.TimelineUpdate);
        }

        public void Tick(BattleContext context, float deltaTime)
        {
        }

        public void Exit(BattleContext context)
        {
        }
    }
}
