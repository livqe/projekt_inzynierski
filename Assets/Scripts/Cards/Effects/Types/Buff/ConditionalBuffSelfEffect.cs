using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(menuName = "Card/Effects/Buff/ConditionalBuffSelfEffect")]
public class ConditionalBuffSelfEffect : CardEffect, IOnOtherCardPlayedEffect
{
    [SerializeField] private int powerToAdd;
    [SerializeField] private List<string> alliesNames = new List<string>();

    public void Initialize(int powerAmount, string[] allies)
    {
        this.powerToAdd = powerAmount;
        this.alliesNames = allies.ToList();
    }

    public override void ActivateEffect(GameController game, CardInstance source)
    {
        var cardBoard = (source.owner == game.player) ? game.playerBoard : game.enemyBoard;

        bool allPresent = true;
        foreach (string reqName in alliesNames)
        {
            bool found = false;
            foreach (var card in cardBoard)
            {
                if (card.data.cardName == reqName && card.currentPower >= 0)
                {
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                allPresent = false;
                break;
            }
        }

        if (allPresent)
        {
            string alliesList = string.Join(", ", alliesNames);
            Debug.Log($"Aktywacja efektu: {effectName}. {alliesList} na planszy, {source.data.cardName} otrzymuje +{powerToAdd}.");
            source.AddPower(powerToAdd);
            game.UpdateUI();
        }
    }
}
