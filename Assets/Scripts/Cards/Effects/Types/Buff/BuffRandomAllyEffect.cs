using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "Card/Effects/Buff/BuffRandomAllyEffect")]
public class BuffRandomAllyEffect :CardEffect
{
    [SerializeField] private int powerToAdd;
    [SerializeField] private int targetCount = 1;

    public void Initialize(int powerAmount, int count = 1)
    {
        this.powerToAdd = powerAmount;
        this.targetCount = count;
    }

    public override void ActivateEffect(GameController game, CardInstance source)
    {
        var cardBoard = (source.owner == game.player) ? game.playerBoard : game.enemyBoard;

        var candidates = cardBoard.Where(c => c != source && c.currentPower >= 0).ToList();

        if (candidates.Count == 0) return;

        var shuffled = candidates.OrderBy(x => Random.value).Take(targetCount).ToList();

        foreach (var target in shuffled)
        {
            Debug.Log($"Aktywacja efektu: {effectName}. {source.data.cardName} losowo wzmacnia {target.data.cardName} o +{powerToAdd}.");
            target.AddPower(powerToAdd);
        }

        game.UpdateUI();
    }
}
