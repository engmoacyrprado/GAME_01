using UnityEngine;

namespace Game01.Adventure
{
    public sealed class WorldActorView : MonoBehaviour
    {
        private Transform _figure;
        private Vector3 _origin;
        private string _role;
        private VisualEra _era;
        public Vector2 Movement { get; set; }

        public void Configure(string role, VisualEra era)
        {
            _role = role; _era = era;
            if (_figure != null) Destroy(_figure.gameObject);
            _figure = new GameObject("Figure").transform;
            _figure.SetParent(transform, false);
            _origin = Vector3.zero;
            Color color = role == "mage" ? new Color32(96, 192, 203, 255) : role == "wolf" ? new Color32(224, 173, 78, 255) :
                role == "bandit" ? new Color32(214, 125, 86, 255) : new Color32(156, 200, 130, 255);
            if (role == "wolf")
            {
                WorldPrimitives.Box(_figure, "Head", new Vector2(0.3f, 0.16f), new Vector2(0.3f, 0.25f), color);
                WorldPrimitives.Line(_figure, "Body", new Vector2(-0.32f, 0), new Vector2(0.3f, 0), color);
                WorldPrimitives.Line(_figure, "FrontLeg", new Vector2(0.2f, 0), new Vector2(0.3f, -0.4f), color);
                WorldPrimitives.Line(_figure, "BackLeg", new Vector2(-0.2f, 0), new Vector2(-0.3f, -0.4f), color);
                return;
            }
            WorldPrimitives.Box(_figure, "Head", new Vector2(0, 0.45f), new Vector2(0.27f, 0.27f), color);
            WorldPrimitives.Line(_figure, "Body", new Vector2(0, 0.3f), new Vector2(0, -0.22f), color);
            WorldPrimitives.Line(_figure, "Arms", new Vector2(-0.3f, 0), new Vector2(0.3f, 0.2f), color);
            WorldPrimitives.Line(_figure, "LeftLeg", new Vector2(0, -0.2f), new Vector2(-0.24f, -0.55f), color);
            WorldPrimitives.Line(_figure, "RightLeg", new Vector2(0, -0.2f), new Vector2(0.24f, -0.55f), color);
            if (role == "mage")
            {
                WorldPrimitives.Line(_figure, "Staff", new Vector2(0.35f, -0.5f), new Vector2(0.35f, 0.6f), color);
                WorldPrimitives.Box(_figure, "Hat", new Vector2(0, 0.65f), new Vector2(0.4f, 0.12f), color);
            }
            else WorldPrimitives.Line(_figure, "Blade", new Vector2(0.3f, 0.2f), new Vector2(0.5f, 0.6f), Color.white);
            if (era == VisualEra.Retro)
            {
                WorldPrimitives.Box(_figure, "Tunic", new Vector2(0, 0.05f), new Vector2(0.25f, 0.35f), color);
                WorldPrimitives.Box(_figure, "Face", new Vector2(0.07f, 0.47f), new Vector2(0.045f, 0.04f), Color.black);
            }
        }

        private void Update()
        {
            if (_figure == null) return;
            var moving = Movement.sqrMagnitude > 0.01f;
            var phase = Time.time * (moving ? 10 : 2) + (_role == "mage" ? 1 : 0);
            _figure.localPosition = _origin + Vector3.up * Mathf.Sin(phase) * (moving ? 0.06f : 0.02f);
            _figure.localRotation = Quaternion.Euler(0, 0, moving ? Mathf.Sin(phase) * 3 : 0);
            var order = 1000 - Mathf.RoundToInt(transform.position.y * 10);
            foreach (var renderer in _figure.GetComponentsInChildren<SpriteRenderer>()) renderer.sortingOrder = order;
        }
    }
}
