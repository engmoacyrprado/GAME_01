using System.Collections.Generic;
using System.IO;
using Game01.Combat.Data;
using Game01.Combat.Debug;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game01.Combat.Editor
{
    public static class CombatPrototypeSceneGenerator
    {
        public const string ScenePath = "Assets/Scenes/BattlePrototype.unity";

        [MenuItem("GAME_01/Combat/Open Playable Prototype")]
        public static void Generate()
        {
            if (Application.isPlaying) return;
            if (!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            if (!File.Exists(ScenePath))
            {
                CombatPrototypeAssetGenerator.GeneratePrototypeAssets();
                if (!AssetDatabase.IsValidFolder("Assets/Scenes")) AssetDatabase.CreateFolder("Assets", "Scenes");
                var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                var camera = new GameObject("Main Camera", typeof(Camera));
                camera.tag = "MainCamera";
                camera.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
                camera.GetComponent<Camera>().backgroundColor = new Color32(20, 31, 33, 255);
                var bootstrap = new GameObject("BattleBootstrap").AddComponent<BattleDebugBootstrap>();
                bootstrap.Configure(Load<BattleConfig>("Configs/BattleConfig_Prototype"),
                    new List<CombatantDefinition> { Load<CombatantDefinition>("Combatants/Combatant_Hero"), Load<CombatantDefinition>("Combatants/Combatant_Mage") },
                    new List<CombatantDefinition> { Load<CombatantDefinition>("Combatants/Combatant_Wolf"), Load<CombatantDefinition>("Combatants/Combatant_Bandit") });
                EditorSceneManager.SaveScene(scene, ScenePath);
            }
            else EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

            var scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            if (!scenes.Exists(scene => scene.path == ScenePath))
            {
                scenes.Insert(0, new EditorBuildSettingsScene(ScenePath, true));
                EditorBuildSettings.scenes = scenes.ToArray();
            }
            AssetDatabase.SaveAssets();
            UnityEngine.Debug.Log("GAME_01: playable prototype ready at " + ScenePath);
        }

        private static T Load<T>(string name) where T : Object
        {
            return AssetDatabase.LoadAssetAtPath<T>("Assets/GameData/Combat/" + name + ".asset");
        }

        [MenuItem("GAME_01/Combat/Build PC Prototype")]
        public static void BuildPc()
        {
            CombatValidation.Run();
            Generate();
            if (!File.Exists(ScenePath)) throw new System.InvalidOperationException("Prototype scene is missing.");
            PlayerSettings.productName = "GAME_01";
            PlayerSettings.defaultScreenWidth = 1280;
            PlayerSettings.defaultScreenHeight = 720;
            PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
            PlayerSettings.resizableWindow = true;
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = new[] { ScenePath },
                locationPathName = "Builds/PC/GAME_01.exe",
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.None
            });
            if (report.summary.result != BuildResult.Succeeded)
                throw new System.InvalidOperationException("PC build failed: " + report.summary.result);
            UnityEngine.Debug.Log("GAME_01 BUILD: PASS Builds/PC/GAME_01.exe");
        }
    }
}
