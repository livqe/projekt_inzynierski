using UnityEngine;

public enum Faction
{
    Elfy,
    Krasnoludy,
    Neutralne
}

public enum RangeType
{
    Bliski,
    Daleki,
    Dowolny
}

[CreateAssetMenu(fileName = "NewCard", menuName = "Cards/CardData")]
public class CardData : ScriptableObject
{
    [Header("Base data")]
    public string cardName;
    public Sprite artwork;
    public Faction faction;
    public RangeType range;
    public int power;

    [Header("Visuals")]
    public string powerDisplayOverride;

    [Header("Effect")]
    public CardEffect effect;
    [TextArea] public string effectDescription;

    [Header("Deck building")]
    public int maxCopies = 1;
}
