using UnityEngine;

public abstract class FactionAbility : ScriptableObject
{
    [Header("Opis mocy frakcyjnej")]
    public string abilityName;
    [TextArea] public string description;

    public abstract void OnRoundEnd(GameController game, Player owner);
}
