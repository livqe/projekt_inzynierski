using UnityEngine;

public static class EffectFactory
{
    public static CardEffect CreateEffectFromName(string effectName, string effectParams)
    {
        if (string.IsNullOrEmpty(effectName))
            return null;

        string[] parameters = effectParams.Split("|");
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
                buffSelfOnAllyDeathEffect.Initialize(int.Parse(parameters[0]), parameters[1]);
                createdEffect = buffSelfOnAllyDeathEffect;
                break;

            case "ConditionalBuffSelfEffect":
                var conditionalBuffSelfEffect = ScriptableObject.CreateInstance<ConditionalBuffSelfEffect>();
                string[] allies = new string[parameters.Length - 1];
                System.Array.Copy(parameters, 1, allies, 0, parameters.Length - 1);
                conditionalBuffSelfEffect.Initialize(int.Parse(parameters[0]), allies);
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
                var conditionalBuffRowEffect = ScriptableObject.CreateInstance<ConditionalBuffRowEffect>();
                conditionalBuffRowEffect.Initialize(int.Parse(parameters[0]), parameters[1]);
                createdEffect = conditionalBuffRowEffect;
                break;

            // === KATEGORIA: DAMAGE ===
            case "PeriodicDamageEffect":
                var periodicDamageEffect = ScriptableObject.CreateInstance<PeriodicDamageEffect>();
                periodicDamageEffect.Initialize(int.Parse(parameters[0]), int.Parse(parameters[1]));
                createdEffect = periodicDamageEffect;
                break;

            case "DamageTargetEnemyEffect":
                var damageTargetEffect = ScriptableObject.CreateInstance<DamageTargetEnemyEffect>();
                damageTargetEffect.Initialize(int.Parse(parameters[0]));
                createdEffect = damageTargetEffect;
                break;

            case "MutualDamageLinkEffect":
                var mutualDamageEffect = ScriptableObject.CreateInstance<MutualDamageLinkEffect>();
                mutualDamageEffect.Initialize(int.Parse(parameters[0]), parameters[1]);
                createdEffect = mutualDamageEffect;
                break;

            case "RowDamageEffect":
                var rowDamageEffect = ScriptableObject.CreateInstance<RowDamageEffect>();
                rowDamageEffect.Initialize(int.Parse(parameters[0]));
                createdEffect = rowDamageEffect;
                break;

            // === KATEGORIA: SPECIAL ===
            case "DeathLinkEffect":
                var deathLinkEffect = ScriptableObject.CreateInstance<DeathLinkEffect>();
                deathLinkEffect.Initialize(parameters[0]);
                createdEffect = deathLinkEffect;
                break;

            case "MutualDestructionEffect":
                var mutualDestructionEffect = ScriptableObject.CreateInstance<MutualDestructionEffect>();
                mutualDestructionEffect.Initialize(parameters[0]);
                createdEffect = mutualDestructionEffect;
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
                var randomDmgBuffChoiceEffect = ScriptableObject.CreateInstance<RandomDmgBuffChoiceEffect>();
                randomDmgBuffChoiceEffect.Initialize(int.Parse(parameters[0]), int.Parse(parameters[1]));
                createdEffect = randomDmgBuffChoiceEffect;
                break;

            // === KATEGORIA: SUMMON ===
            case "SummonCardEffect":
                var summonCardEffect = ScriptableObject.CreateInstance<SummonCardEffect>();
                summonCardEffect.Initialize(parameters[0], parameters[1]);
                createdEffect = summonCardEffect;
                break;

            case "ConditionalSummonCardEffect":
                var conditionalSummonEffect = ScriptableObject.CreateInstance<ConditionalSummonCardEffect>();
                conditionalSummonEffect.Initialize(parameters[0], parameters[1]);
                createdEffect = conditionalSummonEffect;
                break;

            default:
                Debug.LogWarning($"[EffectFactory] Nie rozpoznano nazwy efektu: {effectName}. Karta nie otrzyma³a efektu.");
                createdEffect = null;
                break;
        }

        return createdEffect;
    }
}
