using Game01.Combat.Core;
using Game01.Combat.Runtime;

namespace Game01.Combat.Systems
{
    public sealed class VictoryConditionSystem
    {
        public bool TryGetWinner(BattleContext context, out CombatTeam winner)
        {
            var alliesAlive = false;
            var enemiesAlive = false;

            for (var i = 0; i < context.Allies.Count; i++)
            {
                alliesAlive |= !context.Allies[i].IsDefeated;
            }

            for (var i = 0; i < context.Enemies.Count; i++)
            {
                enemiesAlive |= !context.Enemies[i].IsDefeated;
            }

            if (alliesAlive && enemiesAlive)
            {
                winner = CombatTeam.None;
                return false;
            }

            winner = alliesAlive ? CombatTeam.Allies : enemiesAlive ? CombatTeam.Enemies : CombatTeam.None;
            return true;
        }
    }
}
