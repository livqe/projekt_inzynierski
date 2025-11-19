using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

[CreateAssetMenu(menuName = "Card/Effects/Buff/BuffAllyOnSelfDeathEffect")]
public class BuffAllyOnSelfDeathEffect : CardEffect, IOnDeathEffect
{
    [SerializeField] private int powerToAdd;
    [SerializeField] private string allyToBuff;

    public void Initialize(int powerAmount, string ally)
    {
        this.powerToAdd = powerAmount;
        this.allyToBuff = ally;
    }

    public override void ActivateEffect(GameController game, CardInstance source) {}

    public void OnDeath(GameController game, CardInstance source)
    {
        Debug.Log($"Aktywowano efekt: {effectName}. {source.data.cardName} ginie i dodaje {allyToBuff} +{powerToAdd} mocy.");
        var cardBoard = (source.owner == game.player) ? game.playerBoard : game.enemyBoard;

        foreach (var card in cardBoard)
        {
            if (card.data.cardName == allyToBuff && card.currentPower >= 0)
            {
                card.AddPower(powerToAdd);
                game.UpdateUI();
                break;
            }
        }
    }
}
