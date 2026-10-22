using Game01.Combat.Core;
using Game01.Combat.Runtime;

namespace Game01.Combat.StateMachine
{
    public sealed class StartCombatState : IBattleState
    {
        private readonly BattleStateMachine _stateMachine;

        public StartCombatState(BattleStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void Enter(BattleContext context)
        {
            context.TimelinePaused = false;
            context.Record("Batalha iniciada. Escolha uma acao e depois o alvo.");
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
