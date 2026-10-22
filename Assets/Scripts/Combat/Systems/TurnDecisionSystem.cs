using Game01.Combat.Core;
using Game01.Combat.Data;
using Game01.Combat.Runtime;

namespace Game01.Combat.Systems
{
    public sealed class TurnDecisionSystem
    {
        public void BeginPlayerDecision(BattleContext context)
        {
            context.TimelinePaused = context.Config.pauseTimelineDuringPlayerDecision;

            context.DecisionTimerRemaining = context.Config.decisionTimeLimit;
        }

        public bool UpdateTimer(BattleContext context, float deltaTime)
        {
            context.DecisionTimerRemaining = UnityEngine.Mathf.Max(0f, context.DecisionTimerRemaining - deltaTime);
            return context.DecisionTimerRemaining <= 0f;
        }

        public CombatActionType GetTimeoutFallbackAction()
        {
            return CombatActionType.PassTurn;
        }

        public CombatActionDefinition GetTimeoutFallbackDefinition(CombatantRuntime actor)
        {
            var fallbackType = GetTimeoutFallbackAction();
            var actions = actor.Definition.availableActions;
            for (var i = 0; i < actions.Count; i++)
            {
                if (actions[i] != null && actions[i].actionType == fallbackType)
                {
                    return actions[i];
                }
            }

            return null;
        }
    }
}
