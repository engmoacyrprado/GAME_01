using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game01.Adventure
{
    public sealed class ExplorationHud
    {
        private readonly Text _title, _objective, _prompt, _direction;
        private readonly Font _font;

        public ExplorationHud(Transform parent, Action restart)
        {
            _font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            var root = new GameObject("ExplorationCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            root.transform.SetParent(parent, false);
            root.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            root.GetComponent<Canvas>().sortingOrder = 5000;
            var scaler = root.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280, 720);
            scaler.matchWidthOrHeight = 0.5f;
            Panel(root.transform, "Header", 0, 0, 1280, 112);
            Panel(root.transform, "Footer", 0, 635, 1280, 85);
            _title = Label(root.transform, 32, 18, 850, 32, 24, new Color32(233, 185, 87, 255));
            _objective = Label(root.transform, 32, 62, 900, 32, 18, new Color32(225, 227, 208, 255));
            _direction = Label(root.transform, 32, 651, 1200, 24, 16, new Color32(233, 185, 87, 255));
            _prompt = Label(root.transform, 32, 682, 1200, 28, 16, new Color32(225, 227, 208, 255));
            var reset = Panel(root.transform, "RestartJourney", 1000, 28, 250, 46);
            reset.GetComponent<Image>().raycastTarget = true;
            reset.GetComponent<Image>().color = new Color32(54, 77, 66, 255);
            var button = reset.gameObject.AddComponent<Button>(); button.targetGraphic = reset.GetComponent<Image>();
            button.onClick.AddListener(() => restart());
            Label(reset, 12, 10, 230, 30, 17, Color.white).text = "Reiniciar percurso";
            if (EventSystem.current == null)
            {
                var events = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
                events.transform.SetParent(parent, false);
            }
        }

        public void Refresh(AdventureRun run, Vector2 position, string prompt)
        {
            _title.text = run.Era == VisualEra.Primitive ? "GAME_01 / ERA 01 / AREA DE EXPLORACAO" : "GAME_01 / ERA 02 / PRIMEIRA TRANSFORMACAO";
            _objective.text = run.Completed ? "Percurso concluido: exploracao, batalha e mudanca visual conectadas." :
                !run.EncounterWon ? "1 / Encontre o Lobo e o Bandido no cruzamento e interaja com E." :
                run.Era == VisualEra.Primitive ? "2 / Vitoria registrada. Encontre o marco dourado a nordeste." :
                "3 / O cenario evoluiu. Procure o portal a leste do marco.";
            var target = !run.EncounterWon ? ExplorationController.EncounterPosition :
                run.Era == VisualEra.Primitive ? ExplorationController.MonumentPosition : ExplorationController.ExitPosition;
            var delta = target - position;
            var horizontal = delta.x > 1 ? "LESTE" : delta.x < -1 ? "OESTE" : "";
            var vertical = delta.y > 1 ? "NORTE" : delta.y < -1 ? "SUL" : "";
            _direction.text = run.Completed ? "Use Reiniciar percurso para testar novamente. Progresso apenas nesta execucao." :
                $"OBJETIVO: {vertical} {horizontal}   /   DISTANCIA: {delta.magnitude:0.0} m";
            _prompt.text = prompt;
        }

        private static RectTransform Rect(Transform parent, string name, float x, float y, float width, float height)
        {
            var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            rect.SetParent(parent, false); rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0, 1);
            rect.anchoredPosition = new Vector2(x, -y); rect.sizeDelta = new Vector2(width, height);
            return rect;
        }

        private static RectTransform Panel(Transform parent, string name, float x, float y, float width, float height)
        {
            var rect = Rect(parent, name, x, y, width, height);
            var image = rect.gameObject.AddComponent<Image>(); image.color = new Color32(18, 30, 28, 240); image.raycastTarget = false;
            return rect;
        }

        private Text Label(Transform parent, float x, float y, float width, float height, int size, Color color)
        {
            var text = Rect(parent, "Label", x, y, width, height).gameObject.AddComponent<Text>();
            text.font = _font; text.fontSize = size; text.color = color; text.raycastTarget = false;
            return text;
        }
    }
}
