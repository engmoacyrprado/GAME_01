using Game01.Combat.Data;

namespace Game01.Combat.Runtime
{
    public sealed class StatusEffectRuntime
    {
        public StatusEffectRuntime(StatusEffectDefinition definition)
        {
            Definition = definition;
        }

        public StatusEffectDefinition Definition { get; }
    }
}
