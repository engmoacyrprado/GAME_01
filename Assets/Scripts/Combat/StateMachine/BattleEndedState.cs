using Game01.Combat.Core;
using Game01.Combat.Runtime;

namespace Game01.Combat.StateMachine
{
    public sealed class BattleEndedState : IBattleState
    {
        public void Enter(BattleContext context)
        {
            context.TimelinePaused = true;
            context.CurrentActor = null;
            context.ReadyQueue.Clear();
            context.ClearPlayerSelection();
            context.ClearPlannedActions();
            context.Record(context.Winner == CombatTeam.Allies ? "Vitoria!" :
                context.Winner == CombatTeam.Enemies ? "Derrota." : "Empate.");
        }

        public void Tick(BattleContext context, float deltaTime) { }
        public void Exit(BattleContext context) { }
    }
}
