using UnityEngine;

public abstract class CardEffect : ScriptableObject
{
    [Header("Opis efektu")]
    public string effectName;
    [TextArea] public string effectDescription;

    public abstract void ActivateEffect(GameController game, CardInstance source);
}
