using System;
using System.Collections.Generic;
using System.Text;
using Game01.Combat.Core;
using Game01.Combat.Data;
using Game01.Combat.Runtime;
using Game01.Combat.Systems;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game01.Combat.UI
{
    // Disposable prototype presentation; battle rules live in BattleSession.
    public sealed class PrototypeBattleView
    {
        private readonly RectTransform _root;
        private readonly Font _font;
        private readonly List<CombatantCard> _cards = new List<CombatantCard>();
        private readonly Text _log;
        private readonly Text _queue;
        private readonly Color _ink = new Color32(228, 226, 206, 255);
        private readonly Color _gold = new Color32(232, 177, 72, 255);
        public ActionMenuView ActionMenu { get; }
        public DecisionTimerView Timer { get; }
        public Button Restart { get; }
        public Button Pause { get; }
        public Button CancelPlan { get; }
        public Button ReturnToMap { get; }
        public Text PauseLabel { get; }
        public Text Status { get; }

        public PrototypeBattleView(Transform parent)
        {
            _font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            var canvasObject = new GameObject("CombatCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasObject.transform.SetParent(parent, false);
            canvasObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280, 720);
            scaler.matchWidthOrHeight = 0.5f;
            _root = canvasObject.GetComponent<RectTransform>();
            var background = new GameObject("Background", typeof(RectTransform), typeof(Image));
            background.transform.SetParent(_root, false);
            var bgRect = background.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero; bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero; bgRect.offsetMax = Vector2.zero;
            background.GetComponent<Image>().color = new Color32(20, 31, 33, 255);
            background.GetComponent<Image>().raycastTarget = false;
            Label(_root, "GAME_01 / PRIMEIRA ERA", 40, 24, 600, 38, 28, _gold);
            Label(_root, "PROTOTIPO DE COMBATE    /    PC", 42, 65, 600, 24, 14, _ink);
            Restart = Button(_root, "Reiniciar batalha", 1010, 28, 230, 46, out _);
            Pause = Button(_root, "Iniciar teste", 760, 28, 230, 46, out var pauseLabel);
            PauseLabel = pauseLabel;
            ReturnToMap = Button(_root, "Voltar ao mapa", 1010, 78, 230, 34, out _);
            ReturnToMap.gameObject.SetActive(false);
            Label(_root, "ALIADOS", 40, 112, 300, 25, 16, _gold);
            Label(_root, "INIMIGOS", 940, 112, 300, 25, 16, _gold);
            Panel(_root, "Horizon", 375, 390, 530, 3, _gold);
            for (var i = 0; i < 9; i++) Panel(_root, "Terrain", 380 + i * 62, 400 + (i % 3) * 8, 35, 2, new Color32(60, 78, 71, 255));
            Status = Label(_root, "", 40, 470, 890, 54, 18, _gold);
            CancelPlan = Button(_root, "Limpar proxima acao", 650, 110, 250, 34, out _);
            var menuRect = Rect(_root, "Actions", 40, 530, 860, 148);
            ActionMenu = menuRect.gameObject.AddComponent<ActionMenuView>();
            var buttons = new Button[6];
            var labels = new Text[6];
            for (var i = 0; i < buttons.Length; i++)
                buttons[i] = Button(menuRect, "", (i % 3) * 285, (i / 3) * 72, 275, 64, out labels[i]);
            ActionMenu.Configure(buttons, labels);
            var timerRect = Rect(_root, "DecisionTimer", 940, 476, 300, 44);
            Timer = timerRect.gameObject.AddComponent<DecisionTimerView>();
            Timer.Configure(Bar(timerRect, "Time", 0, 30, 300, 8, _gold), Label(timerRect, "", 0, 0, 300, 26, 18, _gold));
            _log = Label(_root, "", 940, 538, 300, 155, 13, _ink);
            _queue = Label(_root, "", 40, 432, 1200, 28, 14, _ink);
            if (EventSystem.current == null)
            {
                var events = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
                events.transform.SetParent(parent, false);
            }
        }

        public void Bind(BattleContext context, Action<CombatantRuntime> selectTarget)
        {
            foreach (var card in _cards) UnityEngine.Object.Destroy(card.Root.gameObject);
            _cards.Clear();
            for (var i = 0; i < context.Allies.Count; i++) AddCard(context.Allies[i], 40, 150 + i * 141, selectTarget);
            for (var i = 0; i < context.Enemies.Count; i++) AddCard(context.Enemies[i], 940, 150 + i * 141, selectTarget);
        }

        private void AddCard(CombatantRuntime actor, float x, float y, Action<CombatantRuntime> selectTarget)
        {
            var root = Rect(_root, actor.Id, x, y, 300, 132);
            var button = root.gameObject.AddComponent<Image>();
            button.color = new Color32(34, 49, 49, 255);
            var target = root.gameObject.AddComponent<Button>();
            target.targetGraphic = button;
            target.onClick.AddListener(() => selectTarget(actor));
            var card = new CombatantCard { Root = root, Actor = actor, Target = target,
                Name = Label(root, "", 12, 8, 276, 24, 19, _ink),
                Resources = Label(root, "", 12, 38, 276, 22, 14, _ink),
                Health = Bar(root, "HP", 12, 64, 276, 8, new Color32(98, 165, 118, 255)),
                Atb = Bar(root, "ATB", 12, 86, 276, 8, _gold),
                Timing = Label(root, "", 12, 98, 276, 14, 11, _ink),
                Planned = Label(root, "", 12, 113, 276, 18, 11, _gold) };
            _cards.Add(card);
            // Stick figures keep the first era intentionally primitive.
            var figure = Rect(root, "StickFigure", actor.Team == CombatTeam.Allies ? 440 - x : 770 - x, 12, 60, 120);
            var hitArea = figure.gameObject.AddComponent<Image>(); hitArea.color = Color.clear;
            card.FigureButton = figure.gameObject.AddComponent<Button>(); card.FigureButton.targetGraphic = hitArea;
            card.FigureButton.onClick.AddListener(() => selectTarget(actor));
            card.Figure = figure;
            card.FigureOrigin = figure.anchoredPosition;
            card.FigureGroup = figure.gameObject.AddComponent<CanvasGroup>();
            DrawFigure(figure, actor);
        }

        private void DrawFigure(RectTransform figure, CombatantRuntime actor)
        {
            var id = actor.Definition.combatantId;
            Color color = id.Contains("mage") ? new Color32(106, 194, 207, 255) :
                id.Contains("bandit") ? new Color32(218, 137, 91, 255) :
                actor.Team == CombatTeam.Allies ? new Color32(98, 165, 118, 255) : _gold;
            if (id.Contains("wolf"))
            {
                Panel(figure, "Head", 38, 44, 22, 19, color);
                Limb(figure, "Back", 4, 65, 46, 65, color);
                Limb(figure, "Tail", 4, 65, -10, 47, color);
                Limb(figure, "LegA", 10, 65, 5, 104, color);
                Limb(figure, "LegB", 38, 65, 47, 104, color);
                Panel(figure, "Ear", 42, 36, 7, 12, color);
                return;
            }
            Panel(figure, "Head", 20, 0, 22, 22, color);
            Limb(figure, "Body", 31, 22, 31, 72, color);
            Limb(figure, "ArmL", 31, 38, 5, 55, color);
            Limb(figure, "ArmR", 31, 38, 55, 25, color);
            Limb(figure, "LegL", 31, 72, 10, 109, color);
            Limb(figure, "LegR", 31, 72, 52, 109, color);
            if (id.Contains("mage"))
            {
                Panel(figure, "Hat", 14, 0, 35, 5, color);
                Panel(figure, "HatTop", 23, -14, 17, 17, color);
                Limb(figure, "Staff", 55, 15, 55, 100, color);
                Panel(figure, "StaffOrb", 50, 8, 12, 12, _gold);
                Limb(figure, "Robe", 20, 40, 7, 88, color);
            }
            else if (id.Contains("bandit"))
            {
                Panel(figure, "Mask", 20, 6, 22, 6, new Color32(25, 32, 33, 255));
                Limb(figure, "Dagger", 55, 25, 65, 7, _ink);
            }
            else
            {
                Panel(figure, "Armor", 22, 27, 19, 22, color);
                Limb(figure, "Sword", 55, 25, 65, -5, _ink);
                Limb(figure, "Guard", 48, 23, 63, 29, _gold);
            }
        }

        public void Refresh(BattleContext context, CombatantRuntime inspected, CombatActionDefinition pending)
        {
            foreach (var card in _cards)
            {
                var actor = card.Actor;
                card.Name.text = (actor == context.CurrentActor ? "> " : "") + actor.Definition.displayName
                    + (actor.IsDefeated ? " [KO]" : actor.IsDefending ? " [DEF]" : "");
                card.Resources.text = $"HP {actor.CurrentHP:0}/{actor.Definition.stats.maxHP:0}    MP {actor.CurrentMP:0}/{actor.Definition.stats.maxMP:0}";
                card.Health.maxValue = actor.Definition.stats.maxHP; card.Health.value = actor.CurrentHP;
                card.Atb.maxValue = context.Config.atbThreshold; card.Atb.value = Mathf.Max(0, actor.CurrentATB);
                card.Timing.text = actor.CurrentATB < 0 ? $"Recuperacao: {-actor.CurrentATB:0} tempo" : $"ATB {actor.CurrentATB:0} / {context.Config.atbThreshold:0}";
                var plan = context.GetPlannedAction(actor);
                card.Planned.text = plan != null ? $"PROX: {plan.Action.displayName} > {plan.Target.Definition.displayName}" :
                    actor.Team == CombatTeam.Allies && !actor.IsDefeated ? "Clique para preparar" : "";
                card.Target.interactable = context.CurrentPhase != CombatPhase.BattleEnded && !actor.IsDefeated
                    && (actor.Team == CombatTeam.Allies || (pending != null && inspected != null
                    && context.TargetingSystem.IsValidTarget(context, inspected, pending, actor)));
                card.FigureButton.interactable = card.Target.interactable;
                var presenting = context.CurrentPhase == CombatPhase.ResolveAction;
                var progress = context.Config.actionPresentationDuration > 0
                    ? 1f - context.ActionPresentationRemaining / context.Config.actionPresentationDuration : 1f;
                var lunge = presenting && context.PresentedActor == actor ? Mathf.Sin(progress * Mathf.PI) * 18f : 0f;
                var idle = actor.IsDefeated ? 0f : Mathf.Sin(context.PresentationTime * (actor.Team == CombatTeam.Allies ? 2f : 3f) + _cards.IndexOf(card)) * 2f;
                card.Figure.anchoredPosition = card.FigureOrigin + new Vector2(lunge * (actor.Team == CombatTeam.Allies ? 1 : -1), idle);
                card.FigureGroup.alpha = actor.IsDefeated ? 0.25f : 1f;
                card.Root.GetComponent<Image>().color = presenting && context.PresentedTarget == actor && context.PresentedActor != actor
                    ? new Color32(102, 54, 44, 255) : actor == inspected ? new Color32(53, 76, 70, 255) : new Color32(34, 49, 49, 255);
            }
            _log.text = string.Join("\n", context.CombatLog);
            var order = new List<CombatantRuntime>();
            foreach (var actor in context.AllCombatants()) if (!actor.IsDefeated) order.Add(actor);
            order.Sort((a, b) => ComparePredictedOrder(a, b, context));
            var text = new StringBuilder("PREVISAO ATB   /   ");
            for (var i = 0; i < order.Count; i++)
            {
                if (i > 0) text.Append("  >  ");
                text.Append(order[i].Definition.displayName);
            }
            _queue.text = context.CurrentPhase == CombatPhase.BattleEnded ? "Batalha encerrada. Use Reiniciar batalha para tentar novamente." : text.ToString();
        }

        private static float Recovery(CombatantRuntime actor, BattleContext context)
        {
            return actor.SpeedFinal <= 0f ? float.PositiveInfinity : Mathf.Max(0, context.Config.atbThreshold - actor.CurrentATB) / actor.SpeedFinal;
        }

        public static int ComparePredictedOrder(CombatantRuntime a, CombatantRuntime b, BattleContext context)
        {
            var recovery = Recovery(a, context).CompareTo(Recovery(b, context));
            return recovery != 0 ? recovery : ReadyQueueSystem.CompareCombatants(a, b);
        }

        private void Limb(RectTransform parent, string name, float x, float y, float endX, float endY, Color color)
        {
            var direction = new Vector2(endX - x, y - endY);
            var limb = Panel(parent, name, x, y, direction.magnitude, 4, color);
            limb.pivot = new Vector2(0, 0.5f);
            limb.anchoredPosition = new Vector2(x, -y);
            limb.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        }

        private static RectTransform Rect(Transform parent, string name, float x, float y, float width, float height)
        {
            var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0, 1);
            rect.anchoredPosition = new Vector2(x, -y); rect.sizeDelta = new Vector2(width, height);
            return rect;
        }

        private static RectTransform Panel(Transform parent, string name, float x, float y, float width, float height, Color color)
        {
            var rect = Rect(parent, name, x, y, width, height);
            var image = rect.gameObject.AddComponent<Image>(); image.color = color; image.raycastTarget = false;
            return rect;
        }

        private Text Label(Transform parent, string text, float x, float y, float width, float height, int size, Color color)
        {
            var label = Rect(parent, "Label", x, y, width, height).gameObject.AddComponent<Text>();
            label.font = _font; label.fontSize = size; label.color = color; label.text = text;
            label.alignment = TextAnchor.UpperLeft; label.raycastTarget = false;
            return label;
        }

        private Button Button(Transform parent, string text, float x, float y, float width, float height, out Text label)
        {
            var rect = Panel(parent, "Button", x, y, width, height, new Color32(52, 71, 67, 255));
            var image = rect.GetComponent<Image>(); image.raycastTarget = true;
            var button = rect.gameObject.AddComponent<Button>(); button.targetGraphic = image;
            var colors = button.colors; colors.highlightedColor = new Color(1f, 0.85f, 0.5f); colors.disabledColor = new Color(0.45f, 0.45f, 0.45f); button.colors = colors;
            label = Label(rect, text, 10, 8, width - 20, height - 12, 17, _ink);
            return button;
        }

        private Slider Bar(Transform parent, string name, float x, float y, float width, float height, Color color)
        {
            var rect = Panel(parent, name, x, y, width, height, new Color32(14, 23, 25, 255));
            var fill = Panel(rect, "Fill", 0, 0, width, height, color);
            fill.anchorMin = Vector2.zero;
            fill.anchorMax = Vector2.one;
            fill.offsetMin = fill.offsetMax = Vector2.zero;
            var slider = rect.gameObject.AddComponent<Slider>();
            slider.fillRect = fill; slider.minValue = 0; slider.maxValue = 100; slider.interactable = false;
            return slider;
        }

        private sealed class CombatantCard
        {
            public RectTransform Root;
            public CombatantRuntime Actor;
            public Button Target;
            public Button FigureButton;
            public RectTransform Figure;
            public Vector2 FigureOrigin;
            public CanvasGroup FigureGroup;
            public Text Name, Resources, Timing, Planned;
            public Slider Health, Atb;
        }
    }
}
