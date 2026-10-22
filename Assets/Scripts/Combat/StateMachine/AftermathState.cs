using Game01.Combat.Core;
using Game01.Combat.Runtime;

namespace Game01.Combat.StateMachine
{
    public sealed class AftermathState : IBattleState
    {
        private readonly BattleStateMachine _stateMachine;

        public AftermathState(BattleStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void Enter(BattleContext context)
        {
            context.TimelinePaused = false;
            context.CurrentActor = null;
            _stateMachine.ChangeState(context, CombatPhase.CheckEnd);
        }

        public void Tick(BattleContext context, float deltaTime)
        {
        }

        public void Exit(BattleContext context)
        {
        }
    }
}
