using UnityEngine;

[CreateAssetMenu(menuName = "Card/Effects/Damage/RowDamageEffect")]
public class RowDamageEffect : CardEffect, IRowTargetableEffect
{
    [SerializeField] private int damageToDeal;

    public void Initialize(int damageAmount)
    {
        this.damageToDeal = damageAmount;
    }

    public override void ActivateEffect(GameController game, CardInstance source)
    {
        Debug.Log($"Aktywacja efektu: {effectName}. {source.data.cardName} czeka na wybranie rzêdu.");
        game.StartTargeting(source, this);
    }

    public void ExecuteWithRowTarget(RangeType range, bool isPlayerRow)
    {
        Debug.Log($"[Effect] Atakowany rz¹d: {range}.");

        var targetBoard = isPlayerRow ? GameController.Instance.playerBoard : GameController.Instance.enemyBoard;

        foreach ( var card in targetBoard )
        {
            if (card.data.range == range && card.currentPower >= 0)
            {
                card.TakeDamage(damageToDeal);
            }
        }
    }
}
