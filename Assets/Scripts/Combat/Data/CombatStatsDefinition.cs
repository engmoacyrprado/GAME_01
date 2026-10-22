using System;
using UnityEngine;

namespace Game01.Combat.Data
{
    [Serializable]
    public class CombatStatsDefinition
    {
        [Min(1f)] public float maxHP = 100f;
        [Min(0f)] public float maxMP = 25f;
        [Min(1f)] public float habilidade = 10f;
        [Min(0f)] public float bonusClasse = 0f;
        [Min(0f)] public float bonusEquipamento = 0f;
        [Min(0f)] public float penalidadeArmadura = 0f;
        [Min(0f)] public float penalidadePeso = 0f;
        [Min(0f)] public float ataque = 10f;
        [Min(0f)] public float defesa = 5f;
        [Min(0f)] public float poderMagico = 8f;
    }
}
