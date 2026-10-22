using Game01.Adventure;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game01.Narrative
{
    public sealed class NarrativePresenter : MonoBehaviour
    {
        public DialogueSession Session { get; private set; }
        public bool BlocksExploration => _title.activeSelf || (Session != null && Session.IsActive);
        public bool InputConsumedThisFrame => _consumedFrame == Time.frameCount;
        private DialogueGraph _graph;
        private AdventureRun _run;
        private Transform _hero, _mage;
        private RectTransform _canvas, _bubble;
        private GameObject _title, _dialogue;
        private Text _speaker, _body, _hint;
        private Button _continue;
        private readonly Button[] _choices = new Button[3];
        private readonly Text[] _choiceLabels = new Text[3];
        private Font _font;
        private int _consumedFrame = -1;
        private bool _isIntroduction;

        public void Configure(DialogueGraph graph, AdventureRun run, Transform hero, Transform mage)
        {
            _graph = graph; _run = run; _hero = hero; _mage = mage;
            _font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            var root = new GameObject("NarrativeCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            root.transform.SetParent(transform, false);
            _canvas = root.GetComponent<RectTransform>();
            root.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            root.GetComponent<Canvas>().sortingOrder = 8000;
            var scaler = root.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280, 720); scaler.matchWidthOrHeight = 0.5f;
            BuildTitle(); BuildDialogue();
            _dialogue.SetActive(false); _bubble.gameObject.SetActive(false);
            _title.SetActive(!run.IntroductionSeen);
        }

        public void BeginIntroduction()
        {
            _consumedFrame = Time.frameCount;
            _title.SetActive(false);
            Begin(true);
        }

        public void BeginReminder()
        {
            if (BlocksExploration) return;
            Begin(false);
        }

        private void Begin(bool introduction)
        {
            _consumedFrame = Time.frameCount;
            _isIntroduction = introduction;
            Session = new DialogueSession(_graph, introduction ? null : "reminder");
            Session.ChoiceMade += _run.RememberResponse;
            _dialogue.SetActive(true); Refresh();
        }

        public bool Advance()
        {
            if (Session == null || !Session.Advance()) return false;
            _consumedFrame = Time.frameCount;
            Refresh(); return true;
        }

        public bool Choose(int index)
        {
            if (Session == null || !Session.Choose(index)) return false;
            _consumedFrame = Time.frameCount;
            Refresh(); return true;
        }

        private void Update()
        {
            if (InputConsumedThisFrame) return;
            if (_title.activeSelf)
            {
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space)) BeginIntroduction();
                return;
            }
            if (Session == null || !Session.IsActive) return;
            if (Session.Current.choices.Count == 0)
            {
                if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)) Advance();
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1)) Choose(0);
                else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2)) Choose(1);
                else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3)) Choose(2);
            }
        }

        private void LateUpdate()
        {
            if (Session == null || !Session.IsActive || Session.Current.speaker == DialogueSpeaker.Narrator)
            { _bubble.gameObject.SetActive(false); return; }
            var anchor = Session.Current.speaker == DialogueSpeaker.Hero ? _hero : _mage;
            var screen = Camera.main.WorldToScreenPoint(anchor.position + Vector3.up * 1.1f);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas, screen, null, out var point);
            point.x = Mathf.Clamp(point.x, -_canvas.rect.width / 2 + 50, _canvas.rect.width / 2 - 50);
            point.y = Mathf.Clamp(point.y, -_canvas.rect.height / 2 + 310, _canvas.rect.height / 2 - 45);
            _bubble.anchoredPosition = point;
            _bubble.gameObject.SetActive(screen.z > 0);
        }

        private void Refresh()
        {
            if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);
            if (!Session.IsActive)
            {
                if (_isIntroduction) _run.IntroductionSeen = true;
                _dialogue.SetActive(false); _bubble.gameObject.SetActive(false);
                return;
            }
            var node = Session.Current;
            _speaker.text = node.speaker == DialogueSpeaker.Hero ? "HEROI" : node.speaker == DialogueSpeaker.Mage ? "MAGO" : "NARRADOR";
            _speaker.color = node.speaker == DialogueSpeaker.Hero ? new Color32(156, 200, 130, 255) :
                node.speaker == DialogueSpeaker.Mage ? new Color32(106, 194, 207, 255) : new Color32(235, 185, 80, 255);
            foreach (var image in _bubble.GetComponentsInChildren<Image>()) image.color = _speaker.color;
            _body.text = node.text;
            _continue.gameObject.SetActive(node.choices.Count == 0);
            for (var i = 0; i < _choices.Length; i++)
            {
                _choices[i].gameObject.SetActive(i < node.choices.Count);
                if (i < node.choices.Count)
                {
                    _choiceLabels[i].text = $"{i + 1}. {node.choices[i].label}";
                    var rect = _choices[i].GetComponent<RectTransform>();
                    var height = node.choices.Count <= 2 ? 36f : 27f;
                    rect.sizeDelta = new Vector2(1186, height);
                    rect.anchoredPosition = new Vector2(20, -(126 + i * (height + (node.choices.Count <= 2 ? 6 : 1))));
                    _choiceLabels[i].rectTransform.sizeDelta = new Vector2(1166, height - 4);
                }
            }
            _hint.text = node.choices.Count == 0 ? "E, Espaco ou Enter para continuar. Sem limite de tempo." : "Clique na resposta ou pressione 1, 2 ou 3. Sem limite de tempo.";
        }

        private void BuildTitle()
        {
            var title = FullPanel(_canvas, "ProjectOpening", new Color32(15, 28, 27, 255));
            _title = title.gameObject;
            Label(title, "CONCEITO JOGAVEL / ROTEIRO PROVISORIO", 100, 62, 1050, 32, 16, new Color32(151, 183, 157, 255));
            Label(title, "GAME_01", 94, 125, 1080, 100, 76, new Color32(235, 185, 80, 255));
            Label(title, "Um mundo que ganha forma\na cada descoberta.", 100, 252, 1080, 110, 36, Color.white);
            Label(title, "Explore um cenario simples, planeje as acoes dos aliados\ne descubra a primeira mudanca visual do mundo.", 100, 397, 1050, 80, 23, new Color32(197, 213, 194, 255));
            Button(title, "Comecar jornada", 100, 542, 370, 60, out _).onClick.AddListener(BeginIntroduction);
            Label(title, "PC primeiro. Historia e arte ainda em desenvolvimento.", 100, 637, 1060, 32, 16, new Color32(151, 183, 157, 255));
        }

        private void BuildDialogue()
        {
            var shade = FullPanel(_canvas, "DialogueOverlay", new Color(0, 0, 0, 0.08f));
            _dialogue = shade.gameObject;
            var box = Rect(shade, "SubtitlePanel", 24, 462, 1232, 238);
            box.anchorMin = new Vector2(0, 0); box.anchorMax = new Vector2(1, 0); box.pivot = new Vector2(0.5f, 0);
            box.offsetMin = new Vector2(24, 20); box.offsetMax = new Vector2(-24, 258);
            var image = box.gameObject.AddComponent<Image>(); image.color = new Color32(16, 29, 29, 250);
            _speaker = Label(box, "", 20, 12, 1160, 27, 19, new Color32(235, 185, 80, 255));
            _body = Label(box, "", 20, 47, 1160, 78, 23, new Color32(237, 239, 222, 255));
            _hint = Label(box, "", 20, 214, 1160, 22, 13, new Color32(152, 179, 158, 255));
            _continue = Button(box, "Continuar [E]", 936, 167, 270, 40, out _);
            _continue.onClick.AddListener(() => Advance());
            for (var i = 0; i < _choices.Length; i++)
            {
                var index = i;
                _choices[i] = Button(box, "", 20, 126 + i * 28, 1186, 27, out _choiceLabels[i], 16);
                _choices[i].onClick.AddListener(() => Choose(index));
            }
            _bubble = Rect(_canvas, "SpeakerBubble", 0, 0, 64, 40);
            _bubble.anchorMin = _bubble.anchorMax = _bubble.pivot = new Vector2(0.5f, 0.5f);
            _bubble.gameObject.AddComponent<Image>().color = new Color32(237, 213, 150, 255);
            var tail = Rect(_bubble, "BubbleTail", 24, 33, 12, 12);
            tail.gameObject.AddComponent<Image>().color = new Color32(237, 213, 150, 255);
            tail.localRotation = Quaternion.Euler(0, 0, 45);
            Label(_bubble, "...", 17, -2, 50, 40, 27, new Color32(22, 38, 34, 255));
        }

        private static RectTransform Rect(Transform parent, string name, float x, float y, float width, float height)
        {
            var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            rect.SetParent(parent, false); rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0, 1);
            rect.anchoredPosition = new Vector2(x, -y); rect.sizeDelta = new Vector2(width, height);
            return rect;
        }

        private static RectTransform FullPanel(Transform parent, string name, Color color)
        {
            var panel = Rect(parent, name, 0, 0, 1280, 720);
            panel.anchorMin = Vector2.zero; panel.anchorMax = Vector2.one;
            panel.offsetMin = panel.offsetMax = Vector2.zero;
            panel.gameObject.AddComponent<Image>().color = color;
            return panel;
        }

        private Text Label(Transform parent, string value, float x, float y, float width, float height, int size, Color color)
        {
            var text = Rect(parent, "Label", x, y, width, height).gameObject.AddComponent<Text>();
            text.font = _font; text.fontSize = size; text.color = color; text.text = value; text.raycastTarget = false;
            return text;
        }

        private Button Button(Transform parent, string text, float x, float y, float width, float height, out Text label, int size = 18)
        {
            var rect = Rect(parent, "DialogueButton", x, y, width, height);
            var image = rect.gameObject.AddComponent<Image>(); image.color = new Color32(47, 72, 61, 255);
            var button = rect.gameObject.AddComponent<Button>(); button.targetGraphic = image;
            label = Label(rect, text, 10, 3, width - 20, height - 4, size, Color.white);
            label.alignment = TextAnchor.MiddleLeft;
            return button;
        }
    }
}
