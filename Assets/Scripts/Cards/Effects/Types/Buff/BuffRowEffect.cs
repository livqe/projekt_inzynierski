using UnityEngine;

[CreateAssetMenu(menuName = "Card/Effects/Buff/BuffRowEffect")]
public class BuffRowEffect : CardEffect
{
    [SerializeField] private int powerToAdd;

    public void Initialize(int powerAmount)
    {
        this.powerToAdd = powerAmount;
    }

    public override void ActivateEffect(GameController game, CardInstance source)
    {
        Debug.Log($"Aktywacja efektu: {effectName}. {source.data.cardName} wzmacnia swój rz¹d o +{powerToAdd}.");

        var cardBoard = (source.owner == game.player) ? game.playerBoard : game.enemyBoard;

        foreach (var card in cardBoard)
        {
            if (card.data.range == source.data.range && card != source && card.currentPower >= 0)
            {
                card.AddPower(powerToAdd);
            }
        }
        game.UpdateUI();
    }
}
