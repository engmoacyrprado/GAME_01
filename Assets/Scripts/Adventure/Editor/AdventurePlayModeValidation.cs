using System;
using System.IO;
using System.Linq;
using Game01.Combat.Core;
using Game01.Combat.Debug;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Game01.Adventure.Editor
{
    [InitializeOnLoad]
    public static class AdventurePlayModeValidation
    {
        private const string Running = "GAME_01.AdventureValidation";
        private static int _stage;
        private static float _until;
        private static Vector2 _start;
        private static double _started;
        private static string _error;
        private static int _narrativeStage;
        private static Vector2 _introPosition;
        private static int _captureAfterFrame;

        static AdventurePlayModeValidation()
        {
            if (!SessionState.GetBool(Running, false)) return;
            _started = EditorApplication.timeSinceStartup;
            EditorApplication.update += Update;
            Application.logMessageReceived += OnLog;
        }

        public static void Run()
        {
            AdventureSceneGenerator.Generate();
            AdventureValidation.Run();
            var ready = EditorApplication.timeSinceStartup + 2;
            EditorApplication.CallbackFunction begin = null;
            begin = () =>
            {
                if (EditorApplication.timeSinceStartup < ready || EditorApplication.isUpdating) return;
                EditorApplication.update -= begin;
                SessionState.SetBool(Running, true);
                EditorApplication.isPlaying = true;
            };
            EditorApplication.update += begin;
        }

        private static void OnLog(string message, string stack, LogType type)
        {
            if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert) _error = message + "\n" + stack;
        }

        private static void Update()
        {
            if (!EditorApplication.isPlaying || EditorApplication.isCompiling) return;
            try
            {
                if (_error != null) throw new Exception(_error);
                if (EditorApplication.timeSinceStartup - _started > 55) throw new Exception("Adventure test timed out.");
                var exploration = Object.FindFirstObjectByType<ExplorationController>();
                var battle = Object.FindFirstObjectByType<BattleDebugBootstrap>();
                if (_stage == 0 && exploration != null && exploration.Narrative != null)
                {
                    var narrative = exploration.Narrative;
                    if (narrative.BlocksExploration)
                    {
                        if (_narrativeStage == 0)
                        {
                            _introPosition = exploration.Body.position;
                            exploration.MovementSource = () => Vector2.right;
                            Capture("Logs/story-opening.png");
                            narrative.BeginIntroduction();
                            _until = Time.realtimeSinceStartup + 0.25f;
                            _narrativeStage = 1;
                        }
                        else if (_narrativeStage == 1 && Time.realtimeSinceStartup >= _until)
                        {
                            Check(Vector2.Distance(exploration.Body.position, _introPosition) < 0.02f, "Player moved during dialogue.");
                            narrative.Advance(); _narrativeStage = 2; _captureAfterFrame = Time.frameCount + 2;
                        }
                        else if (_narrativeStage == 2 && Time.frameCount >= _captureAfterFrame)
                        {
                            Capture("Logs/story-dialogue.png");
                            narrative.Advance(); _narrativeStage = 3; _captureAfterFrame = Time.frameCount + 2;
                        }
                        else if (_narrativeStage == 3 && Time.frameCount >= _captureAfterFrame)
                        {
                            Capture("Logs/story-choices.png");
                            Check(narrative.Session.Current.choices.Count == 2 && !narrative.Advance(), "Response choice was skipped.");
                            narrative.Choose(0);
                            Check(AdventureRuntime.Run.Responses["question"] == "ask_world", "Response was not recorded.");
                            _narrativeStage = 4;
                        }
                        else if (_narrativeStage == 4) narrative.Advance();
                        return;
                    }
                    if (narrative.InputConsumedThisFrame) return;
                }
                if (_stage == 0 && exploration != null && exploration.Body != null)
                {
                    exploration.Body.position = new Vector2(-12, -5);
                    Check(!exploration.TryInteract(), "Interaction activated from too far away.");
                    _start = exploration.Body.position;
                    exploration.MovementSource = () => Vector2.right;
                    _until = Time.realtimeSinceStartup + 0.4f;
                    _stage = 1;
                }
                else if (_stage == 1 && Time.realtimeSinceStartup >= _until)
                {
                    Check(exploration.Body.position.x > _start.x + 0.5f, "Player movement failed.");
                    exploration.Body.position = new Vector2(-8.2f, -1);
                    _until = Time.realtimeSinceStartup + 0.7f;
                    _stage = 2;
                }
                else if (_stage == 2 && Time.realtimeSinceStartup >= _until)
                {
                    Check(exploration.Body.position.x < -7.6f, "Player crossed a solid tree.");
                    exploration.MovementSource = () => Vector2.zero;
                    exploration.Body.linearVelocity = Vector2.zero;
                    Capture("Logs/exploration-primitive.png");
                    exploration.Body.position = new Vector2(-1.2f, 0);
                    Check(exploration.TryInteract(), "Encounter interaction failed.");
                    _stage = 3;
                }
                else if (_stage == 3 && battle != null && battle.Session != null)
                {
                    Check(AdventureRuntime.Run.EncounterActive, "Battle lost the adventure context.");
                    var button = FindButton("Voltar ao mapa");
                    Check(!button.interactable, "Allowed returning before battle ended.");
                    var context = battle.Session.Context;
                    foreach (var enemy in context.Enemies) enemy.ApplyDamage(10000);
                    var actor = context.Allies[0];
                    actor.AddATB(context.Config.atbThreshold, context.Config.atbThreshold);
                    var pass = actor.Definition.availableActions.First(action => action.actionType == CombatActionType.PassTurn);
                    context.PlanPlayerAction(actor, pass, actor);
                    FindButton("Iniciar teste").onClick.Invoke();
                    _stage = 4;
                }
                else if (_stage == 4 && battle != null && battle.Session.Context.CurrentPhase == CombatPhase.BattleEnded)
                {
                    Check(battle.Session.Context.Winner == CombatTeam.Allies, "Victory result missing.");
                    var button = FindButton("Voltar ao mapa");
                    Check(button.interactable, "Return button did not unlock.");
                    button.onClick.Invoke();
                    _stage = 5;
                }
                else if (_stage == 5 && exploration != null && exploration.Body != null)
                {
                    Check(AdventureRuntime.Run.EncounterWon && !AdventureRuntime.Run.EncounterActive, "Map did not retain victory.");
                    Check(AdventureRuntime.Run.IntroductionSeen && !exploration.Narrative.BlocksExploration, "Introduction replayed after battle.");
                    Check(Vector2.Distance(exploration.Body.position, new Vector2(-1.2f, 0)) < 0.2f, "Map did not restore player position.");
                    Check(GameObject.Find("EncounterWolf") == null, "Cleared enemy respawned.");
                    exploration.Body.position = new Vector2(9, 4);
                    Check(exploration.TryInteract() && AdventureRuntime.Run.Era == VisualEra.Retro, "Era transition failed.");
                    _until = Time.realtimeSinceStartup + 0.5f;
                    _stage = 6;
                }
                else if (_stage == 6 && Time.realtimeSinceStartup >= _until)
                {
                    Check(exploration.Body.GetComponentsInChildren<SpriteRenderer>().Any(sprite => sprite.name == "Tunic"), "Era transition did not update the hero.");
                    Capture("Logs/exploration-retro.png");
                    exploration.Body.position = new Vector2(13, 5);
                    Check(exploration.TryInteract() && AdventureRuntime.Run.Completed, "Demo completion failed.");
                    Finish(true, "Movement, obstacle collision, interaction range, battle handoff, return UI, victory, position, visual era and completion passed.");
                }
            }
            catch (Exception error) { Finish(false, error.Message); }
        }

        private static Button FindButton(string prefix)
        {
            foreach (var button in Object.FindObjectsByType<Button>(FindObjectsSortMode.None))
                if (button.GetComponentsInChildren<Text>().Any(text => text.text.StartsWith(prefix, StringComparison.Ordinal))) return button;
            throw new Exception("Missing button: " + prefix);
        }

        private static void Check(bool condition, string message)
        {
            if (!condition) throw new Exception(message);
        }

        private static void Capture(string path)
        {
            var camera = Camera.main;
            var canvases = Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None);
            var modes = canvases.Select(canvas => canvas.renderMode).ToArray();
            var texture = new RenderTexture(1280, 720, 24);
            var pixels = new Texture2D(1280, 720, TextureFormat.RGB24, false);
            var previous = RenderTexture.active;
            try
            {
                foreach (var canvas in canvases)
                {
                    canvas.renderMode = RenderMode.ScreenSpaceCamera; canvas.worldCamera = camera; canvas.planeDistance = 1;
                }
                camera.targetTexture = texture;
                Canvas.ForceUpdateCanvases(); camera.Render(); RenderTexture.active = texture;
                pixels.ReadPixels(new Rect(0, 0, 1280, 720), 0, 0); pixels.Apply();
                Directory.CreateDirectory("Logs"); File.WriteAllBytes(path, pixels.EncodeToPNG());
            }
            finally
            {
                camera.targetTexture = null;
                for (var i = 0; i < canvases.Length; i++) canvases[i].renderMode = modes[i];
                RenderTexture.active = previous; Object.DestroyImmediate(pixels); Object.DestroyImmediate(texture);
            }
        }

        private static void Finish(bool success, string message)
        {
            SessionState.SetBool(Running, false);
            EditorApplication.update -= Update; Application.logMessageReceived -= OnLog;
            UnityEngine.Debug.Log("GAME_01 ADVENTURE PLAY: " + (success ? "PASS " : "FAIL ") + message);
            EditorApplication.Exit(success ? 0 : 1);
        }
    }
}
