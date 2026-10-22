using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Game01.Combat.Core;
using Game01.Combat.Data;
using Game01.Combat.Runtime;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game01.Combat.Editor
{
    public static class CombatPacingAnalysis
    {
        [MenuItem("GAME_01/Combat/Analyze Battle Pacing")]
        public static void Run()
        {
            var results = new List<Result>
            {
                Simulate("Basico / decisao 1s", "basic", 1),
                Simulate("Basico / decisao 3s", "basic", 3),
                Simulate("Basico / decisao 5.5s", "basic", 5.5f),
                Simulate("Pesado / decisao 1s", "heavy", 1),
                Simulate("Magia / decisao 1s", "magic", 1),
                Simulate("Heroi pesado + Mago magia / 1s", "mixed", 1),
                Simulate("Heroi pesado + Mago magia / 3s", "mixed", 3),
                Simulate("Defesa alternada do Heroi / 1s", "defend", 1),
                Simulate("Somente esperar / 1s", "wait", 1),
                Simulate("Somente passar / 1s", "pass", 1),
                Simulate("Sem responder / timeout 6s", "basic", 7),
                Simulate("Basico / decisao 7s / limite 10s", "basic", 7, 2, 10),
                Simulate("Basico / 3s / escala 2.5", "basic", 3, 2.5f),
                Simulate("Basico / 3s / escala 3", "basic", 3, 3),
                Simulate("Basico / priorizar Bandido / 1s", "bandit", 1),
                Simulate("Basico / 1s / simulacao 30 FPS", "basic", 1, 2, 6, 30),
                Simulate("Atual: basico / decisao 1s", "basic", 1, 1.8f, 10, 60, 0.55f),
                Simulate("Atual: pesado + magia / decisao 1s", "mixed", 1, 1.8f, 10, 60, 0.55f),
                Simulate("Atual: planos automaticos ideais", "planned", 0, 1.8f, 10, 60, 0.55f)
            };
            Directory.CreateDirectory("docs/reports");
            var report = new StringBuilder("# Ritmo da batalha - medicao de 2026-10-22\n\n");
            report.Append("Simulacao determinista da BattleSession com os assets reais da cena 2x2. ");
            report.Append("Tempo total inclui espera ATB e demora artificial de decisao; nao mede FPS, latencia do computador ou tempo de leitura humano. ");
            report.Append("Perfis sem prefixo Atual preservam a medicao anterior, com apresentacao de duracao zero. Perfis Atual usam os novos tempos e intervalo visual. Cada perfil e uma execucao determinista, nao uma amostra estatistica.\n\n");
            report.Append("Alvo padrao: primeiro inimigo vivo (Lobo, depois Bandido). IA: primeira acao utilizavel, primeiro aliado vivo. ");
            report.Append("Magia/pesado usam ataque basico como fallback quando indisponiveis. Defesa alternada afeta apenas o Heroi; Mago ataca. ");
            report.Append("Baseline: escala 2, limiar 100, decisao 6s, pausa ativa. Atual: escala 1.8, decisao 10s e apresentacao 0.55s. Planos automaticos ideais escolhem imediatamente pesado/magia e alvo vivo; nao medem o tempo de planejamento humano.\n\n");
            report.Append("| Perfil | Resultado | Total (s) | ATB ativo (s) | Decisao (s) | Apresentacao (s) | Acoes aliadas | Golpes inimigos | Timeouts | HP restante | MP gasto |\n");
            report.Append("|---|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|\n");
            foreach (var r in results)
                report.AppendFormat(CultureInfo.InvariantCulture,
                    "| {0} | {1} | {2:0.0} | {3:0.0} | {4:0.0} | {5:0.0} | {6} | {7} | {8} | {9:0} / 200 | {10:0} |\n",
                    r.Name, r.Outcome, r.Total, r.Timeline, r.Decision, r.Presentation, r.Confirmed, r.EnemyHits, r.Timeouts, r.Hp, r.MpSpent);
            report.Append("\n## Primeira abertura de decisao\n\n");
            report.AppendFormat(CultureInfo.InvariantCulture,
                "No perfil basico de 1s: Heroi em {0:0.00}s e Mago em {1:0.00}s de tempo total simulado. ", results[0].FirstHero, results[0].FirstMage);
            report.Append("O Mago depende da demora do Heroi porque as barras ficam pausadas durante a decisao. ");
            report.Append("O Lobo tem velocidade 14, igual ao Heroi; desempata por Habilidade (14 contra 12) e ataca primeiro.\n\n");
            report.Append("## Recuperacao teorica entre acoes\n\n");
            report.Append("Sem pausas de outros participantes, sem excesso inicial de ATB e com escala 2.\n\n");
            report.Append("| Acao | Heroi (velocidade 14) | Mago (velocidade 13) |\n|---|---:|---:|\n");
            foreach (var entry in new[] { ("Ataque basico", 100f), ("Golpe pesado", 150f), ("Magia", 120f), ("Defender", 80f), ("Esperar (50%)", 50f) })
                report.AppendFormat(CultureInfo.InvariantCulture, "| {0} | {1:0.00}s | {2:0.00}s |\n", entry.Item1, entry.Item2 / 28f, entry.Item2 / 26f);
            report.Append("\n## Leitura dos resultados\n\n");
            report.Append("- Com pausa, demorar para decidir alonga o tempo real, mas nao da ataques extras aos inimigos antes do timeout.\n");
            report.Append("- Esperar/defender sao ferramentas de apoio. So esperar ou passar nao causa dano e leva a derrota.\n");
            report.Append("- A IA concentra dano sempre no primeiro aliado vivo; estes resultados nao representam IA que troca de alvo.\n");
            report.Append("- A nova apresentacao separa acoes por 0.55s com ATB parado. Os movimentos sao provisorios.\n");
            report.Append("- Dano e duracao nao equivalem a diversao. Conforto do prazo e clareza precisam da avaliacao do jogador.\n");
            File.WriteAllText("docs/reports/battle-pacing-2026-10-22.md", report.ToString());
            UnityEngine.Debug.Log("GAME_01 PACING: " + results.Count + " profiles saved to docs/reports/battle-pacing-2026-10-22.md");
        }

        private static Result Simulate(string name, string policy, float delay, float scale = 2, float limit = 6, int fps = 60, float presentation = 0)
        {
            var config = Object.Instantiate(Load<BattleConfig>("Configs/BattleConfig_Prototype"));
            try
            {
                config.tickScale = scale; config.decisionTimeLimit = limit;
                config.actionPresentationDuration = presentation;
                var session = new BattleSession(config,
                    new[] { Load<CombatantDefinition>("Combatants/Combatant_Hero"), Load<CombatantDefinition>("Combatants/Combatant_Mage") },
                    new[] { Load<CombatantDefinition>("Combatants/Combatant_Wolf"), Load<CombatantDefinition>("Combatants/Combatant_Bandit") });
                var context = session.Context;
                var result = new Result { Name = name };
                var dt = 1f / fps;
                var age = 0f;
                var heroTurns = 0;
                CombatantRuntime lastActor = null;
                for (var frame = 0; frame < fps * 360 && context.CurrentPhase != CombatPhase.BattleEnded; frame++)
                {
                    if (policy == "planned")
                    {
                        for (var i = 0; i < context.Allies.Count; i++)
                        {
                            var ally = context.Allies[i];
                            if (ally.IsDefeated || context.GetPlannedAction(ally) != null) continue;
                            var preferred = i == 0 ? CombatActionType.Technique : CombatActionType.Magic;
                            var prepared = ally.Definition.availableActions.FirstOrDefault(a => a.actionType == preferred && context.CanUseAction(ally, a))
                                ?? ally.Definition.availableActions.First(a => a.actionType == CombatActionType.BasicAttack);
                            context.PlanPlayerAction(ally, prepared, context.TargetingSystem.GetDefaultTarget(context, ally));
                        }
                    }
                    var deciding = context.CurrentPhase == CombatPhase.DecisionPhase;
                    if (deciding)
                    {
                        if (lastActor != context.CurrentActor)
                        {
                            age = 0;
                            if (context.CurrentActor == context.Allies[0])
                            {
                                heroTurns++;
                                if (result.FirstHero == 0) result.FirstHero = result.Total;
                            }
                            else if (result.FirstMage == 0) result.FirstMage = result.Total;
                        }
                        age += dt;
                        if (policy != "planned" && age >= delay && context.SelectedAction == null)
                        {
                            var actor = context.CurrentActor;
                            var type = CombatActionType.BasicAttack;
                            if (policy == "heavy" || (policy == "mixed" && actor == context.Allies[0])) type = CombatActionType.Technique;
                            if (policy == "magic" || (policy == "mixed" && actor == context.Allies[1])) type = CombatActionType.Magic;
                            if (policy == "wait") type = CombatActionType.Wait;
                            if (policy == "pass") type = CombatActionType.PassTurn;
                            if (policy == "defend" && actor == context.Allies[0] && heroTurns % 2 == 1) type = CombatActionType.Defend;
                            var action = actor.Definition.availableActions.FirstOrDefault(a => a.actionType == type && context.CanUseAction(actor, a))
                                ?? actor.Definition.availableActions.First(a => a.actionType == CombatActionType.BasicAttack);
                            var target = action.targetType == TargetType.Self ? actor : policy == "bandit"
                                ? context.Enemies.LastOrDefault(e => !e.IsDefeated) : context.TargetingSystem.GetDefaultTarget(context, actor);
                            if (context.SetPlayerSelection(action, target)) result.Confirmed++;
                        }
                        result.Decision += dt;
                    }
                    else if (context.CurrentPhase == CombatPhase.ResolveAction) result.Presentation += dt;
                    else result.Timeline += dt;
                    lastActor = context.CurrentActor;
                    var hpBefore = context.Allies.Sum(a => a.CurrentHP);
                    var enemyHpBefore = context.Enemies.Sum(a => a.CurrentHP);
                    var timeout = deciding && context.SelectedAction == null && context.DecisionTimerRemaining <= dt;
                    session.Tick(dt);
                    if (context.Allies.Sum(a => a.CurrentHP) < hpBefore) result.EnemyHits++;
                    if (policy == "planned" && context.Enemies.Sum(a => a.CurrentHP) < enemyHpBefore) result.Confirmed++;
                    if (timeout) result.Timeouts++;
                    result.Total += dt;
                }
                result.Outcome = context.CurrentPhase != CombatPhase.BattleEnded ? "Limite 360s" :
                    context.Winner == CombatTeam.Allies ? "Vitoria" : context.Winner == CombatTeam.Enemies ? "Derrota" : "Empate";
                result.Hp = context.Allies.Sum(a => a.CurrentHP);
                result.MpSpent = context.Allies.Sum(a => a.Definition.stats.maxMP - a.CurrentMP);
                return result;
            }
            finally { Object.DestroyImmediate(config); }
        }

        private static T Load<T>(string name) where T : Object =>
            AssetDatabase.LoadAssetAtPath<T>("Assets/GameData/Combat/" + name + ".asset");

        private sealed class Result
        {
            public string Name, Outcome;
            public float Total, Timeline, Decision, Presentation, Hp, MpSpent, FirstHero, FirstMage;
            public int Confirmed, EnemyHits, Timeouts;
        }
    }
}
