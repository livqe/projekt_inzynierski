using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(menuName = "Card/Effects/Buff/BuffRandomAllyEffect")]
public class BuffRandomAllyEffect :CardEffect
{
    [SerializeField] private int powerToAdd;

    public void Initialize(int powerAmount)
    {
        this.powerToAdd = powerAmount;
    }

    public override void ActivateEffect(GameController game, CardInstance source)
    {
        var cardBoard = (source.owner == game.player) ? game.playerBoard : game.enemyBoard;

        var candidates = cardBoard.Where(c => c != source && c.currentPower >= 0).ToList();

        if (candidates.Count > 0)
        {
            var target = candidates[Random.Range(0, candidates.Count)];
            Debug.Log($"Aktywacja efektu: {effectName}. {source.data.cardName} losowo wzmacnia {target.data.cardName} o {powerToAdd}.");
            target.AddPower(powerToAdd);
            game.UpdateUI();
        }
    }
}
