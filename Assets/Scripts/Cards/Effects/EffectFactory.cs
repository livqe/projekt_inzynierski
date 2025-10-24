using UnityEngine;

public static class EffectFactory
{
    public static CardEffect CreateEffectFromName(string effectName)
    {
        if (string.IsNullOrEmpty(effectName))
            return null;

        effectName = effectName.ToLower();

        if (effectName.Contains(""))
            return ScriptableObject.CreateInstance<AddPowerEffect>();
        else if (effectName.Contains(""))
            return ScriptableObject.CreateInstance<AddPowerToRandomEffect>();


        Debug.LogWarning($"Nieznany efekt: {effectName}");
        return null;
    }
}
