using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Game01.Narrative.Editor
{
    public static class IntroductionAssetGenerator
    {
        public const string AssetPath = "Assets/GameData/Narrative/Introduction.asset";

        public static DialogueGraph Generate()
        {
            var existing = AssetDatabase.LoadAssetAtPath<DialogueGraph>(AssetPath);
            if (existing != null) return existing;
            if (!AssetDatabase.IsValidFolder("Assets/GameData")) AssetDatabase.CreateFolder("Assets", "GameData");
            if (!AssetDatabase.IsValidFolder("Assets/GameData/Narrative")) AssetDatabase.CreateFolder("Assets/GameData", "Narrative");
            var graph = ScriptableObject.CreateInstance<DialogueGraph>();
            graph.nodes = new List<DialogueNode>
            {
                Node("opening", DialogueSpeaker.Narrator, "No inicio, quase tudo parece feito de linhas. Alem do caminho, um marco guarda a primeira mudanca.", "hero"),
                Node("hero", DialogueSpeaker.Hero, "Voce tambem sente que falta alguma coisa neste lugar?", "question"),
                new DialogueNode { id = "question", speaker = DialogueSpeaker.Mage,
                    text = "Sinto. Podemos procurar respostas, mas primeiro precisamos atravessar o cruzamento. O que voce quer saber?",
                    choices = new List<DialogueChoice>
                    {
                        new DialogueChoice { id = "ask_world", label = "Por que o mundo parece incompleto?", nextNodeId = "explain" },
                        new DialogueChoice { id = "follow_path", label = "Vamos seguir. Descobrimos pelo caminho.", nextNodeId = "go" }
                    } },
                Node("explain", DialogueSpeaker.Mage, "Ainda nao sei. Minha suspeita e que o marco revele detalhes que nao conseguimos enxergar. Precisamos chegar la para testar.", "task"),
                Node("go", DialogueSpeaker.Hero, "Entao vamos em frente. Se alguma coisa mudar, quero ver de perto.", "task"),
                Node("task", DialogueSpeaker.Mage, "O Lobo e o Bandido estao no cruzamento. Prepare nossas acoes antes de enfrentar os dois. Depois, procuramos o marco dourado.", null),
                Node("reminder", DialogueSpeaker.Mage, "Siga o caminho ate o cruzamento. Depois da vitoria, o marco dourado fica a nordeste. O que existe alem dele ainda e uma pergunta.", null)
            };
            AssetDatabase.CreateAsset(graph, AssetPath); AssetDatabase.SaveAssets();
            return graph;
        }

        private static DialogueNode Node(string id, DialogueSpeaker speaker, string text, string next)
        {
            return new DialogueNode { id = id, speaker = speaker, text = text, nextNodeId = next };
        }
    }
}
