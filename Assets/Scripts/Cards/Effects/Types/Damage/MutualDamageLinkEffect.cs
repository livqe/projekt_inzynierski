using UnityEngine;

[CreateAssetMenu(menuName = "Card/Effects/Damage/MutualDamageLinkEffect")]
public class MutualDamageLinkEffect : CardEffect, IOnOtherCardPlayedEffect
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
        AttackIfPresent(game, source);
    }

    public void OnOtherCardPlayed(GameController game, CardInstance source, CardInstance playedCard)
    {
        if (playedCard.data.cardName == linkedCard && playedCard.owner != source.owner)
        {
            playedCard.TakeDamage(damageToDeal);
            game.UpdateUI();
        }
    }

    private void AttackIfPresent(GameController game, CardInstance source)
    {
        var cardBoard = (source.owner == game.player) ? game.enemyBoard : game.playerBoard;

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
