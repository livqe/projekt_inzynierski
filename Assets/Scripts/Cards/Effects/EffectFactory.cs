using NUnit.Framework;
using UnityEngine;

public static class EffectFactory
{
    public static CardEffect CreateEffectFromName(string effectName, string effectParams)
    {
        if (string.IsNullOrEmpty(effectName))
            return null;

        string[] parameters = effectParams.Split(";");
        CardEffect createdEffect = null;

        switch (effectName)
        {
            // === KATEGORIA: BUFF ===
            case "AddShieldEffect":
                var shieldEffect = ScriptableObject.CreateInstance<AddShieldEffect>();
                shieldEffect.Initialize(int.Parse(parameters[0]));
                createdEffect = shieldEffect;
                break;

            case "BuffSelfEffect":
                var buffSelfEffect = ScriptableObject.CreateInstance<BuffSelfEffect>();
                buffSelfEffect.Initialize(int.Parse(parameters[0]), int.Parse(parameters[1]));
                createdEffect = buffSelfEffect;
                break;

            case "BuffTargetEffect":
                var buffTargetEffect = ScriptableObject.CreateInstance<BuffTargetEffect>();
                buffTargetEffect.Initialize(int.Parse(parameters[0]), int.Parse(parameters[1]));
                createdEffect = buffTargetEffect;
                break;

            case "BuffSelfOnAllyDeathEffect":
                var buffSelfOnAllyDeathEffect = ScriptableObject.CreateInstance<BuffSelfOnAllyDeathEffect>();
                buffSelfOnAllyDeathEffect.Initialize(int.Parse(parameters[0]), int.Parse(parameters[1]));
                createdEffect = buffSelfOnAllyDeathEffect;
                break;

            case "ConditionalBuffSelfEffect":
                var conditionalBuffSelfEffect = ScriptableObject.CreateInstance<ConditionalBuffSelfEffect>();
                conditionalBuffSelfEffect.Initialize(int.Parse(parameters[0]), parameters[1]);
                createdEffect = conditionalBuffSelfEffect;
                break;

            case "BuffRowEffect":
                var buffRowEffect = ScriptableObject.CreateInstance<BuffRowEffect>();
                buffRowEffect.Initialize(int.Parse(parameters[0]));
                createdEffect = buffRowEffect;
                break;

            case "BuffAllyOnSelfDeathEffect":
                var buffAllyOnSelfDeathEffect = ScriptableObject.CreateInstance<BuffAllyOnSelfDeathEffect>();
                buffAllyOnSelfDeathEffect.Initialize(int.Parse(parameters[0]), parameters[1]);
                createdEffect = buffAllyOnSelfDeathEffect;
                break;

            case "BuffRandomAllyEffect":
                var buffRandomEffect = ScriptableObject.CreateInstance<BuffRandomAllyEffect>();
                buffRandomEffect.Initialize(int.Parse(parameters[0]));
                createdEffect = buffRandomEffect;
                break;

            case "ConditionalBuffRowEffect":
                var condBuffRowEffect = ScriptableObject.CreateInstance<ConditionalBuffRowEffect>();
                condBuffRowEffect.Initialize(int.Parse(parameters[0]), parameters[1]);
                createdEffect = condBuffRowEffect;
                break;

            // === KATEGORIA: DAMAGE ===
            case "PeriodicDamageEffect":
                var periodicDmgEffect = ScriptableObject.CreateInstance<PeriodicDamageEffect>();
                periodicDmgEffect.Initialize(int.Parse(parameters[0]), int.Parse(parameters[1]));
                createdEffect = periodicDmgEffect;
                break;

            case "DamageTargetEnemyEffect":
                var dmgTargetEffect = ScriptableObject.CreateInstance<DamageTargetEnemyEffect>();
                dmgTargetEffect.Initialize(int.Parse(parameters[0]));
                createdEffect = dmgTargetEffect;
                break;

            case "MutualDamageLinkEffect":
                var mutualDmgEffect = ScriptableObject.CreateInstance<MutualDamageLinkEffect>();
                mutualDmgEffect.Initialize(int.Parse(parameters[0]), parameters[1]);
                createdEffect = mutualDmgEffect;
                break;

            case "RowDamageEffect":
                var rowDmgEffect = ScriptableObject.CreateInstance<RowDamageEffect>();
                rowDmgEffect.Initialize(int.Parse(parameters[0]));
                createdEffect = rowDmgEffect;
                break;

            // === KATEGORIA: SPECIAL ===
            case "DeathLinkEffect":
                var deathLinkEffect = ScriptableObject.CreateInstance<DeathLinkEffect>();
                deathLinkEffect.Initialize(parameters[0]);
                createdEffect = deathLinkEffect;
                break;

            case "MutualDestructionEffect":
                var mutualDestrEffect = ScriptableObject.CreateInstance<MutualDestructionEffect>();
                mutualDestrEffect.Initialize(parameters[0]);
                createdEffect = mutualDestrEffect;
                break;

            case "RandomPowerEffect":
                var randomPowerEffect = ScriptableObject.CreateInstance<RandomPowerEffect>();
                randomPowerEffect.Initialize(int.Parse(parameters[0]), int.Parse(parameters[1]));
                createdEffect = randomPowerEffect;
                break;

            case "ImmuneToDamageEffect":
                createdEffect = ScriptableObject.CreateInstance<ImmuneToDamageEffect>();
                break;

            case "SpyEffect":
                var spyEffect = ScriptableObject.CreateInstance<SpyEffect>();
                spyEffect.Initialize(int.Parse(parameters[0]));
                createdEffect = spyEffect;
                break;

            case "ChanceAutoPlayEffect":
                var autoPlayEffect = ScriptableObject.CreateInstance<ChanceAutoPlayEffect>();
                autoPlayEffect.Initialize(int.Parse(parameters[0]), int.Parse(parameters[1]));
                createdEffect = autoPlayEffect;
                break;

            case "RandomDmgBuffChoiceEffect":
                var randomChoiceEffect = ScriptableObject.CreateInstance<RandomDmgBuffChoiceEffect>();
                randomChoiceEffect.Initialize(int.Parse(parameters[0]), int.Parse(parameters[1]));
                createdEffect = randomChoiceEffect;
                break;

            // === KATEGORIA: SUMMON ===
            case "SummonCardEffect":
                var summonEffect = ScriptableObject.CreateInstance<SummonCardEffect>();
                summonEffect.Initialize(parameters[0], parameters[1]);
                createdEffect = summonEffect;
                break;

            case "ConditionalSummonCardEffect":
                var condSummonEffect = ScriptableObject.CreateInstance<ConditionalSummonCardEffect>();
                condSummonEffect.Initialize(parameters[0], parameters[1]);
                createdEffect = condSummonEffect;
                break;


            default:
                Debug.LogWarning($"[EffectFactory] Nie rozpoznano nazwy efektu: {effectName}. Karta nie otrzyma³a efektu.");
                createdEffect = null;
                break;
        }

        return createdEffect;
    }
}
