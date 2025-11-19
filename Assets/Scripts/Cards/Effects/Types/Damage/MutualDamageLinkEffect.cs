using UnityEngine;

[CreateAssetMenu(menuName = "Card/Effects/Damage/MutualDamageLinkEffect")]
public class MutualDamageLinkEffect : CardEffect
{
    [SerializeField] private int damageToDeal;
    [SerializeField] private string linkedCard;

    public void Initialize(int damageAmount, string card)
    {
        this.damageToDeal = damageAmount;
        this.linkedCard = card;
    }

    public override void ActivateEffect(GameController game, CardInstance source)
    {
        var cardBoard = (source.owner == game.player) ? game.playerBoard : game.enemyBoard;

        foreach (var card in cardBoard)
        {
            if (card.data.cardName == linkedCard && card.currentPower >= 0)
            {
                Debug.Log($"Aktywacja efektu: {effectName}. {source.data.cardName} zadaje -{damageToDeal} po³¹czonej karcie {linkedCard}.");
                card.TakeDamage(damageToDeal);
                game.UpdateUI();
                return;
            }
        }
    }
}
