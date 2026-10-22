using System.Collections.Generic;
using Game01.Combat.Runtime;

namespace Game01.Combat.Systems
{
    public sealed class ReadyQueueSystem
    {
        public void Refresh(BattleContext context)
        {
            context.ReadyQueue.Clear();

            foreach (var combatant in context.AllCombatants())
            {
                if (combatant.IsDefeated)
                {
                    continue;
                }

                if (combatant.CurrentATB >= context.Config.atbThreshold)
                {
                    context.ReadyQueue.Add(combatant);
                }
            }

            context.ReadyQueue.Sort(CompareCombatants);
        }

        public CombatantRuntime PopNext(BattleContext context)
        {
            if (context.ReadyQueue.Count == 0)
            {
                return null;
            }

            var combatant = context.ReadyQueue[0];
            context.ReadyQueue.RemoveAt(0);
            return combatant;
        }

        public static int CompareCombatants(CombatantRuntime a, CombatantRuntime b)
        {
            var atb = b.CurrentATB.CompareTo(a.CurrentATB);
            if (atb != 0)
            {
                return atb;
            }

            var speed = b.SpeedFinal.CompareTo(a.SpeedFinal);
            if (speed != 0)
            {
                return speed;
            }

            var habilidade = b.Definition.stats.habilidade.CompareTo(a.Definition.stats.habilidade);
            return habilidade != 0 ? habilidade : string.CompareOrdinal(a.Id, b.Id);
        }
    }
}
