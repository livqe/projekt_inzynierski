using UnityEngine;

[CreateAssetMenu(menuName = "Card/Effects/Damage/DamageTargetEnemyEffect")]
public class DamageTargetEnemyEffect : CardEffect, ITargetableEffect
{
    [SerializeField] private int damageToDeal;

    public void Initialize(int damageAmount)
    {
        this.damageToDeal = damageAmount;
    }

    public override void ActivateEffect(GameController game, CardInstance source)
    {
        Debug.Log($"Aktywacja efektu: {effectName}. {source.data.cardName} ¿¹da wyboru celu.");

        game.StartTargeting(source, this);
    }

    public void ExecuteWithTarget(CardInstance target)
    {
        Debug.Log($"[Effect] Zadajê {damageToDeal} obra¿eñ {target.data.cardName}.");
        target.TakeDamage(damageToDeal);
    }
}
