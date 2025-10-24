using UnityEngine;

[System.Serializable]
public class CardInstance
{
    public CardData data;
    public Player Owner;
    public int CurrentPower;

    public Faction Faction => data.faction;
    public string Name => data.cardName;

    public CardInstance(CardData cardData, Player owner)
    {
        data = cardData;
        Owner = owner;
        CurrentPower = cardData.power;
    }

    public void AddPower(int amount)
    {
        CurrentPower += amount;
        if (CurrentPower < 0) CurrentPower = 0;
        Debug.Log($"{data.cardName} zmienia si³ê o {amount}, nowa wartoœæ: {CurrentPower}");
    }
}
