using Game01.Combat.Core;
using Game01.Combat.Runtime;

namespace Game01.Combat.StateMachine
{
    public sealed class TimelineUpdateState : IBattleState
    {
        private readonly BattleStateMachine _stateMachine;

        public TimelineUpdateState(BattleStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void Enter(BattleContext context)
        {
        }

        public void Tick(BattleContext context, float deltaTime)
        {
            context.AtbSystem.Update(context, deltaTime);
            context.ReadyQueueSystem.Refresh(context);

            if (context.ReadyQueue.Count > 0)
            {
                _stateMachine.ChangeState(context, CombatPhase.ActorReady);
            }
        }

        public void Exit(BattleContext context)
        {
        }
    }
}
