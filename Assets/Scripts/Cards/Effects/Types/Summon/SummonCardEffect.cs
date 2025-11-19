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

        if (amountToSummon == "All")
        {
            foreach (CardInstance card in owner.cardsInDeck)
            {
                if (card.data.cardName == cardToSummon)
                {
                    cardsToSummon.Add(card);
                }
            }
        }
        else
        {
            int count = int.Parse(amountToSummon);
            int found = 0;
            foreach (CardInstance card in owner.cardsInDeck)
            {
                if (card.data.cardName == cardToSummon)
                {
                    cardsToSummon.Add(card);
                    found++;
                    if (found >= count) break;
                }
            }
        }

        if (cardsToSummon.Count > 0)
        {
            foreach (CardInstance card in cardsToSummon)
            {
                Debug.Log($"[Effect] Przyzywanie {card.data.cardName} z talii.");

                owner.cardsInDeck.Remove(card);
                game.PlayCard(card, (owner == game.player));
            }
        }
        else
        {
            Debug.Log($"[Effect] Nie znaleziono karty {cardToSummon} w talii.");
        }
    }
}
