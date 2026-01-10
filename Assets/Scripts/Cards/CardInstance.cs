using System;
using UnityEngine;

[System.Serializable]
public class CardInstance
{
    public CardData data;

    [System.NonSerialized]
    public Player owner;
    
    [SerializeField] private int _currentPower;
    public Action<int> OnPowerChanged;

    [SerializeField] private int _shield;
    public Action OnShieldChanged;

    public int shield
    {
        get { return _shield; }
        set
        {
            if (_shield != value)
            {
                _shield = value;
                OnShieldChanged?.Invoke();
            }
        }
    }

    public bool isImunne;
    public Faction Faction => data.faction;
    public string Name => data.cardName;
    public bool survivor;
    public bool effectTriggered = false;
    public int effectTurnCounter = 0;
    public int basePowerFromEffect;


    public int currentPower
    {
        get { return _currentPower; }
        set 
        {
            int difference = value - _currentPower;
            _currentPower = value;

            if (difference != 0) OnPowerChanged?.Invoke(difference);
        }
    }

    public CardInstance(CardData cardData, Player owner)
    {
        this.data = cardData;
        this.owner = owner;
        this._currentPower = cardData.power;
        this._shield = Mathf.Max(0, cardData.baseShield);
        this.isImunne = false;
        this.effectTurnCounter = 0;

        if (data.power == 0)
        {
            this.survivor = true;
        }
        else
        {
            this.survivor = false;
        }

        this.effectTriggered = false;
        this.basePowerFromEffect = data.power;
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

        if (currentPower < 0)
        {
            Die();
        }
        else if (currentPower == 0 && !survivor)
        {
            Die();
        }
        else if (currentPower == 0 && survivor)
        {
            Debug.Log($"{data.cardName} ma 0 mocy, ale pozostaje na planszy.");
        }
    }

    private void Die()
    {
        currentPower = 0;
        Debug.Log($"{data.cardName} umiera.");

        GameController.Instance.OnCardDeath(this);
    }
}
