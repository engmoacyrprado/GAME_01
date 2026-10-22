using Game01.Combat.Core;
using Game01.Combat.Runtime;
using Game01.Combat.Data;

namespace Game01.Combat.Systems
{
    public sealed class TargetingSystem
    {
        public bool IsValidTarget(BattleContext context, CombatantRuntime actor, CombatActionDefinition action, CombatantRuntime target)
        {
            if (target == null || target.IsDefeated || (!context.Allies.Contains(target) && !context.Enemies.Contains(target)))
                return false;
            switch (action.targetType)
            {
                case TargetType.Self: return target == actor;
                case TargetType.SingleEnemy: return target.Team != actor.Team;
                case TargetType.SingleAlly: return target.Team == actor.Team;
                default: return false;
            }
        }

        public CombatantRuntime GetDefaultTarget(BattleContext context, CombatantRuntime actor)
        {
            var candidates = actor.Team == CombatTeam.Allies ? context.Enemies : context.Allies;
            for (var i = 0; i < candidates.Count; i++)
            {
                if (!candidates[i].IsDefeated)
                {
                    return candidates[i];
                }
            }

            return null;
        }
    }
}
