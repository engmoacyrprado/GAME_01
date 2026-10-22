using System.Collections.Generic;
using System;
using Game01.Combat.Core;
using Game01.Combat.Data;
using Game01.Combat.Systems;
using Game01.Combat.Actions;

namespace Game01.Combat.Runtime
{
    public sealed class BattleContext
    {
        public BattleContext(BattleConfig config)
        {
            if (config == null || config.atbThreshold <= 0f || config.tickScale <= 0f || config.decisionTimeLimit <= 0f)
                throw new ArgumentException("Battle configuration must have positive timing values.", nameof(config));
            Config = config;
            AtbSystem = new ATBSystem();
            ReadyQueueSystem = new ReadyQueueSystem();
            TurnDecisionSystem = new TurnDecisionSystem();
            CombatResolver = new CombatResolver();
            DamageSystem = new DamageSystem();
            VictoryConditionSystem = new VictoryConditionSystem();
            TargetingSystem = new TargetingSystem();
        }

        public BattleConfig Config { get; }
        public List<CombatantRuntime> Allies { get; } = new List<CombatantRuntime>();
        public List<CombatantRuntime> Enemies { get; } = new List<CombatantRuntime>();
        public List<CombatantRuntime> ReadyQueue { get; } = new List<CombatantRuntime>();
        public CombatPhase CurrentPhase { get; set; }
        public bool TimelinePaused { get; set; }
        public float DecisionTimerRemaining { get; set; }
        public CombatantRuntime CurrentActor { get; set; }
        public CombatActionDefinition SelectedAction { get; private set; }
        public CombatantRuntime SelectedTarget { get; private set; }
        public CombatTeam Winner { get; set; }
        public float PresentationTime { get; set; }
        public float ActionPresentationRemaining { get; set; }
        public CombatantRuntime PresentedActor { get; set; }
        public CombatantRuntime PresentedTarget { get; set; }
        private readonly Dictionary<CombatantRuntime, CombatActionCommand> _plans = new Dictionary<CombatantRuntime, CombatActionCommand>();
        public List<string> CombatLog { get; } = new List<string>();

        public void Record(string message)
        {
            CombatLog.Add(message);
            if (CombatLog.Count > 8) CombatLog.RemoveAt(0);
        }

        public ATBSystem AtbSystem { get; }
        public ReadyQueueSystem ReadyQueueSystem { get; }
        public TurnDecisionSystem TurnDecisionSystem { get; }
        public CombatResolver CombatResolver { get; }
        public DamageSystem DamageSystem { get; }
        public VictoryConditionSystem VictoryConditionSystem { get; }
        public TargetingSystem TargetingSystem { get; }

        public IEnumerable<CombatantRuntime> AllCombatants()
        {
            for (var i = 0; i < Allies.Count; i++)
            {
                yield return Allies[i];
            }

            for (var i = 0; i < Enemies.Count; i++)
            {
                yield return Enemies[i];
            }
        }

        public bool SetPlayerSelection(CombatActionDefinition action, CombatantRuntime target)
        {
            if (CurrentPhase != CombatPhase.DecisionPhase || CurrentActor == null
                || CurrentActor.Team != CombatTeam.Allies || SelectedAction != null
                || DecisionTimerRemaining <= 0f || !CanUseAction(CurrentActor, action)
                || !TargetingSystem.IsValidTarget(this, CurrentActor, action, target)) return false;
            SelectedAction = action;
            SelectedTarget = target;
            return true;
        }

        public bool CanUseAction(CombatantRuntime actor, CombatActionDefinition action)
        {
            if (actor == null || !actor.CanAct() || action == null || action.manaCost < 0f
                || action.timeCost < 0f || actor.CurrentMP < action.manaCost
                || !actor.Definition.availableActions.Contains(action)) return false;
            switch (action.actionType)
            {
                case CombatActionType.BasicAttack:
                case CombatActionType.Technique:
                case CombatActionType.Magic:
                    return action.timeCost > 0f && action.targetType == TargetType.SingleEnemy;
                case CombatActionType.Defend:
                case CombatActionType.PassTurn:
                    return action.manaCost == 0f && action.timeCost > 0f && action.targetType == TargetType.Self;
                case CombatActionType.Wait:
                    return action.manaCost == 0f && action.targetType == TargetType.Self;
                default: return false;
            }
        }

        public bool PlanPlayerAction(CombatantRuntime actor, CombatActionDefinition action, CombatantRuntime target)
        {
            if (CurrentPhase == CombatPhase.BattleEnded || !Allies.Contains(actor)
                || !CanUseAction(actor, action) || !TargetingSystem.IsValidTarget(this, actor, action, target)) return false;
            if (CurrentPhase == CombatPhase.DecisionPhase && CurrentActor == actor && SelectedAction == null)
            {
                if (!SetPlayerSelection(action, target)) return false;
                _plans.Remove(actor);
                return true;
            }
            _plans[actor] = new CombatActionCommand(actor, target, action);
            return true;
        }

        public CombatActionCommand GetPlannedAction(CombatantRuntime actor)
        {
            return actor != null && _plans.TryGetValue(actor, out var plan) ? plan : null;
        }

        public void CancelPlannedAction(CombatantRuntime actor)
        {
            if (actor != null) _plans.Remove(actor);
        }

        public void ClearPlannedActions() { _plans.Clear(); }

        public bool TryCommitPlannedAction(CombatantRuntime actor)
        {
            var plan = GetPlannedAction(actor);
            CancelPlannedAction(actor);
            if (plan == null) return false;
            if (SetPlayerSelection(plan.Action, plan.Target)) return true;
            Record(actor.Definition.displayName + ": plano indisponivel; escolha novamente.");
            return false;
        }

        public void ClearPlayerSelection()
        {
            SelectedAction = null;
            SelectedTarget = null;
        }
    }
}
