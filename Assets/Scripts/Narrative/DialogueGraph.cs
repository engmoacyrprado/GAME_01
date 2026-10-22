using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game01.Narrative
{
    public enum DialogueSpeaker { Narrator, Hero, Mage }

    [Serializable]
    public sealed class DialogueChoice
    {
        public string id;
        public string label;
        public string nextNodeId;
    }

    [Serializable]
    public sealed class DialogueNode
    {
        public string id;
        public DialogueSpeaker speaker;
        [TextArea(2, 6)] public string text;
        public string nextNodeId;
        public List<DialogueChoice> choices = new List<DialogueChoice>();
    }

    [CreateAssetMenu(menuName = "GAME_01/Narrative/Dialogue", fileName = "Dialogue")]
    public sealed class DialogueGraph : ScriptableObject
    {
        public string entryNodeId = "opening";
        public List<DialogueNode> nodes = new List<DialogueNode>();
    }
}
