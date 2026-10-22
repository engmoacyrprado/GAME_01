using UnityEngine;

namespace Game01.Adventure
{
    public sealed class ExplorationWorld
    {
        private readonly Transform _parent;
        private Transform _root;
        public ExplorationWorld(Transform parent) { _parent = parent; }

        public void Rebuild(AdventureRun run)
        {
            if (_root != null) Object.Destroy(_root.gameObject);
            _root = new GameObject("ExplorationWorld").transform;
            _root.SetParent(_parent, false);
            var retro = run.Era == VisualEra.Retro;
            var grass = retro ? new Color32(50, 83, 62, 255) : new Color32(31, 42, 37, 255);
            var road = retro ? new Color32(126, 107, 75, 255) : new Color32(74, 76, 57, 255);
            Box("Ground", Vector2.zero, new Vector2(38, 22), grass, -100);
            for (var x = -18; x <= 18; x += 2)
                for (var y = -10; y <= 10; y += 2)
                {
                    if (retro)
                    {
                        var shade = (x + y) % 4 == 0 ? new Color32(57, 90, 64, 255) : new Color32(44, 76, 57, 255);
                        Box("GrassTile", new Vector2(x, y), new Vector2(1.94f, 1.94f), shade, -90);
                        Box("GrassDetail", new Vector2(x + 0.5f, y + 0.5f), new Vector2(0.25f, 0.08f), new Color32(85, 120, 69, 255), -80);
                    }
                    else Box("Grid", new Vector2(x, y), new Vector2(0.1f, 0.1f), new Color32(73, 88, 66, 255), -90);
                }
            Box("RoadWest", new Vector2(-6, -4), new Vector2(13, 1.6f), road, -60);
            Box("RoadCenter", new Vector2(0, -2), new Vector2(1.6f, 5), road, -60);
            Box("RoadEast", new Vector2(5, 0), new Vector2(11, 1.6f), road, -60);
            Box("RoadNorth", new Vector2(10, 2), new Vector2(1.6f, 5), road, -60);
            Box("RoadExit", new Vector2(12, 4.5f), new Vector2(5, 1.6f), road, -60);
            var boundary = new Color32(21, 29, 27, 255);
            Box("WallWest", new Vector2(-18.5f, 0), new Vector2(1, 22), boundary, 0, true);
            Box("WallEast", new Vector2(18.5f, 0), new Vector2(1, 22), boundary, 0, true);
            Box("WallNorth", new Vector2(0, 10.5f), new Vector2(38, 1), boundary, 0, true);
            Box("WallSouth", new Vector2(0, -10.5f), new Vector2(38, 1), boundary, 0, true);
            Box("Water", new Vector2(-12, 5), new Vector2(7, 3), retro ? new Color32(50, 111, 127, 255) : new Color32(37, 58, 62, 255), -50, true);
            foreach (var position in new[] { new Vector2(-7, -1), new Vector2(-6, 2), new Vector2(-3, -6),
                new Vector2(4, -3), new Vector2(6, 2), new Vector2(11, -2), new Vector2(-13, 1), new Vector2(15, 1),
                new Vector2(3, 6), new Vector2(0, 7), new Vector2(-10, -7), new Vector2(14, -6) })
            {
                var order = 1000 - Mathf.RoundToInt(position.y * 10);
                Box("TreeBase", position, new Vector2(0.8f, 0.65f), new Color32(97, 78, 51, 255), order, true);
                Box("TreeTrunk", position + Vector2.up * 0.5f, new Vector2(0.25f, 1.2f), new Color32(98, 79, 51, 255), order);
                Box("TreeCrown", position + Vector2.up * 1.2f, new Vector2(1.4f, 1.1f),
                    retro ? new Color32(90, 133, 72, 255) : new Color32(79, 102, 69, 255), order);
                if (retro) Box("TreeLight", position + new Vector2(-0.25f, 1.5f), new Vector2(0.8f, 0.5f), new Color32(130, 164, 89, 255), order + 1);
            }
            if (!run.EncounterWon)
            {
                Actor("EncounterWolf", "wolf", new Vector2(-0.55f, 0), run.Era);
                Actor("EncounterBandit", "bandit", new Vector2(0.55f, 0), run.Era);
                Box("EncounterMarker", new Vector2(0, -0.8f), new Vector2(1.8f, 0.15f), new Color32(213, 134, 71, 255), 10);
            }
            else Box("ClearedCamp", new Vector2(0, -0.3f), new Vector2(1.5f, 0.2f), new Color32(139, 182, 121, 255), 10);
            var gold = run.EncounterWon ? new Color32(235, 191, 86, 255) : new Color32(111, 103, 80, 255);
            Box("MonumentBase", ExplorationController.MonumentPosition, new Vector2(1.5f, 0.7f), gold, 900);
            Box("Monument", ExplorationController.MonumentPosition + Vector2.up * 0.7f, new Vector2(0.4f, 1.5f), gold, 901);
            Box("MonumentCrown", ExplorationController.MonumentPosition + Vector2.up * 1.4f, new Vector2(0.85f, 0.25f), gold, 902);
            if (retro) Box("EraLight", ExplorationController.MonumentPosition + Vector2.up * 1.8f, new Vector2(0.35f, 0.35f), new Color32(157, 224, 197, 255), 903);
            var exit = ExplorationController.ExitPosition;
            var exitColor = retro ? new Color32(117, 193, 177, 255) : new Color32(71, 85, 78, 255);
            Box("ExitLeft", exit + Vector2.left * 0.65f, new Vector2(0.2f, 1.8f), exitColor, 900);
            Box("ExitRight", exit + Vector2.right * 0.65f, new Vector2(0.2f, 1.8f), exitColor, 900);
            Box("ExitLintel", exit + Vector2.up * 0.9f, new Vector2(1.5f, 0.2f), exitColor, 900);
        }

        private GameObject Box(string name, Vector2 point, Vector2 size, Color color, int order, bool solid = false) =>
            WorldPrimitives.Box(_root, name, point, size, color, order, solid);

        private void Actor(string name, string role, Vector2 point, VisualEra era)
        {
            var actor = new GameObject(name);
            actor.transform.SetParent(_root, false);
            actor.transform.localPosition = point;
            actor.AddComponent<WorldActorView>().Configure(role, era);
        }
    }
}
