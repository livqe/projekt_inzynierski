using UnityEngine;

[CreateAssetMenu(menuName = "Card/Effects/Buff/ConditionalBuffRowEffect")]
public class ConditionalBuffRowEffect : CardEffect, IOnOtherCardPlayedEffect
{
    [SerializeField] private int powerToAdd;
    [SerializeField] private string allyName;

    public void Initialize(int powerAmount, string ally)
    {
        this.powerToAdd = powerAmount;
        this.allyName = ally;
    }

    public override void ActivateEffect(GameController game, CardInstance source)
    {
        var cardBoard = (source.owner == game.player) ? game.playerBoard : game.enemyBoard;

        bool conditionMet = false;
        foreach (var card in cardBoard)
        {
            if (card.data.cardName == allyName && card.currentPower >= 0)
            {
                conditionMet = true;
                break;
            }
        }

        if(conditionMet)
        {
            Debug.Log($"Aktywacja efektu: {effectName}. {allyName} na stole, {source.data.cardName} wzmacnia rz¹d o +{powerToAdd}.");
            
            BuffTheRow(source, cardBoard);
            
            game.UpdateUI();
        }
    }

    public void OnOtherCardPlayed(GameController game, CardInstance source, CardInstance playedCard)
    {
        if (playedCard.owner == source.owner && playedCard.data.cardName == allyName)
        {
            Debug.Log($"Aktywacja efektu: {effectName}. {allyName} na stole, {source.data.cardName} wzmacnia rz¹d o +{powerToAdd}.");
            
            var cardBoard = (source.owner == game.player) ? game.playerBoard : game.enemyBoard;
            BuffTheRow(source, cardBoard);

            game.UpdateUI();
        }
    }

    private void BuffTheRow(CardInstance source, System.Collections.Generic.List<CardInstance> board)
    {
        foreach (var card in board)
        {
            if (card.data.range == source.data.range && card != source && card.currentPower >= 0)
            {
                card.AddPower(powerToAdd);
            }
        }
    }
}
