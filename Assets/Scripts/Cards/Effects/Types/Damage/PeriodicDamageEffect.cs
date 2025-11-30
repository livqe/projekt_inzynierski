using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Card/Effects/Damage/PeriodicDamageEffect")]
public class PeriodicDamageEffect : CardEffect, IOnTurnEndEffect
{
    [SerializeField] private int damageToDeal;
    [SerializeField] private int turnFrequency;

    public void Initialize(int damageAmount, int frequency)
    {
        this.damageToDeal = damageAmount;
        this.turnFrequency = frequency;
    }

    public override void ActivateEffect(GameController game, CardInstance source)
    {
        Debug.Log($"Aktywacja efektu: {effectName}. {source.data.cardName} zostaje zagrany, aktywuje siê co {turnFrequency} tur.");
        source.effectTurnCounter = 1;
    }

    public void OnTurnEnd(GameController game, CardInstance source)
    {
        if (source.currentPower <= 0 && !source.survivor) return;

        if (source.effectTurnCounter >= turnFrequency)
        {
            Debug.Log($"[Effect] {source.data.cardName} atakuje");

            List<CardInstance> targetBoard = (source.owner == game.player) ? game.enemyBoard : game.playerBoard;

            if (targetBoard.Count > 0)
            {
                CardInstance randomTarget = GetRandomTarget(targetBoard);
                if (randomTarget != null)
                {
                    Debug.Log($"[Effect] {source.data.cardName} trafia {randomTarget.data.cardName} i zadaje -{damageToDeal} obra¿eñ.");
                    randomTarget.TakeDamage(damageToDeal);
                }
            }
            else
            {
                Debug.Log("[Effect] Brak celów dla efektu okresowego.");
            }

            source.effectTurnCounter = 0;
        }
        else
        {
            source.effectTurnCounter++;
            Debug.Log($"[Effect] {source.data.cardName} ³aduje atak: {source.effectTurnCounter}/{turnFrequency}");
        }
    }

    private CardInstance GetRandomTarget(List<CardInstance> board)
    {
        List<CardInstance> validTargets = new List<CardInstance>();
        foreach (var card in board)
        {
            if (card.currentPower > 0 && !card.isImunne) validTargets.Add(card);
        }

        if (validTargets.Count == 0) return null;

        int index = Random.Range(0, validTargets.Count);
        return validTargets[index];
    }
}
