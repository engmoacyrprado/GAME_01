using System;
using Game01.Combat.Core;
using Game01.Combat.Data;
using Game01.Combat.Runtime;
using UnityEngine;

namespace Game01.Combat.UI
{
    public class BattleHUDController : MonoBehaviour
    {
        private BattleContext _context;
        private PrototypeBattleView _view;
        private CombatActionDefinition _pendingAction;
        private CombatantRuntime _inspectedActor;
        private CombatantRuntime _lastDecisionActor;
        private bool _pinnedInspection;
        public event Action RestartRequested;
        public event Action PauseRequested;
        public event Action ReturnToMapRequested;
        private bool _adventureMode;

        private void Awake()
        {
            _view = new PrototypeBattleView(transform);
            _view.ActionMenu.ActionSelected += OnActionSelected;
            _view.Restart.onClick.AddListener(() => RestartRequested?.Invoke());
            _view.Pause.onClick.AddListener(() => PauseRequested?.Invoke());
            _view.ReturnToMap.onClick.AddListener(() => ReturnToMapRequested?.Invoke());
            _view.CancelPlan.onClick.AddListener(() =>
            {
                _context?.CancelPlannedAction(_inspectedActor);
                _pendingAction = null;
                Tick();
            });
        }

        public void Bind(BattleContext context)
        {
            _context = context;
            _pendingAction = null;
            _inspectedActor = null;
            _lastDecisionActor = null;
            _pinnedInspection = false;
            _view.Bind(context, OnCombatantClicked);
            Tick();
        }

        public void SetAdventureMode(bool enabled)
        {
            _adventureMode = enabled;
            _view.ReturnToMap.gameObject.SetActive(enabled);
        }

        public void SetTestPaused(bool paused, bool started)
        {
            _view.PauseLabel.text = paused ? (started ? "Continuar teste" : "Iniciar teste") : "Pausar teste";
            if (paused)
                _view.Status.text = (started ? "TESTE PAUSADO / voce pode preparar acoes" : "PRONTO / prepare os aliados ou inicie o teste")
                    + "\n" + MenuMessage();
            _view.Pause.interactable = _context != null && _context.CurrentPhase != CombatPhase.BattleEnded;
        }

        public void Tick()
        {
            if (_context == null) return;
            var actor = _context.CurrentActor;
            var turnActor = _context.CurrentPhase == CombatPhase.DecisionPhase && actor != null
                && actor.Team == CombatTeam.Allies && _context.SelectedAction == null ? actor : null;
            if (_inspectedActor != null && _inspectedActor.IsDefeated)
            {
                _inspectedActor = null;
                _pendingAction = null;
                _pinnedInspection = false;
            }
            if (turnActor != null && turnActor != _lastDecisionActor && !_pinnedInspection && _pendingAction == null)
                _inspectedActor = turnActor;
            _lastDecisionActor = turnActor;
            var ended = _context.CurrentPhase == CombatPhase.BattleEnded;
            _view.ReturnToMap.interactable = _adventureMode && ended;
            if (_inspectedActor != null && !ended) _view.ActionMenu.Show(_inspectedActor, _context);
            else _view.ActionMenu.Hide();
            if (turnActor != null)
                _view.Timer.SetTime(_context.DecisionTimerRemaining, _context.Config.decisionTimeLimit, turnActor.Definition.displayName);
            else _view.Timer.Hide();
            var phaseText = turnActor != null ? "VEZ DE " + turnActor.Definition.displayName :
                _context.CurrentPhase == CombatPhase.ResolveAction && actor != null ? "EM ACAO: " + actor.Definition.displayName :
                "ATB avancando / prepare as proximas acoes";
            if (ended)
                phaseText = _context.Winner == CombatTeam.Allies ? "VITORIA" : _context.Winner == CombatTeam.Enemies ? "DERROTA" : "EMPATE";
            _view.Status.text = phaseText + (ended ? "" : "\n" + MenuMessage());
            _view.CancelPlan.interactable = !ended && _context.GetPlannedAction(_inspectedActor) != null;
            _view.Refresh(_context, _inspectedActor, _pendingAction);
        }

        private string MenuMessage()
        {
            if (_inspectedActor == null) return "Clique no cartao ou boneco de um aliado para ver seus poderes.";
            if (_pendingAction != null) return _inspectedActor.Definition.displayName + ": " + _pendingAction.displayName + " / escolha o inimigo";
            var plan = _context.GetPlannedAction(_inspectedActor);
            return _inspectedActor.Definition.displayName + (plan != null
                ? $" / PROX: {plan.Action.displayName} > {plan.Target.Definition.displayName}"
                : " / escolha uma acao para sua proxima vez");
        }

        private void OnActionSelected(CombatActionDefinition action)
        {
            if (_context == null || !_context.CanUseAction(_inspectedActor, action)
                || _context.CurrentPhase == CombatPhase.BattleEnded) return;
            if (action.targetType == TargetType.Self) SubmitPlan(action, _inspectedActor);
            else _pendingAction = action;
            Tick();
        }

        private void SubmitPlan(CombatActionDefinition action, CombatantRuntime target)
        {
            if (!_context.PlanPlayerAction(_inspectedActor, action, target)) return;
            var prepared = _context.GetPlannedAction(_inspectedActor) != null;
            _context.Record($"{_inspectedActor.Definition.displayName}: {action.displayName} " + (prepared ? "preparada." : "confirmada."));
            _pendingAction = null;
            _pinnedInspection = false;
        }

        private void OnCombatantClicked(CombatantRuntime target)
        {
            if (_context.CurrentPhase == CombatPhase.BattleEnded || target.IsDefeated) return;
            if (target.Team == CombatTeam.Allies)
            {
                _inspectedActor = target;
                _pinnedInspection = true;
                _pendingAction = null;
            }
            else if (_pendingAction != null) SubmitPlan(_pendingAction, target);
            Tick();
        }
    }
}
