using System.Collections.Generic;
using Game01.Combat.Core;
using Game01.Combat.Data;
using UnityEditor;
using UnityEngine;

namespace Game01.Combat.Editor
{
    public static class CombatPrototypeAssetGenerator
    {
        private const string RootFolder = "Assets/GameData";
        private const string CombatFolder = "Assets/GameData/Combat";
        private const string ActionsFolder = "Assets/GameData/Combat/Actions";
        private const string CombatantsFolder = "Assets/GameData/Combat/Combatants";
        private const string ConfigFolder = "Assets/GameData/Combat/Configs";

        [MenuItem("GAME_01/Combat/Generate Prototype Assets")]
        public static void GeneratePrototypeAssets()
        {
            EnsureFolders();

            var attack = CreateAction(
                "Action_BasicAttack",
                "action.basic_attack",
                "Ataque Basico",
                CombatActionType.BasicAttack,
                TargetType.SingleEnemy,
                100f,
                0f,
                10f);

            var defend = CreateAction(
                "Action_Defend",
                "action.defend",
                "Defender",
                CombatActionType.Defend,
                TargetType.Self,
                80f,
                0f,
                0f);

            var wait = CreateAction(
                "Action_Wait",
                "action.wait",
                "Esperar",
                CombatActionType.Wait,
                TargetType.Self,
                0f,
                0f,
                0f);

            var passTurn = CreateAction(
                "Action_PassTurn",
                "action.pass_turn",
                "Passar Turno",
                CombatActionType.PassTurn,
                TargetType.Self,
                100f,
                0f,
                0f);

            CreateBattleConfig();

            var heavy = CreateAction("Action_HeavyAttack", "action.heavy_attack", "Golpe Pesado",
                CombatActionType.Technique, TargetType.SingleEnemy, 150f, 0f, 24f);
            var magic = CreateAction("Action_Magic", "action.magic", "Faisca", CombatActionType.Magic,
                TargetType.SingleEnemy, 120f, 8f, 26f);
            var sharedPlayerActions = new List<CombatActionDefinition> { attack, heavy, magic, defend, wait, passTurn };
            var sharedEnemyActions = new List<CombatActionDefinition> { attack, defend };

            CreateCombatant(
                "Combatant_Hero",
                "combatant.hero",
                "Heroi",
                120f,
                20f,
                12f,
                2f,
                1f,
                1f,
                12f,
                6f,
                6f,
                sharedPlayerActions);

            CreateCombatant(
                "Combatant_Mage",
                "combatant.mage",
                "Mago",
                80f,
                40f,
                13f,
                0f,
                0f,
                0f,
                8f,
                4f,
                12f,
                sharedPlayerActions);

            CreateCombatant(
                "Combatant_Wolf",
                "combatant.wolf",
                "Lobo",
                75f,
                0f,
                14f,
                0f,
                0f,
                0f,
                10f,
                3f,
                0f,
                sharedEnemyActions);

            CreateCombatant(
                "Combatant_Bandit",
                "combatant.bandit",
                "Bandido",
                95f,
                0f,
                11f,
                0f,
                0f,
                0f,
                11f,
                5f,
                0f,
                sharedEnemyActions);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            if (!Application.isBatchMode) EditorUtility.DisplayDialog(
                "GAME_01",
                "Assets base do prototipo de combate foram gerados em Assets/GameData/Combat.",
                "OK");
        }

        private static void EnsureFolders()
        {
            CreateFolderIfMissing("Assets", "GameData");
            CreateFolderIfMissing(RootFolder, "Combat");
            CreateFolderIfMissing(CombatFolder, "Actions");
            CreateFolderIfMissing(CombatFolder, "Combatants");
            CreateFolderIfMissing(CombatFolder, "Configs");
        }

        private static void CreateFolderIfMissing(string parent, string child)
        {
            var fullPath = $"{parent}/{child}";
            if (!AssetDatabase.IsValidFolder(fullPath))
            {
                AssetDatabase.CreateFolder(parent, child);
            }
        }

        private static CombatActionDefinition CreateAction(
            string assetName,
            string actionId,
            string displayName,
            CombatActionType actionType,
            TargetType targetType,
            float timeCost,
            float manaCost,
            float basePower)
        {
            var path = $"{ActionsFolder}/{assetName}.asset";
            var asset = AssetDatabase.LoadAssetAtPath<CombatActionDefinition>(path);
            if (asset != null) return asset;
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<CombatActionDefinition>();
                AssetDatabase.CreateAsset(asset, path);
            }

            asset.actionId = actionId;
            asset.displayName = displayName;
            asset.actionType = actionType;
            asset.targetType = targetType;
            asset.timeCost = timeCost;
            asset.manaCost = manaCost;
            asset.basePower = basePower;
            EditorUtility.SetDirty(asset);
            return asset;
        }

        private static void CreateBattleConfig()
        {
            var path = $"{ConfigFolder}/BattleConfig_Prototype.asset";
            var asset = AssetDatabase.LoadAssetAtPath<BattleConfig>(path);
            if (asset != null) return;
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<BattleConfig>();
                AssetDatabase.CreateAsset(asset, path);
            }

            asset.atbThreshold = CombatConstants.DefaultAtbThreshold;
            asset.tickScale = CombatConstants.DefaultTickScale;
            asset.decisionTimeLimit = CombatConstants.DefaultDecisionTimeLimit;
            asset.pauseTimelineDuringPlayerDecision = true;
            EditorUtility.SetDirty(asset);
        }

        private static void CreateCombatant(
            string assetName,
            string combatantId,
            string displayName,
            float maxHp,
            float maxMp,
            float habilidade,
            float bonusClasse,
            float bonusEquipamento,
            float penalidadeArmadura,
            float ataque,
            float defesa,
            float poderMagico,
            List<CombatActionDefinition> actions)
        {
            var path = $"{CombatantsFolder}/{assetName}.asset";
            var asset = AssetDatabase.LoadAssetAtPath<CombatantDefinition>(path);
            if (asset != null) return;
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<CombatantDefinition>();
                AssetDatabase.CreateAsset(asset, path);
            }

            asset.combatantId = combatantId;
            asset.displayName = displayName;
            asset.stats.maxHP = maxHp;
            asset.stats.maxMP = maxMp;
            asset.stats.habilidade = habilidade;
            asset.stats.bonusClasse = bonusClasse;
            asset.stats.bonusEquipamento = bonusEquipamento;
            asset.stats.penalidadeArmadura = penalidadeArmadura;
            asset.stats.penalidadePeso = 0f;
            asset.stats.ataque = ataque;
            asset.stats.defesa = defesa;
            asset.stats.poderMagico = poderMagico;
            asset.availableActions = actions;
            EditorUtility.SetDirty(asset);
        }
    }
}
