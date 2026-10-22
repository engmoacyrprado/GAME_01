using System;
using System.Collections.Generic;
using Game01.Combat.Core;
using Game01.Combat.Data;
using Game01.Combat.StateMachine;

namespace Game01.Combat.Runtime
{
    public sealed class BattleSession
    {
        public BattleContext Context { get; }
        private readonly BattleStateMachine _machine;

        public BattleSession(BattleConfig config, IReadOnlyList<CombatantDefinition> allies, IReadOnlyList<CombatantDefinition> enemies)
        {
            Context = new BattleContext(config);
            AddTeam(allies, Context.Allies, CombatTeam.Allies);
            AddTeam(enemies, Context.Enemies, CombatTeam.Enemies);
            _machine = new BattleStateMachine();
            _machine.Register(CombatPhase.StartCombat, new StartCombatState(_machine));
            _machine.Register(CombatPhase.TimelineUpdate, new TimelineUpdateState(_machine));
            _machine.Register(CombatPhase.ActorReady, new ActorReadyState(_machine));
            _machine.Register(CombatPhase.DecisionPhase, new DecisionPhaseState(_machine));
            _machine.Register(CombatPhase.ResolveAction, new ResolveActionState(_machine));
            _machine.Register(CombatPhase.Aftermath, new AftermathState(_machine));
            _machine.Register(CombatPhase.CheckEnd, new CheckEndState(_machine));
            _machine.Register(CombatPhase.BattleEnded, new BattleEndedState());
            _machine.ChangeState(Context, CombatPhase.StartCombat);
        }

        private static void AddTeam(IReadOnlyList<CombatantDefinition> definitions, List<CombatantRuntime> team, CombatTeam side)
        {
            if (definitions == null || definitions.Count == 0) throw new ArgumentException("Each team needs at least one combatant.");
            for (var i = 0; i < definitions.Count; i++)
            {
                if (definitions[i] == null || definitions[i].stats == null) throw new ArgumentException("Missing combatant definition.");
                team.Add(new CombatantRuntime($"{side}_{i}", side, definitions[i]));
            }
        }

        public void Tick(float deltaTime)
        {
            if (float.IsNaN(deltaTime) || float.IsInfinity(deltaTime) || deltaTime < 0f)
                throw new ArgumentOutOfRangeException(nameof(deltaTime));
            if (Context.CurrentPhase != CombatPhase.BattleEnded) Context.PresentationTime += deltaTime;
            _machine.Tick(Context, deltaTime);
        }
    }
}
