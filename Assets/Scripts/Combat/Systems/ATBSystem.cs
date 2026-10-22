using Game01.Combat.Runtime;

namespace Game01.Combat.Systems
{
    public sealed class ATBSystem
    {
        public void Update(BattleContext context, float deltaTime)
        {
            if (context.TimelinePaused)
            {
                return;
            }

            var amountMultiplier = context.Config.tickScale * deltaTime;
            foreach (var combatant in context.AllCombatants())
            {
                combatant.AddATB(combatant.SpeedFinal * amountMultiplier, context.Config.atbThreshold);
            }
        }
    }
}
