using UnityEngine;

[CreateAssetMenu(menuName = "Card/Effects/Damage/PeriodicDamageEffect")]
public class PeriodicDamageEffect : CardEffect
{
    [SerializeField] private int damageToDeal;
    [SerializeField] private int turnFrequency;

    public void Initialize(int damageAmount, int frequency)
    {
        this.damageToDeal = damageAmount;
        this.turnFrequency = frequency;
    }

    public override void ActivateEffect(GameController game, CardInstance source)
    {
        Debug.Log($"Aktywacja efektu: {effectName}. {source.data.cardName} zadaje -{damageToDeal} co {turnFrequency} tur.");

        //logika karty tutaj
    }
}
