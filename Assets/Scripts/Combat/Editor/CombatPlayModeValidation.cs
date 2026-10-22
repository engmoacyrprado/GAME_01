using System;
using System.IO;
using Game01.Combat.Core;
using Game01.Combat.Debug;
using Game01.Combat.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Game01.Combat.Editor
{
    [InitializeOnLoad]
    public static class CombatPlayModeValidation
    {
        private const string Running = "GAME_01.PlayValidation";
        private static double _started;
        private static int _stage;
        private static BattleSession _oldSession;
        private static float _targetHp;
        private static CombatantRuntime _target;
        private static string _loggedFailure;
        private static bool _startedBattle;
        private static float _pausedTimer;
        private static float _pausedAtb;
        private static float _resumeAt;

        static CombatPlayModeValidation()
        {
            if (SessionState.GetBool(Running, false))
            {
                _started = EditorApplication.timeSinceStartup;
                EditorApplication.update += Update;
                Application.logMessageReceived += CheckLog;
            }
        }

        public static void Run()
        {
            CombatPrototypeSceneGenerator.Generate();
            // Let Editor startup callbacks create the search index before entering Play.
            var readyAt = EditorApplication.timeSinceStartup + 2;
            EditorApplication.CallbackFunction begin = null;
            begin = () =>
            {
                if (EditorApplication.timeSinceStartup < readyAt || EditorApplication.isUpdating) return;
                EditorApplication.update -= begin;
                SessionState.SetBool(Running, true);
                EditorApplication.isPlaying = true;
            };
            EditorApplication.update += begin;
        }

        private static void CheckLog(string message, string stack, LogType type)
        {
            if (type == LogType.Exception || type == LogType.Error || type == LogType.Assert) _loggedFailure = message + "\n" + stack;
        }

        private static void Update()
        {
            if (!EditorApplication.isPlaying || EditorApplication.isCompiling) return;
            try
            {
                if (_loggedFailure != null) throw new Exception(_loggedFailure);
                if (EditorApplication.timeSinceStartup - _started > 40) throw new Exception("Play Mode validation timed out.");
                var bootstrap = Object.FindFirstObjectByType<BattleDebugBootstrap>();
                if (bootstrap == null || bootstrap.Session == null) return;
                var context = bootstrap.Session.Context;
                var buttons = Object.FindObjectsByType<Button>(FindObjectsSortMode.None);
                if (!_startedBattle)
                {
                    if (!bootstrap.TestPaused) throw new Exception("Battle started before the player was ready.");
                    FindButton(buttons, "Heroi").onClick.Invoke();
                    buttons = Object.FindObjectsByType<Button>(FindObjectsSortMode.None);
                    FindButton(buttons, "Golpe Pesado").onClick.Invoke();
                    FindButton(buttons, "Lobo").onClick.Invoke();
                    FindButton(buttons, "Mago").onClick.Invoke();
                    FindButton(buttons, "Faisca").onClick.Invoke();
                    FindButton(buttons, "Bandido").onClick.Invoke();
                    if (context.GetPlannedAction(context.Allies[0]) == null || context.GetPlannedAction(context.Allies[1]) == null
                        || context.Enemies[0].CurrentHP != 75 || context.Allies[1].CurrentMP != 40)
                        throw new Exception("Pre-turn UI plans were not stored independently or executed too early.");
                    FindButton(buttons, "Iniciar teste").onClick.Invoke();
                    _startedBattle = true;
                    return;
                }
                if (_stage == 0 && context.CurrentPhase == CombatPhase.DecisionPhase)
                {
                    if (context.GetPlannedAction(context.Allies[0]) != null || context.GetPlannedAction(context.Allies[1]) != null
                        || context.Enemies[0].CurrentHP >= 75 || context.Enemies[1].CurrentHP >= 95 || context.Allies[1].CurrentMP != 32)
                        throw new Exception("Prepared actions failed to execute at their own turns.");
                    _pausedTimer = context.DecisionTimerRemaining;
                    _pausedAtb = context.Enemies[0].CurrentATB;
                    FindButton(buttons, "Pausar teste").onClick.Invoke();
                    _resumeAt = Time.realtimeSinceStartup + 0.25f;
                    _stage = 10;
                    return;
                }
                if (_stage == 10)
                {
                    if (Time.realtimeSinceStartup < _resumeAt) return;
                    if (!bootstrap.TestPaused || context.DecisionTimerRemaining != _pausedTimer || context.Enemies[0].CurrentATB != _pausedAtb)
                        throw new Exception("Test pause failed to freeze the timer and ATB.");
                    FindButton(buttons, "Continuar teste").onClick.Invoke();
                    _stage = 11;
                    return;
                }
                if (_stage == 11 && context.CurrentPhase == CombatPhase.DecisionPhase)
                {
                    if (Object.FindFirstObjectByType<Canvas>() == null) throw new Exception("Missing combat canvas.");
                    Canvas.ForceUpdateCanvases();
                    CapturePreview();
                    _target = context.Enemies.Find(enemy => !enemy.IsDefeated);
                    _targetHp = _target.CurrentHP;
                    FindButton(buttons, "Ataque Basico").onClick.Invoke();
                    FindButton(buttons, _target.Definition.displayName).onClick.Invoke();
                    if (context.SelectedAction == null || context.SelectedTarget != _target) throw new Exception("UI selection did not reach combat rules.");
                    _stage = 1;
                }
                else if (_stage == 1 && context.SelectedAction == null)
                {
                    if (_target.CurrentHP >= _targetHp) throw new Exception("Selected attack caused no damage.");
                    _oldSession = bootstrap.Session;
                    FindButton(buttons, "Reiniciar batalha").onClick.Invoke();
                    _stage = 2;
                }
                else if (_stage == 2)
                {
                    if (bootstrap.Session == _oldSession || !bootstrap.TestPaused || context.Allies[0].CurrentHP != context.Allies[0].Definition.stats.maxHP)
                        throw new Exception("Restart failed to create a clean battle.");
                    Finish(true, "Pre-turn planning for both allies, execution, pause/resume, current action, damage and clean restart passed.");
                }
            }
            catch (Exception error) { Finish(false, error.Message); }
        }

        private static Button FindButton(Button[] buttons, string prefix)
        {
            foreach (var button in buttons)
                foreach (var label in button.GetComponentsInChildren<Text>())
                    if (label.text.StartsWith(prefix, StringComparison.Ordinal) && button.interactable) return button;
            throw new Exception("Missing enabled UI button: " + prefix);
        }

        private static void CapturePreview()
        {
            var canvas = Object.FindFirstObjectByType<Canvas>();
            var camera = Camera.main;
            var texture = new RenderTexture(1280, 720, 24);
            var pixels = new Texture2D(1280, 720, TextureFormat.RGB24, false);
            var previous = RenderTexture.active;
            try
            {
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = camera;
                canvas.planeDistance = 1f;
                camera.targetTexture = texture;
                Canvas.ForceUpdateCanvases();
                camera.Render();
                RenderTexture.active = texture;
                pixels.ReadPixels(new Rect(0, 0, 1280, 720), 0, 0);
                pixels.Apply();
                Directory.CreateDirectory("Logs");
                File.WriteAllBytes("Logs/prototype-review.png", pixels.EncodeToPNG());
            }
            finally
            {
                camera.targetTexture = null;
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                RenderTexture.active = previous;
                Object.DestroyImmediate(pixels);
                Object.DestroyImmediate(texture);
            }
        }

        private static void Finish(bool passed, string message)
        {
            SessionState.SetBool(Running, false);
            EditorApplication.update -= Update;
            Application.logMessageReceived -= CheckLog;
            UnityEngine.Debug.Log("GAME_01 PLAY VALIDATION: " + (passed ? "PASS " : "FAIL ") + message);
            EditorApplication.Exit(passed ? 0 : 1);
        }
    }
}
