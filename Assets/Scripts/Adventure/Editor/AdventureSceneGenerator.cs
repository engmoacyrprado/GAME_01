using System;
using System.Collections.Generic;
using System.IO;
using Game01.Combat.Editor;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Game01.Adventure.Editor
{
    public static class AdventureSceneGenerator
    {
        public const string ScenePath = "Assets/Scenes/ExplorationPrototype.unity";

        [MenuItem("GAME_01/Adventure/Open Exploration Prototype")]
        public static void Generate()
        {
            if (Application.isPlaying) return;
            if (!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            CombatPrototypeSceneGenerator.Generate();
            if (!File.Exists(ScenePath))
            {
                var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                var camera = new GameObject("Main Camera", typeof(Camera)); camera.tag = "MainCamera";
                var view = camera.GetComponent<Camera>();
                view.orthographic = true; view.orthographicSize = 6.2f;
                view.clearFlags = CameraClearFlags.SolidColor; view.backgroundColor = new Color32(18, 30, 28, 255);
                camera.transform.position = new Vector3(0, 0, -10);
                new GameObject("ExplorationBootstrap").AddComponent<ExplorationController>();
                EditorSceneManager.SaveScene(scene, ScenePath);
            }
            else EditorSceneManager.OpenScene(ScenePath);
            var scenes = new List<EditorBuildSettingsScene> { new EditorBuildSettingsScene(ScenePath, true),
                new EditorBuildSettingsScene(CombatPrototypeSceneGenerator.ScenePath, true) };
            foreach (var scene in EditorBuildSettings.scenes)
                if (!scenes.Exists(item => item.path == scene.path)) scenes.Add(scene);
            EditorBuildSettings.scenes = scenes.ToArray();
            AssetDatabase.SaveAssets();
            UnityEngine.Debug.Log("GAME_01 ADVENTURE: exploration scene ready.");
        }

        [MenuItem("GAME_01/Adventure/Build PC Vertical Slice")]
        public static void BuildPc()
        {
            CombatValidation.Run();
            AdventureValidation.Run();
            Generate();
            PlayerSettings.productName = "GAME_01";
            PlayerSettings.defaultScreenWidth = 1280; PlayerSettings.defaultScreenHeight = 720;
            PlayerSettings.fullScreenMode = FullScreenMode.Windowed; PlayerSettings.resizableWindow = true;
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = new[] { ScenePath, CombatPrototypeSceneGenerator.ScenePath },
                locationPathName = "Builds/Adventure/GAME_01.exe", target = BuildTarget.StandaloneWindows64
            });
            if (report.summary.result != BuildResult.Succeeded) throw new InvalidOperationException("Adventure build failed.");
            UnityEngine.Debug.Log("GAME_01 ADVENTURE BUILD: PASS Builds/Adventure/GAME_01.exe");
        }
    }
}
