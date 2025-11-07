using UnityEngine;

[CreateAssetMenu(menuName = "Card/Effects/Damage/DamageTargetEnemyEffect")]
public class DamageTargetEnemyEffect : CardEffect
{
    [SerializeField] private int damageToDeal;

    public void Initialize(int damageAmount)
    {
        this.damageToDeal = damageAmount;
    }

    public override void ActivateEffect(GameController game, CardInstance source)
    {
        Debug.Log($"Aktywacja efektu: {effectName}. {source.data.cardName} zadaje -{damageToDeal} wybranemu celowi.");

        //logika karty tutaj
    }
}
