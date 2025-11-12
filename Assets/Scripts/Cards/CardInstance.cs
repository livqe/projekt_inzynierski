using UnityEngine;

[System.Serializable]
public class CardInstance
{
    public CardData data;

    [System.NonSerialized]
    public Player owner;
    
    public int currentPower;
    public int shield;
    public bool isImunne;

    public Faction Faction => data.faction;
    public string Name => data.cardName;

    public CardInstance(CardData cardData)
    {
        this.data = cardData;
        this.currentPower = cardData.power;
        this.shield = 0;
        this.isImunne = false;
    }

    public void AddShield(int amount)
    {
        shield += amount;
        Debug.Log($"{data.cardName} otrzymuje {amount} tarczy. Razem: {shield}.");

        //tutaj update UI
    }

    public void AddPower(int amount)
    {
        currentPower += amount;
        if (currentPower < 0) currentPower = 0;
        Debug.Log($"{data.cardName} otrzymuje +{amount} mocy, nowa wartoœæ: {currentPower}.");
    }

    public void TakeDamage(int amount)
    {
        if (isImunne)
        {
            Debug.Log($"{data.cardName} jest odporny na obra¿enia.");
            return;
        }

        if (shield > 0)
        {
            int damageToShield = Mathf.Min(shield, amount);
            shield -= damageToShield;
            amount -= damageToShield;
            Debug.Log($"{data.cardName} tarcza poch³onê³a {damageToShield} obra¿eñ. Pozosta³a tarcza: {shield}.");
        }

        if (amount > 0)
        {
            currentPower -= amount;
            Debug.Log($"{data.cardName} otrzymuje {amount} obra¿eñ. Pozosta³a moc: {currentPower}.");
        }

        if (currentPower <=0)
        {
            Die();
        }
    }

    private void Die()
    {
        currentPower = 0;
        Debug.Log($"{data.cardName} umiera.");

        //tutaj usuwanie karty z planszy
    }
}
