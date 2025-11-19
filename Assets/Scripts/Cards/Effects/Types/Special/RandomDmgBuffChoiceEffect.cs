using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Card/Effects/Special/RandomDmgBuffChoiceEffect")]
public class RandomDmgBuffChoiceEffect : CardEffect
{
    [SerializeField] private int damageToDeal;
    [SerializeField] private int powerToAdd;

    public void Initialize(int damage, int power)
    {
        this.damageToDeal = damage;
        this.powerToAdd = power;
    }

    public override void ActivateEffect(GameController game, CardInstance source)
    {
        Debug.Log($"Aktywacja efektu: {effectName}. {source.data.cardName} wybiera zaklêcie.");

        List<CardInstance> allCards = new List<CardInstance>();
        allCards.AddRange(game.playerBoard);
        allCards.AddRange(game.enemyBoard);

        List<CardInstance> validTargets = new List<CardInstance>();
        foreach (var card in allCards)
        {
            if (card.currentPower >= 0) validTargets.Add(card);
        }

        if (validTargets.Count == 0)
        {
            Debug.Log("[Effect] Pusty stó³. Radagast jest bezradny.");
            return;
        }

        int randomIndex = Random.Range(0, validTargets.Count);
        CardInstance victim = validTargets[randomIndex];

        bool dealDamage = Random.value > 0.5f;

        if (dealDamage)
        {
            Debug.Log($"[Effect] Radagast trafi³ {victim.data.cardName}, ups.");
            victim.TakeDamage(damageToDeal);
        }
        else
        {
            Debug.Log($"[Effect] Radagast uleczy³ {victim.data.cardName}, ups?");
            victim.AddPower(powerToAdd);
        }

        game.UpdateUI();
    }
}
