using UnityEngine;

public abstract class FactionAbility : ScriptableObject
{
    [Header("Opis mocy frakcyjnej")]
    public string abilityName;
    [TextArea] public string description;

    public virtual void OnTurnEnd(GameController game, Player owner) { }
    public virtual void OnRoundEnd(GameController game, Player owner) { }
}
