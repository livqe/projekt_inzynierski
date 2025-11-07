using UnityEngine;

[CreateAssetMenu(menuName = "Card/Effects/Damage/RowDamageEffect")]
public class RowDamageEffect : CardEffect
{
    [SerializeField] private int damageToDeal;

    public void Initialize(int damageAmount)
    {
        this.damageToDeal = damageAmount;
    }

    public override void ActivateEffect(GameController game, CardInstance source)
    {
        Debug.Log($"Aktywacja efektu: {effectName}. {source.data.cardName} zadaje -{damageToDeal} wybranemu rzêdowi.");

        //logika karty tutaj
    }
}
