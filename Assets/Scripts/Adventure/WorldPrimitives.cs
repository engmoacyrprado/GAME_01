using UnityEngine;

namespace Game01.Adventure
{
    public static class WorldPrimitives
    {
        private static Sprite _square;

        public static GameObject Box(Transform parent, string name, Vector2 position, Vector2 size, Color color, int order = 0, bool solid = false)
        {
            if (_square == null)
                _square = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, Texture2D.whiteTexture.width, Texture2D.whiteTexture.height),
                    new Vector2(0.5f, 0.5f), Texture2D.whiteTexture.width);
            var item = new GameObject(name, typeof(SpriteRenderer));
            item.transform.SetParent(parent, false);
            item.transform.localPosition = position;
            item.transform.localScale = size;
            var renderer = item.GetComponent<SpriteRenderer>();
            renderer.sprite = _square;
            renderer.color = color;
            renderer.sortingOrder = order;
            if (solid) item.AddComponent<BoxCollider2D>();
            return item;
        }

        public static void Line(Transform parent, string name, Vector2 start, Vector2 end, Color color, int order = 1000, float width = 0.07f)
        {
            var difference = end - start;
            var item = Box(parent, name, (start + end) * 0.5f, new Vector2(difference.magnitude, width), color, order);
            item.transform.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg);
        }
    }
}
