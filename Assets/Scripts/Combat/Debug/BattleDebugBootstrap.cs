using System.Collections.Generic;
using Game01.Combat.Data;
using Game01.Combat.Runtime;
using Game01.Combat.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Game01.Adventure;
using Game01.Combat.Core;

namespace Game01.Combat.Debug
{
    public class BattleDebugBootstrap : MonoBehaviour
    {
        [SerializeField] private BattleConfig battleConfig;
        [SerializeField] private List<CombatantDefinition> allies = new List<CombatantDefinition>();
        [SerializeField] private List<CombatantDefinition> enemies = new List<CombatantDefinition>();
        [SerializeField] private BattleHUDController battleHudController;
        public BattleSession Session { get; private set; }
        public bool TestPaused { get; private set; } = true;
        private bool _testStarted;

        public void Configure(BattleConfig config, List<CombatantDefinition> allyDefinitions, List<CombatantDefinition> enemyDefinitions)
        {
            battleConfig = config;
            allies = allyDefinitions;
            enemies = enemyDefinitions;
        }

        private void Start()
        {
            if (battleHudController == null) battleHudController = gameObject.AddComponent<BattleHUDController>();
            battleHudController.RestartRequested += RestartBattle;
            battleHudController.PauseRequested += ToggleTestPause;
            battleHudController.ReturnToMapRequested += ReturnToExploration;
            RestartBattle();
            battleHudController.SetAdventureMode(AdventureRuntime.Run.EncounterActive);
        }

        public void RestartBattle()
        {
            try
            {
                Session = new BattleSession(battleConfig, allies, enemies);
                TestPaused = true;
                _testStarted = false;
                battleHudController.Bind(Session.Context);
                battleHudController.SetTestPaused(TestPaused, _testStarted);
            }
            catch (System.ArgumentException error)
            {
                UnityEngine.Debug.LogError("GAME_01: " + error.Message, this);
                enabled = false;
            }
        }

        private void Update()
        {
            if (!TestPaused) Session?.Tick(Time.unscaledDeltaTime);
            battleHudController?.Tick();
            battleHudController?.SetTestPaused(TestPaused, _testStarted);
        }

        public void ToggleTestPause()
        {
            TestPaused = !TestPaused;
            _testStarted = true;
            battleHudController.SetTestPaused(TestPaused, _testStarted);
        }

        public void ReturnToExploration()
        {
            if (Session == null || Session.Context.CurrentPhase != CombatPhase.BattleEnded
                || !AdventureRuntime.Run.EncounterActive) return;
            AdventureRuntime.Run.FinishEncounter(Session.Context.Winner == CombatTeam.Allies);
            SceneManager.LoadScene(AdventureRuntime.MapScene);
        }

        private void OnDestroy()
        {
            if (battleHudController != null) battleHudController.RestartRequested -= RestartBattle;
            if (battleHudController != null) battleHudController.PauseRequested -= ToggleTestPause;
            if (battleHudController != null) battleHudController.ReturnToMapRequested -= ReturnToExploration;
        }
    }
}
