using UnityEngine;

[CreateAssetMenu(menuName = "Card/Effects/Special/ChanceAutoPlayEffect")]
public class ChanceAutoPlayEffect : CardEffect
{
    [SerializeField] private int deckPercent;
    [SerializeField] private int handPercent;

    public void Initialize(int chanceFromDeck, int chanceFromHand)
    {
        this.deckPercent = chanceFromDeck;
        this.handPercent = chanceFromHand;
    }

    public override void ActivateEffect(GameController game, CardInstance source)
    {
        Debug.Log($"Aktywacja efektu: {effectName}. {source.data.cardName} ma {deckPercent}% na zagranie z talii i {handPercent}% z rêki.");
    }

    public bool TryAutoPlay(GameController game, CardInstance card, bool isInHand)
    {
        int chance = isInHand ? handPercent : deckPercent;
        int roll = Random.Range(0, 100);

        if (roll < chance)
        {
            Debug.Log($"[Effect] Wylosowano {roll} ({chance}%), Gandalf wchodzi.");

            if (isInHand)
            {
                game.handManager.RemoveCardVisual(card);
                card.owner.cardsInHand.Remove(card);
            }
            else
            {
                card.owner.cardsInDeck.Remove(card);
            }

            game.PlayCard(card, (card.owner == game.player), false);
            return true;
        }
        
        return false;
    }
}
