using System;
using System.Collections.Generic;
using Game01.Combat.Data;
using Game01.Combat.Core;
using Game01.Combat.Runtime;
using UnityEngine;
using UnityEngine.UI;

namespace Game01.Combat.UI
{
    public class ActionMenuView : MonoBehaviour
    {
        [SerializeField] private Button[] actionButtons = Array.Empty<Button>();
        [SerializeField] private Text[] actionLabels = Array.Empty<Text>();
        private IReadOnlyList<CombatActionDefinition> _actions;
        public event Action<CombatActionDefinition> ActionSelected;

        public void Configure(Button[] buttons, Text[] labels)
        {
            actionButtons = buttons;
            actionLabels = labels;
            for (var i = 0; i < actionButtons.Length; i++)
            {
                var index = i;
                if (actionButtons[i] != null) actionButtons[i].onClick.AddListener(() => OnClicked(index));
            }
        }

        private void Awake()
        {
            if (actionButtons.Length > 0) Configure(actionButtons, actionLabels);
        }

        public void Show(CombatantRuntime actor, BattleContext context)
        {
            _actions = actor.Definition.availableActions;
            gameObject.SetActive(true);
            for (var i = 0; i < actionButtons.Length; i++)
            {
                var button = actionButtons[i];
                if (button == null) continue;
                var action = i < _actions.Count ? _actions[i] : null;
                button.gameObject.SetActive(action != null);
                if (action == null) continue;
                button.interactable = context.CanUseAction(actor, action);
                if (i < actionLabels.Length && actionLabels[i] != null)
                    actionLabels[i].text = action.actionType == CombatActionType.Wait
                        ? action.displayName + "\nConserva 50% ATB"
                        : $"{action.displayName}\n{action.timeCost:0} tempo | {action.manaCost:0} MP";
            }
        }

        public void Hide() { gameObject.SetActive(false); }
        public void Select(CombatActionDefinition action) { ActionSelected?.Invoke(action); }
        private void OnClicked(int index)
        {
            if (_actions != null && index < _actions.Count && actionButtons[index].interactable) Select(_actions[index]);
        }
    }
}
