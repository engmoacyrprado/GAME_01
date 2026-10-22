using Game01.Combat.Runtime;

namespace Game01.Combat.StateMachine
{
    public interface IBattleState
    {
        void Enter(BattleContext context);
        void Tick(BattleContext context, float deltaTime);
        void Exit(BattleContext context);
    }
}
