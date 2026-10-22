using Game01.Combat.Actions;
using Game01.Combat.Core;
using System;

namespace Game01.Combat.Systems
{
    public sealed class CombatResolver
    {
        private readonly DamageSystem _damageSystem = new DamageSystem();

        public CombatActionResult Resolve(CombatActionCommand command)
        {
            if (command == null || command.Actor == null || command.Target == null || command.Action == null
                || !command.Actor.CanAct() || command.Target.IsDefeated
                || command.Action.manaCost < 0f || command.Actor.CurrentMP < command.Action.manaCost)
            {
                throw new InvalidOperationException("Invalid combat command or insufficient MP.");
            }

            if (command.Action.actionType != CombatActionType.BasicAttack
                && command.Action.actionType != CombatActionType.Technique
                && command.Action.actionType != CombatActionType.Magic)
            {
                throw new NotSupportedException("This action has no damage implementation.");
            }

            var actorStats = command.Actor.Definition.stats;
            var targetStats = command.Target.Definition.stats;
            var power = command.Action.actionType == CombatActionType.Magic ? actorStats.poderMagico : actorStats.ataque;
            var damage = _damageSystem.CalculateBasicDamage(power, targetStats.defesa, command.Action.basePower);

            if (command.Target.IsDefending)
            {
                damage *= 0.5f;
            }

            command.Target.ApplyDamage(damage);
            command.Actor.SpendMP(command.Action.manaCost);
            command.Actor.ConsumeTimeCost(command.Action.timeCost);

            return new CombatActionResult
            {
                DidHit = true,
                DamageAmount = damage,
                ManaSpent = command.Action.manaCost,
                TimeCostApplied = command.Action.timeCost,
                TargetDefeated = command.Target.IsDefeated,
            };
        }
    }
}
