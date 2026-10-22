using System;
using System.Collections.Generic;

namespace Game01.Narrative
{
    public sealed class DialogueSession
    {
        private readonly Dictionary<string, DialogueNode> _nodes = new Dictionary<string, DialogueNode>();
        public DialogueNode Current { get; private set; }
        public bool IsActive => Current != null;
        public event Action<string, string> ChoiceMade;

        public DialogueSession(DialogueGraph graph, string startNodeId = null)
        {
            if (graph == null || graph.nodes == null) throw new ArgumentException("Missing dialogue graph.");
            foreach (var node in graph.nodes)
            {
                if (node == null || string.IsNullOrWhiteSpace(node.id) || _nodes.ContainsKey(node.id)
                    || string.IsNullOrWhiteSpace(node.text) || node.choices == null || node.choices.Count > 3)
                    throw new ArgumentException("Invalid or duplicate dialogue node.");
                _nodes.Add(node.id, node);
            }
            foreach (var node in graph.nodes)
            {
                ValidateLink(node.nextNodeId);
                var choiceIds = new HashSet<string>();
                foreach (var choice in node.choices)
                {
                    if (choice == null || string.IsNullOrWhiteSpace(choice.id) || !choiceIds.Add(choice.id)
                        || string.IsNullOrWhiteSpace(choice.label)) throw new ArgumentException("Invalid dialogue choice.");
                    ValidateLink(choice.nextNodeId);
                }
            }
            var entry = startNodeId ?? graph.entryNodeId;
            if (entry == null || !_nodes.TryGetValue(entry, out var current)) throw new ArgumentException("Dialogue entry does not exist.");
            Current = current;
        }

        private void ValidateLink(string id)
        {
            if (!string.IsNullOrEmpty(id) && !_nodes.ContainsKey(id)) throw new ArgumentException("Dialogue link does not exist: " + id);
        }

        public bool Advance()
        {
            if (!IsActive || Current.choices.Count > 0) return false;
            MoveTo(Current.nextNodeId);
            return true;
        }

        public bool Choose(int index)
        {
            if (!IsActive || index < 0 || index >= Current.choices.Count) return false;
            var node = Current;
            var choice = node.choices[index];
            MoveTo(choice.nextNodeId);
            ChoiceMade?.Invoke(node.id, choice.id);
            return true;
        }

        private void MoveTo(string id) { Current = string.IsNullOrEmpty(id) ? null : _nodes[id]; }
    }
}
