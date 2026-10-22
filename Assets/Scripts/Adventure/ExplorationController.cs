using System;
using Game01.Combat.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game01.Adventure
{
    public sealed class ExplorationController : MonoBehaviour
    {
        public static readonly Vector2 EncounterPosition = Vector2.zero;
        public static readonly Vector2 MonumentPosition = new Vector2(10, 4);
        public static readonly Vector2 ExitPosition = new Vector2(14, 5);
        public const float MoveSpeed = 4f;
        public Rigidbody2D Body { get; private set; }
        public Func<Vector2> MovementSource { get; set; }
        private ExplorationWorld _world;
        private ExplorationHud _hud;
        private WorldActorView _actor;
        private Camera _camera;
        private bool _loading;

        private void Start()
        {
            var run = AdventureRuntime.Run;
            _camera = Camera.main;
            _world = new ExplorationWorld(transform);
            _world.Rebuild(run);
            var player = new GameObject("ExplorationHero");
            player.transform.SetParent(transform, false);
            player.transform.position = run.PlayerPosition;
            Body = player.AddComponent<Rigidbody2D>();
            Body.gravityScale = 0;
            Body.constraints = RigidbodyConstraints2D.FreezeRotation;
            Body.interpolation = RigidbodyInterpolation2D.Interpolate;
            Body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            var collider = player.AddComponent<CircleCollider2D>(); collider.radius = 0.28f;
            _actor = player.AddComponent<WorldActorView>(); _actor.Configure("hero", run.Era);
            _hud = new ExplorationHud(transform, RestartJourney);
            FollowCamera(true);
        }

        private void FixedUpdate()
        {
            if (Body == null || _loading) return;
            var input = MovementSource != null ? MovementSource() : new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            Body.linearVelocity = Vector2.ClampMagnitude(input, 1f) * MoveSpeed;
            _actor.Movement = Body.linearVelocity;
            AdventureRuntime.Run.PlayerPosition = Body.position;
        }

        private void Update()
        {
            if (Body == null || _loading) return;
            _hud.Refresh(AdventureRuntime.Run, Body.position, GetPrompt());
            if (Input.GetKeyDown(KeyCode.E)) TryInteract();
        }

        private void LateUpdate() { if (Body != null) FollowCamera(false); }

        private void FollowCamera(bool instant)
        {
            var halfWidth = _camera.orthographicSize * _camera.aspect;
            var target = new Vector3(Mathf.Clamp(Body.position.x, -18 + halfWidth, 18 - halfWidth),
                Mathf.Clamp(Body.position.y, -10 + _camera.orthographicSize, 10 - _camera.orthographicSize), -10);
            _camera.transform.position = instant ? target : Vector3.Lerp(_camera.transform.position, target, 8 * Time.deltaTime);
        }

        private bool Near(Vector2 target) => Vector2.Distance(Body.position, target) <= 1.8f;

        public string GetPrompt()
        {
            var run = AdventureRuntime.Run;
            if (run.Completed) return "Percurso concluido / continue explorando ou reinicie.";
            if (!run.EncounterWon && Near(EncounterPosition)) return "[E] Enfrentar o encontro de teste";
            if (Near(MonumentPosition)) return run.EncounterWon
                ? (run.Era == VisualEra.Primitive ? "[E] Ativar a proxima era visual" : "Marco ativado / procure a saida a leste")
                : "Marco bloqueado / venca o encontro primeiro";
            if (Near(ExitPosition)) return run.Era == VisualEra.Retro ? "[E] Concluir este percurso" : "Ative o marco visual antes de sair";
            return "WASD ou setas: mover    /    E: interagir quando estiver perto";
        }

        public bool TryInteract()
        {
            if (_loading || Body == null) return false;
            var run = AdventureRuntime.Run;
            if (!run.EncounterWon && Near(EncounterPosition) && run.TryBeginEncounter())
            {
                run.PlayerPosition = Body.position;
                Body.linearVelocity = Vector2.zero;
                _loading = true;
                SceneManager.LoadScene(AdventureRuntime.BattleScene);
                return true;
            }
            if (Near(MonumentPosition) && run.TryAdvanceEra())
            {
                _world.Rebuild(run);
                _actor.Configure("hero", run.Era);
                return true;
            }
            if (Near(ExitPosition)) return run.TryComplete();
            return false;
        }

        public void RestartJourney()
        {
            if (_loading) return;
            _loading = true;
            AdventureRuntime.Restart();
            SceneManager.LoadScene(AdventureRuntime.MapScene);
        }
    }
}
