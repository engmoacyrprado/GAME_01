using System;
using Game01.Adventure;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game01.Narrative.Editor
{
    public static class DialogueValidation
    {
        [MenuItem("GAME_01/Narrative/Validate Dialogue")]
        public static void Run()
        {
            var graph = IntroductionAssetGenerator.Generate();
            var session = new DialogueSession(graph);
            Check(session.Current.id == "opening" && session.Current.speaker == DialogueSpeaker.Narrator, "Opening.");
            session.Advance(); Check(session.Current.speaker == DialogueSpeaker.Hero, "Speaker transition.");
            session.Advance(); Check(session.Current.choices.Count == 2 && !session.Advance(), "Choice cannot be skipped.");
            Check(!session.Choose(-1) && !session.Choose(2) && session.Current.id == "question", "Invalid choice.");
            var run = new AdventureRun(); session.ChoiceMade += run.RememberResponse;
            Check(session.Choose(0) && session.Current.id == "explain" && run.Responses["question"] == "ask_world", "Explanation branch.");
            session.Advance(); session.Advance(); Check(!session.IsActive && !session.Advance() && !session.Choose(0), "Dialogue termination.");
            var other = new DialogueSession(graph, "question");
            Check(other.Choose(1) && other.Current.id == "go", "Continue branch.");
            other.Advance(); Check(other.Current.id == "task", "Branches converge explicitly.");
            var reminder = new DialogueSession(graph, "reminder"); reminder.Advance(); Check(!reminder.IsActive, "Repeat conversation.");
            var invalid = ScriptableObject.CreateInstance<DialogueGraph>();
            try
            {
                invalid.nodes.Add(new DialogueNode { id = "opening", text = "Test", nextNodeId = "missing" });
                Check(Rejects(invalid), "Broken links must fail.");
                invalid.nodes[0].nextNodeId = null;
                invalid.nodes.Add(new DialogueNode { id = "opening", text = "Duplicate" });
                Check(Rejects(invalid), "Duplicate nodes must fail.");
            }
            finally { Object.DestroyImmediate(invalid); }
            AdventureRuntime.Restart(); Check(AdventureRuntime.Run.Responses.Count == 0 && !AdventureRuntime.Run.IntroductionSeen, "Narrative reset.");
            UnityEngine.Debug.Log("GAME_01 DIALOGUE VALIDATION: 12 checks passed.");
        }

        private static bool Rejects(DialogueGraph graph)
        {
            try { new DialogueSession(graph); return false; }
            catch (ArgumentException) { return true; }
        }

        private static void Check(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException("Dialogue validation: " + message);
        }
    }
}
