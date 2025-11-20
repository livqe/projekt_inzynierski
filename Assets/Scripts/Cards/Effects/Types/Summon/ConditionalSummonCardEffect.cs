using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Card/Effects/Summon/ConditionalSummonCardEffect")]
public class ConditionalSummonCardEffect : CardEffect, IOnOtherCardPlayedEffect
{
    [SerializeField] private string cardToSummon;
    [SerializeField] private string requiredAlly;

    public void Initialize(string cardName, string allyName)
    {
        this.cardToSummon = cardName;
        this.requiredAlly = allyName;
    }

    public override void ActivateEffect(GameController game, CardInstance source)
    {
        Summon(game, source);
    }

    public void OnOtherCardPlayed(GameController game, CardInstance source, CardInstance playedCard)
    {
        if (source.effectTriggered) return;

        if (playedCard.data.cardName == requiredAlly && playedCard.owner == source.owner)
        {
            Summon(game, source);
        }
    }

    private void Summon(GameController game, CardInstance source)
    {
        if (source.effectTriggered) return;

        var cardBoard = (source.owner == game.player) ? game.playerBoard : game.enemyBoard;

        bool conditionMet = false;
        foreach (var card in cardBoard)
        {
            if (card.data.cardName == requiredAlly && card.currentPower >= 0)
            {
                conditionMet = true;
                break;
            }
        }

        if (conditionMet)
        {
            Debug.Log($"Aktywacja efektu: {effectName}. {requiredAlly} jest na planszy, {source.data.cardName} przyzywa {cardToSummon}.");

            Player owner = source.owner;
            List<CardInstance> toSummon = new List<CardInstance>();

            foreach (var card in owner.cardsInDeck)
            {
                if (card.data.cardName == cardToSummon) toSummon.Add(card);
            }

            foreach (var card in toSummon)
            {
                owner.cardsInDeck.Remove(card);
                game.PlayCard(card, (owner == game.player));
            }

            source.effectTriggered = true;
        }
    }
}
