using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Card/Effects/Summon/SummonCardEffect")]
public class SummonCardEffect : CardEffect
{
    [SerializeField] private string cardToSummon;
    [SerializeField] private string amountToSummon;

    public void Initialize(string cardName, string amount)
    {
        this.cardToSummon = cardName;
        this.amountToSummon = amount;
    }

    public override void ActivateEffect(GameController game, CardInstance source)
    {
        Debug.Log($"Aktywacja efektu: {effectName}. {source.data.cardName} próbuje przyzywaæ {amountToSummon} kopii karty {cardToSummon}.");

        Player owner = source.owner;

        if (owner.cardsInDeck.Count == 0 )
        {
            Debug.Log("[Effect] Talia jest pusta. Nie ma co wezwaæ.");
            return;
        }

        List<CardInstance> cardsToSummon = new List<CardInstance>();
        int needed = (amountToSummon == "All") ? 999 : int.Parse(amountToSummon);

        foreach (var card in new List<CardInstance>(owner.cardsInDeck))
        {
            if (cardsToSummon.Count >= needed) break;
            if (card.data.cardName == cardToSummon)
            {
                cardsToSummon.Add(card);
                owner.cardsInDeck.Remove(card);
            }
        }

        if (cardsToSummon.Count < needed)
        {
            foreach (var card in new List<CardInstance>(owner.cardsInHand))
            {
                if (cardsToSummon.Count >= needed) break;
                if (card.data.cardName == cardToSummon)
                {
                    cardsToSummon.Add(card);
                    owner.cardsInHand.Remove(card);
                    game.handManager.RemoveCardVisual(card);
                }
            }
        }

        if (cardsToSummon.Count > 0)
        {
            foreach (var card in cardsToSummon)
            {
                Debug.Log($"[Effect] Przyzywanie {card.data.cardName} z talii.");

                game.PlayCard(card, (owner == game.player), false);
            }
        }
        else
        {
            Debug.Log($"[Effect] Nie znaleziono karty {cardToSummon} w talii.");
        }
    }
}
