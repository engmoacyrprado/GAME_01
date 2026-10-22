using Game01.Combat.Core;
using Game01.Combat.Runtime;

namespace Game01.Combat.StateMachine
{
    public sealed class DecisionPhaseState : IBattleState
    {
        private readonly BattleStateMachine _stateMachine;

        public DecisionPhaseState(BattleStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void Enter(BattleContext context)
        {
            if (context.CurrentActor == null)
            {
                _stateMachine.ChangeState(context, CombatPhase.TimelineUpdate);
                return;
            }

            context.ClearPlayerSelection();

            if (!context.CurrentActor.CanAct())
            {
                context.CancelPlannedAction(context.CurrentActor);
                _stateMachine.ChangeState(context, CombatPhase.ResolveAction);
                return;
            }

            if (context.CurrentActor.Team == CombatTeam.Allies)
            {
                context.TurnDecisionSystem.BeginPlayerDecision(context);
                if (context.TryCommitPlannedAction(context.CurrentActor))
                    _stateMachine.ChangeState(context, CombatPhase.ResolveAction);
                return;
            }

            _stateMachine.ChangeState(context, CombatPhase.ResolveAction);
        }

        public void Tick(BattleContext context, float deltaTime)
        {
            if (context.CurrentActor == null || context.CurrentActor.Team != CombatTeam.Allies)
            {
                return;
            }

            context.AtbSystem.Update(context, deltaTime);

            if (context.SelectedAction != null)
            {
                _stateMachine.ChangeState(context, CombatPhase.ResolveAction);
                return;
            }

            if (context.TurnDecisionSystem.UpdateTimer(context, deltaTime))
            {
                _stateMachine.ChangeState(context, CombatPhase.ResolveAction);
            }
        }

        public void Exit(BattleContext context)
        {
        }
    }
}
