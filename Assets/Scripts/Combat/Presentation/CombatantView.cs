using Game01.Combat.Runtime;
using UnityEngine;

namespace Game01.Combat.Presentation
{
    public class CombatantView : MonoBehaviour
    {
        public string combatantId;

        public void Bind(CombatantRuntime runtime)
        {
            combatantId = runtime.Id;
        }
    }
}
