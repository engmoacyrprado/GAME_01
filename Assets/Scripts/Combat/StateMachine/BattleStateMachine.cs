using System.Collections.Generic;
using Game01.Combat.Core;
using Game01.Combat.Runtime;

namespace Game01.Combat.StateMachine
{
    public sealed class BattleStateMachine
    {
        private readonly Dictionary<CombatPhase, IBattleState> _states = new Dictionary<CombatPhase, IBattleState>();
        private IBattleState _currentState;

        public void Register(CombatPhase phase, IBattleState state)
        {
            _states[phase] = state;
        }

        public void ChangeState(BattleContext context, CombatPhase nextPhase)
        {
            _currentState?.Exit(context);
            context.CurrentPhase = nextPhase;
            _currentState = _states[nextPhase];
            _currentState.Enter(context);
        }

        public void Tick(BattleContext context, float deltaTime)
        {
            _currentState?.Tick(context, deltaTime);
        }
    }
}
