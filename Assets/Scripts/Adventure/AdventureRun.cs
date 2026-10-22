using UnityEngine;
using System.Collections.Generic;

namespace Game01.Adventure
{
    public enum VisualEra { Primitive, Retro }

    public sealed class AdventureRun
    {
        public static readonly Vector2 Spawn = new Vector2(-12, -4);
        public Vector2 PlayerPosition { get; set; } = Spawn;
        public bool EncounterActive { get; private set; }
        public bool EncounterWon { get; private set; }
        public VisualEra Era { get; private set; }
        public bool Completed { get; private set; }
        public bool IntroductionSeen { get; set; }
        private readonly Dictionary<string, string> _responses = new Dictionary<string, string>();
        public IReadOnlyDictionary<string, string> Responses => _responses;
        public void RememberResponse(string nodeId, string choiceId) { _responses[nodeId] = choiceId; }

        public bool TryBeginEncounter()
        {
            if (EncounterWon || EncounterActive || Completed) return false;
            EncounterActive = true;
            return true;
        }

        public bool FinishEncounter(bool victory)
        {
            if (!EncounterActive) return false;
            EncounterActive = false;
            EncounterWon = victory;
            return true;
        }

        public bool TryAdvanceEra()
        {
            if (!EncounterWon || EncounterActive || Era != VisualEra.Primitive) return false;
            Era = VisualEra.Retro;
            return true;
        }

        public bool TryComplete()
        {
            if (Era != VisualEra.Retro || EncounterActive || Completed) return false;
            Completed = true;
            return true;
        }
    }

    public static class AdventureRuntime
    {
        public const string MapScene = "ExplorationPrototype";
        public const string BattleScene = "BattlePrototype";
        public static AdventureRun Run { get; private set; } = new AdventureRun();
        public static void Restart() { Run = new AdventureRun(); }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetRuntime() { Restart(); }
    }
}
