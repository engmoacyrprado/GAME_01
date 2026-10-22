using Game01.Combat.Core;
using Game01.Combat.Runtime;

namespace Game01.Combat.StateMachine
{
    public sealed class ActorReadyState : IBattleState
    {
        private readonly BattleStateMachine _stateMachine;

        public ActorReadyState(BattleStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void Enter(BattleContext context)
        {
            context.CurrentActor = context.ReadyQueueSystem.PopNext(context);
            if (context.CurrentActor != null) context.CurrentActor.IsDefending = false;
            _stateMachine.ChangeState(context, CombatPhase.DecisionPhase);
        }

        public void Tick(BattleContext context, float deltaTime)
        {
        }

        public void Exit(BattleContext context)
        {
        }
    }
}
