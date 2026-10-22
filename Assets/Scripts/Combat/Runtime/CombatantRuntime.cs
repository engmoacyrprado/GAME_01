using System.Collections.Generic;
using Game01.Combat.Core;
using Game01.Combat.Data;
using UnityEngine;

namespace Game01.Combat.Runtime
{
    public sealed class CombatantRuntime
    {
        public CombatantRuntime(string id, CombatTeam team, CombatantDefinition definition)
        {
            Id = id;
            Team = team;
            Definition = definition;

            CurrentHP = definition.stats.maxHP;
            CurrentMP = definition.stats.maxMP;
            RecalculateDerivedStats();
        }

        public string Id { get; }
        public CombatTeam Team { get; }
        public CombatantDefinition Definition { get; }
        public float CurrentHP { get; private set; }
        public float CurrentMP { get; private set; }
        public float CurrentATB { get; private set; }
        public float SpeedBase { get; private set; }
        public float SpeedPenalty { get; private set; }
        public float SpeedFinal { get; private set; }
        public bool IsDefending { get; set; }
        public List<StatusEffectRuntime> ActiveStatus { get; } = new List<StatusEffectRuntime>();

        public bool IsDefeated => CurrentHP <= 0f;

        public void RecalculateDerivedStats()
        {
            var stats = Definition.stats;
            SpeedBase = stats.habilidade + stats.bonusClasse + stats.bonusEquipamento;
            SpeedPenalty = stats.penalidadeArmadura + stats.penalidadePeso;

            var statusPenalty = 0f;
            var statusMultiplier = 1f;

            for (var i = 0; i < ActiveStatus.Count; i++)
            {
                var effect = ActiveStatus[i].Definition;
                statusPenalty += effect.speedFlatPenalty;
                statusMultiplier *= effect.speedMultiplier;
            }

            SpeedPenalty += statusPenalty;
            SpeedFinal = Mathf.Max(CombatConstants.MinimumSpeed, SpeedBase - SpeedPenalty);
            SpeedFinal *= statusMultiplier;
        }

        public void AddATB(float amount, float threshold)
        {
            if (IsDefeated)
            {
                return;
            }

            CurrentATB = Mathf.Min(CurrentATB + amount, threshold * 2f);
        }

        public void ConsumeTimeCost(float timeCost)
        {
            // Negative ATB is recovery debt: heavy actions take longer to recharge.
            CurrentATB -= Mathf.Max(0f, timeCost);
        }

        public void RetainAtb(float threshold, float ratio)
        {
            CurrentATB = Mathf.Clamp(CurrentATB, 0f, threshold * ratio);
        }

        public void ResetATB()
        {
            CurrentATB = 0f;
        }

        public void ApplyDamage(float amount)
        {
            CurrentHP = Mathf.Max(0f, CurrentHP - Mathf.Max(0f, amount));
        }

        public void SpendMP(float amount)
        {
            CurrentMP = Mathf.Max(0f, CurrentMP - Mathf.Max(0f, amount));
        }

        public bool CanAct()
        {
            if (IsDefeated)
            {
                return false;
            }

            for (var i = 0; i < ActiveStatus.Count; i++)
            {
                if (ActiveStatus[i].Definition.blocksActions)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
