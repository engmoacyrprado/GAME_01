using Game01.Combat.Actions;
using Game01.Combat.Core;
using Game01.Combat.Data;
using Game01.Combat.Runtime;

namespace Game01.Combat.StateMachine
{
    public sealed class ResolveActionState : IBattleState
    {
        private readonly BattleStateMachine _stateMachine;
        public ResolveActionState(BattleStateMachine stateMachine) { _stateMachine = stateMachine; }

        public void Enter(BattleContext context)
        {
            var actor = context.CurrentActor;
            context.PresentedActor = actor;
            context.PresentedTarget = null;
            if (actor != null)
            {
                var action = actor.Team == CombatTeam.Allies ? context.SelectedAction : ChooseEnemyAction(context, actor);
                if (!actor.CanAct() || action == null)
                {
                    // Timeout never depends on an action asset and can never become an attack.
                    actor.ConsumeTimeCost(context.Config.atbThreshold);
                    context.Record(actor.Definition.displayName + " passou o turno.");
                }
                else
                {
                    var target = action.targetType == TargetType.Self ? actor :
                        context.SelectedTarget ?? context.TargetingSystem.GetDefaultTarget(context, actor);
                    context.PresentedTarget = target;
                    if (context.CanUseAction(actor, action) && context.TargetingSystem.IsValidTarget(context, actor, action, target))
                        Execute(context, actor, action, target);
                    else
                    {
                        actor.ConsumeTimeCost(context.Config.atbThreshold);
                        context.Record(actor.Definition.displayName + ": acao indisponivel; turno consumido.");
                    }
                }
            }
            context.ClearPlayerSelection();
            context.TimelinePaused = true;
            context.ActionPresentationRemaining = context.Config.actionPresentationDuration;
            if (context.ActionPresentationRemaining <= 0f)
                _stateMachine.ChangeState(context, CombatPhase.Aftermath);
        }

        private static CombatActionDefinition ChooseEnemyAction(BattleContext context, CombatantRuntime actor)
        {
            foreach (var action in actor.Definition.availableActions)
                if (context.CanUseAction(actor, action)) return action;
            return null;
        }

        private static void Execute(BattleContext context, CombatantRuntime actor, CombatActionDefinition action, CombatantRuntime target)
        {
            var command = new CombatActionCommand(actor, target, action);
            CombatActionResult result;
            switch (action.actionType)
            {
                case CombatActionType.Defend:
                    result = new DefendActionExecutor().Execute(command);
                    break;
                case CombatActionType.Wait:
                    result = new WaitActionExecutor(context.Config.atbThreshold).Execute(command);
                    break;
                case CombatActionType.PassTurn:
                    actor.ConsumeTimeCost(action.timeCost);
                    result = new CombatActionResult();
                    break;
                default:
                    result = context.CombatResolver.Resolve(command);
                    break;
            }
            context.Record(result.DidHit
                ? $"{actor.Definition.displayName}: {action.displayName} em {target.Definition.displayName} (-{result.DamageAmount:0} HP)."
                : $"{actor.Definition.displayName}: {action.displayName}.");
            if (target.IsDefeated) context.CancelPlannedAction(target);
        }

        public void Tick(BattleContext context, float deltaTime)
        {
            context.ActionPresentationRemaining = UnityEngine.Mathf.Max(0f, context.ActionPresentationRemaining - deltaTime);
            if (context.ActionPresentationRemaining <= 0f)
                _stateMachine.ChangeState(context, CombatPhase.Aftermath);
        }
        public void Exit(BattleContext context) { }
    }
}
