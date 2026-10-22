using System;
using UnityEditor;
using UnityEngine;

namespace Game01.Adventure.Editor
{
    public static class AdventureValidation
    {
        [MenuItem("GAME_01/Adventure/Validate Progression")]
        public static void Run()
        {
            var run = new AdventureRun();
            Check(run.PlayerPosition == AdventureRun.Spawn && !run.EncounterWon && run.Era == VisualEra.Primitive, "Initial state.");
            Check(!run.TryAdvanceEra() && !run.TryComplete(), "Progression bypassed the battle.");
            Check(!run.FinishEncounter(true), "Victory without an active encounter.");
            Check(run.TryBeginEncounter() && !run.TryBeginEncounter(), "Duplicate encounter.");
            Check(!run.TryAdvanceEra() && !run.TryComplete(), "Progression while in battle.");
            var position = new Vector2(-1, 0); run.PlayerPosition = position;
            Check(run.FinishEncounter(false) && !run.EncounterWon && run.PlayerPosition == position, "Defeat or position preservation.");
            Check(run.TryBeginEncounter() && run.FinishEncounter(true) && run.EncounterWon, "Retry then victory.");
            Check(!run.TryBeginEncounter(), "Cleared encounter restarted.");
            Check(run.TryAdvanceEra() && !run.TryAdvanceEra() && run.Era == VisualEra.Retro, "Era transition.");
            Check(run.TryComplete() && !run.TryComplete(), "Completion.");
            AdventureRuntime.Restart();
            Check(!AdventureRuntime.Run.EncounterWon && !AdventureRuntime.Run.Completed && AdventureRuntime.Run.PlayerPosition == AdventureRun.Spawn, "Restart.");
            UnityEngine.Debug.Log("GAME_01 ADVENTURE VALIDATION: 11 checks passed.");
        }

        private static void Check(bool condition, string scenario)
        {
            if (!condition) throw new InvalidOperationException("Adventure validation failed: " + scenario);
        }
    }
}
