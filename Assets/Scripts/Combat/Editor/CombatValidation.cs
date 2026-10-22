using System;
using System.Collections.Generic;
using Game01.Combat.Actions;
using Game01.Combat.Core;
using Game01.Combat.Data;
using Game01.Combat.Runtime;
using Game01.Combat.Systems;
using Game01.Combat.UI;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game01.Combat.Editor
{
    public static class CombatValidation
    {
        private static int _passed;

        [MenuItem("GAME_01/Combat/Validate Combat Rules")]
        public static void Run()
        {
            _passed = 0;
            Test("Timeout passes without a PassTurn asset", f =>
            {
                f.Ready();
                var hp = f.Enemy.CurrentHP;
                f.Session.Tick(1f);
                Check(f.Enemy.CurrentHP == hp && f.Hero.CurrentATB < f.Config.atbThreshold, "Timeout attacked or failed to consume time.");
            });
            Test("Decision pauses ATB but timer advances", f =>
            {
                f.Ready();
                var atb = f.Enemy.CurrentATB;
                var timer = f.Context.DecisionTimerRemaining;
                f.Session.Tick(0.05f);
                Check(f.Enemy.CurrentATB == atb && f.Context.DecisionTimerRemaining < timer, "Pause/timer mismatch.");
            });
            Test("Active timing mode advances ATB", f =>
            {
                f.Config.pauseTimelineDuringPlayerDecision = false;
                f.Ready();
                var atb = f.Enemy.CurrentATB;
                f.Session.Tick(0.05f);
                Check(f.Enemy.CurrentATB > atb, "Active mode did not advance time.");
            });
            Test("Input outside decision rejected", f => Check(!f.Context.SetPlayerSelection(f.Attack, f.Enemy), "Accepted stale input."));
            Test("Insufficient MP rejected", f =>
            {
                f.Ready(); f.Attack.manaCost = 1000;
                Check(!f.Context.SetPlayerSelection(f.Attack, f.Enemy), "Overspent MP.");
            });
            Test("Friendly, defeated and foreign targets rejected", f =>
            {
                f.Ready();
                Check(!f.Context.SetPlayerSelection(f.Attack, f.Hero), "Friendly fire accepted.");
                var foreign = new CombatantRuntime("foreign", CombatTeam.Enemies, f.Enemy.Definition);
                Check(!f.Context.SetPlayerSelection(f.Attack, foreign), "Foreign target accepted.");
                f.Enemy.ApplyDamage(10000);
                Check(!f.Context.SetPlayerSelection(f.Attack, f.Enemy), "Defeated target accepted.");
            });
            Test("Attack applies once and duplicate input rejected", f =>
            {
                f.Ready(); f.Attack.manaCost = 3;
                var hp = f.Enemy.CurrentHP;
                Check(f.Context.SetPlayerSelection(f.Attack, f.Enemy), "Valid input rejected.");
                Check(!f.Context.SetPlayerSelection(f.Attack, f.Enemy), "Duplicate accepted.");
                f.Session.Tick(0.01f);
                Check(f.Enemy.CurrentHP < hp && f.Hero.CurrentMP == 17, "Attack/MP not applied once.");
            });
            Test("Heavy action creates recovery debt", f =>
            {
                f.Ready(); f.Attack.timeCost = 150;
                f.Context.SetPlayerSelection(f.Attack, f.Enemy); f.Session.Tick(0.01f);
                Check(f.Hero.CurrentATB < 0, "High costs were clamped away.");
            });
            Test("Wait uses configured threshold", f =>
            {
                f.Config.atbThreshold = 200; f.Ready();
                f.Attack.actionType = CombatActionType.Wait; f.Attack.targetType = TargetType.Self;
                f.Context.SetPlayerSelection(f.Attack, f.Hero); f.Session.Tick(0.01f);
                Check(Mathf.Approximately(f.Hero.CurrentATB, 100), "Wait used hardcoded threshold.");
            });
            Test("Magic uses magical power", f =>
            {
                f.Attack.actionType = CombatActionType.Magic;
                f.Hero.Definition.stats.poderMagico = 50;
                var result = f.Context.CombatResolver.Resolve(new CombatActionCommand(f.Hero, f.Enemy, f.Attack));
                Check(result.DamageAmount == 59, "Magic used physical attack.");
            });
            Test("Defense protects multiple hits until next turn", f =>
            {
                f.Hero.IsDefending = true;
                var command = new CombatActionCommand(f.Enemy, f.Hero, f.Attack);
                var first = f.Context.CombatResolver.Resolve(command).DamageAmount;
                var second = f.Context.CombatResolver.Resolve(command).DamageAmount;
                Check(first == second && f.Hero.IsDefending, "Defense expired on first hit.");
                f.Ready();
                Check(!f.Hero.IsDefending, "Defense did not expire at next turn.");
            });
            Test("Blocked actor consumes turn", f =>
            {
                var status = f.New<StatusEffectDefinition>(); status.blocksActions = true;
                f.Hero.ActiveStatus.Add(new StatusEffectRuntime(status));
                f.Hero.AddATB(100, 100); f.Session.Tick(0.01f);
                Check(f.Hero.CurrentATB < 100 && f.Context.CurrentPhase != CombatPhase.DecisionPhase, "Blocked actor stuck in ready loop.");
            });
            Test("HAB breaks equal ATB and speed", f =>
            {
                f.Enemy.Definition.stats.habilidade = 30;
                f.Enemy.Definition.stats.penalidadePeso = 10;
                f.Enemy.RecalculateDerivedStats();
                f.Hero.AddATB(100, 100); f.Enemy.AddATB(100, 100);
                f.Context.ReadyQueueSystem.Refresh(f.Context);
                Check(f.Context.ReadyQueue[0] == f.Enemy, "Missing HAB tie breaker.");
                Check(PrototypeBattleView.ComparePredictedOrder(f.Enemy, f.Hero, f.Context) < 0,
                    "UI prediction disagrees with the ready queue tie breaker.");
            });
            Test("Victory enters terminal state", f =>
            {
                f.Enemy.ApplyDamage(10000); f.Ready(); f.Session.Tick(1);
                Check(f.Context.CurrentPhase == CombatPhase.BattleEnded && f.Context.Winner == CombatTeam.Allies, "Victory not terminal.");
                var atb = f.Hero.CurrentATB; f.Session.Tick(100);
                Check(f.Hero.CurrentATB == atb, "Terminal battle still advances.");
            });
            Test("Defeat and draw are distinct", f =>
            {
                f.Hero.ApplyDamage(10000);
                Check(f.Context.VictoryConditionSystem.TryGetWinner(f.Context, out var winner) && winner == CombatTeam.Enemies, "Defeat missing.");
                f.Enemy.ApplyDamage(10000);
                Check(f.Context.VictoryConditionSystem.TryGetWinner(f.Context, out winner) && winner == CombatTeam.None, "Draw missing.");
            });
            Test("Unsupported item/flee actions cannot become attacks", f =>
            {
                f.Ready(); f.Attack.actionType = CombatActionType.Item;
                Check(!f.Context.SetPlayerSelection(f.Attack, f.Enemy), "Item accepted as damage.");
                f.Attack.actionType = CombatActionType.Flee;
                Check(!f.Context.SetPlayerSelection(f.Attack, f.Enemy), "Flee accepted as damage.");
            });
            Test("Full automated 2x2 battle completes", f =>
            {
                var session = new BattleSession(f.Config,
                    new[] { f.Hero.Definition, f.Hero.Definition }, new[] { f.Enemy.Definition, f.Enemy.Definition });
                for (var frame = 0; frame < 30000 && session.Context.CurrentPhase != CombatPhase.BattleEnded; frame++)
                {
                    var context = session.Context;
                    if (context.CurrentPhase == CombatPhase.DecisionPhase)
                        context.SetPlayerSelection(f.Attack, context.TargetingSystem.GetDefaultTarget(context, context.CurrentActor));
                    session.Tick(1f / 60f);
                }
                Check(session.Context.CurrentPhase == CombatPhase.BattleEnded && session.Context.Winner == CombatTeam.Allies, "2x2 battle failed to finish.");
            });
            Test("Planning does not spend resources or execute early", f =>
            {
                f.Attack.manaCost = 3;
                var hp = f.Enemy.CurrentHP;
                Check(f.Context.PlanPlayerAction(f.Hero, f.Attack, f.Enemy), "Plan rejected.");
                Check(f.Hero.CurrentMP == 20 && f.Hero.CurrentATB == 0 && f.Enemy.CurrentHP == hp, "Plan executed early.");
            });
            Test("Plan executes exactly once at next readiness", f =>
            {
                f.Attack.manaCost = 3;
                f.Context.PlanPlayerAction(f.Hero, f.Attack, f.Enemy);
                f.Ready();
                Check(f.Hero.CurrentMP == 17 && f.Context.GetPlannedAction(f.Hero) == null, "Plan not consumed.");
                var hp = f.Enemy.CurrentHP;
                f.Ready();
                Check(f.Context.CurrentPhase == CombatPhase.DecisionPhase && f.Enemy.CurrentHP == hp, "Plan repeated on next turn.");
            });
            Test("Plan may be replaced and cancelled", f =>
            {
                var replacement = f.New<CombatActionDefinition>();
                replacement.actionType = CombatActionType.Magic;
                f.Hero.Definition.availableActions.Add(replacement);
                f.Context.PlanPlayerAction(f.Hero, f.Attack, f.Enemy);
                f.Context.PlanPlayerAction(f.Hero, replacement, f.Enemy);
                Check(f.Context.GetPlannedAction(f.Hero).Action == replacement, "Replacement failed.");
                f.Context.CancelPlannedAction(f.Hero);
                f.Ready();
                Check(f.Context.CurrentPhase == CombatPhase.DecisionPhase && f.Context.GetPlannedAction(f.Hero) == null, "Cancellation failed.");
            });
            Test("Defeated planned target opens a fresh decision without retargeting", f =>
            {
                var other = new CombatantRuntime("other", CombatTeam.Enemies, f.Enemy.Definition);
                f.Context.Enemies.Add(other);
                f.Context.PlanPlayerAction(f.Hero, f.Attack, f.Enemy);
                f.Enemy.ApplyDamage(10000);
                f.Ready();
                Check(f.Context.CurrentPhase == CombatPhase.DecisionPhase && f.Context.DecisionTimerRemaining == f.Config.decisionTimeLimit
                    && other.CurrentHP == other.Definition.stats.maxHP && f.Context.GetPlannedAction(f.Hero) == null, "Invalid target silently retargeted or lost decision time.");
            });
            Test("Resource loss invalidates a plan at execution", f =>
            {
                f.Attack.manaCost = 3;
                f.Context.PlanPlayerAction(f.Hero, f.Attack, f.Enemy);
                f.Hero.SpendMP(20);
                f.Ready();
                Check(f.Context.CurrentPhase == CombatPhase.DecisionPhase && f.Context.SelectedAction == null, "Plan bypassed MP validation.");
            });
            Test("Planning another ally preserves the current decision", f =>
            {
                var other = new CombatantRuntime("ally2", CombatTeam.Allies, f.Hero.Definition);
                f.Context.Allies.Add(other);
                f.Ready();
                var timer = f.Context.DecisionTimerRemaining;
                Check(f.Context.PlanPlayerAction(other, f.Attack, f.Enemy), "Other ally plan rejected.");
                Check(f.Context.CurrentActor == f.Hero && f.Context.SelectedAction == null
                    && f.Context.DecisionTimerRemaining == timer, "Planning stole the current turn or reset its timer.");
            });
            Test("Enemy and foreign combatants cannot receive player plans", f =>
            {
                var foreign = new CombatantRuntime("foreign", CombatTeam.Allies, f.Hero.Definition);
                Check(!f.Context.PlanPlayerAction(f.Enemy, f.Attack, f.Hero)
                    && !f.Context.PlanPlayerAction(foreign, f.Attack, f.Enemy), "Unauthorized actor accepted.");
            });
            Test("Current actor planning confirms immediately", f =>
            {
                f.Ready();
                Check(f.Context.PlanPlayerAction(f.Hero, f.Attack, f.Enemy) && f.Context.SelectedAction == f.Attack
                    && f.Context.GetPlannedAction(f.Hero) == null, "Current decision was queued for an extra turn.");
            });
            Test("Presentation holds time without applying damage twice", f =>
            {
                f.Config.actionPresentationDuration = 0.5f;
                f.Ready(); f.Context.SetPlayerSelection(f.Attack, f.Enemy); f.Session.Tick(0.01f);
                var hp = f.Enemy.CurrentHP; var atb = f.Enemy.CurrentATB;
                Check(f.Context.CurrentPhase == CombatPhase.ResolveAction, "Presentation phase skipped.");
                f.Session.Tick(0.25f);
                Check(f.Context.CurrentPhase == CombatPhase.ResolveAction && f.Enemy.CurrentHP == hp && f.Enemy.CurrentATB == atb,
                    "Presentation advanced ATB or repeated damage.");
                f.Session.Tick(0.25f);
                Check(f.Context.CurrentPhase == CombatPhase.TimelineUpdate, "Presentation failed to release the timeline.");
            });
            Test("New defaults are ten seconds and slower ATB", f =>
            {
                var config = f.New<BattleConfig>();
                Check(config.decisionTimeLimit == 10f && Mathf.Approximately(config.tickScale, 1.8f), "New pacing defaults missing.");
            });
            UnityEngine.Debug.Log($"GAME_01 VALIDATION: {_passed} checks passed.");
        }

        private static void Test(string name, Action<Fixture> test)
        {
            using (var fixture = new Fixture()) test(fixture);
            _passed++;
            UnityEngine.Debug.Log("PASS: " + name);
        }

        private static void Check(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException("GAME_01 validation: " + message);
        }

        private sealed class Fixture : IDisposable
        {
            private readonly List<Object> _objects = new List<Object>();
            public BattleConfig Config;
            public CombatActionDefinition Attack;
            public BattleSession Session;
            public BattleContext Context => Session.Context;
            public CombatantRuntime Hero => Context.Allies[0];
            public CombatantRuntime Enemy => Context.Enemies[0];

            public Fixture()
            {
                Config = New<BattleConfig>(); Config.tickScale = 2; Config.decisionTimeLimit = 0.3f;
                Config.actionPresentationDuration = 0f;
                Attack = New<CombatActionDefinition>();
                var hero = New<CombatantDefinition>(); hero.stats.habilidade = 20; hero.stats.maxHP = 1000; hero.stats.maxMP = 20;
                hero.availableActions.Add(Attack);
                var enemy = New<CombatantDefinition>(); enemy.stats.habilidade = 1; enemy.stats.maxHP = 1000; enemy.stats.defesa = 1;
                enemy.availableActions.Add(Attack);
                Session = new BattleSession(Config, new[] { hero }, new[] { enemy });
            }

            public T New<T>() where T : ScriptableObject
            {
                var value = ScriptableObject.CreateInstance<T>(); _objects.Add(value); return value;
            }

            public void Ready()
            {
                Hero.AddATB(Config.atbThreshold, Config.atbThreshold);
                Session.Tick(0.01f);
            }

            public void Dispose()
            {
                foreach (var value in _objects) Object.DestroyImmediate(value);
            }
        }
    }
}
