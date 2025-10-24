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
    [Header("Podstawowe dane")]
    public string cardName;
    public Sprite artwork;
    public Faction faction;
    public RangeType range;
    public int power;

    [Header("Efekt specjalny")]
    public CardEffect effect;
    [TextArea] public string effectDescription;
}
