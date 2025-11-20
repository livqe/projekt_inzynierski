using UnityEngine;
using System.Collections.Generic;


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

    public void ExecuteWithRowTarget(CardInstance source, RangeType range, bool isPlayerRow)
    {
        Debug.Log($"[Effect] Atakowany rz¹d: {range}.");

        List<CardInstance> targets = new List<CardInstance>();

        if (source.data.cardName == "Tolkien")
        {

            foreach (var card in GameController.Instance.playerBoard)
            {
                if (card.data.range == range && card.currentPower >= 0) targets.Add(card);
            }

            foreach (var card in GameController.Instance.enemyBoard)
            {
                if (card.data.range == range && card.currentPower >= 0) targets.Add(card);
            }
        }
        else
        {
            List<CardInstance> targetBoard = isPlayerRow ? GameController.Instance.playerBoard : GameController.Instance.enemyBoard;

            foreach (var card in targetBoard)
            {
                if (card.data.range == range && card.currentPower >= 0)
                {
                    targets.Add(card);
                }
            }
        }

        if (targets.Count > 0)
        {
            foreach (var card in targets)
            {
                card.TakeDamage(damageToDeal);
            }
        }
        else
        {
            Debug.Log("[Effect] Rz¹d jest pusty.");
        }

            GameController.Instance.UpdateUI();
    }
}
