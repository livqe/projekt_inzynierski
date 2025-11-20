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
        BuffTheRow(game, source);
    }

    public void OnOtherCardPlayed(GameController game, CardInstance source, CardInstance playedCard)
    {
        if (source.effectTriggered) return;

        if (playedCard.owner == source.owner && playedCard.data.cardName == allyName)
        {
            BuffTheRow(game, source);
        }
    }

    private void BuffTheRow(GameController game, CardInstance source)
    {
        if (source.effectTriggered) return;

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

        if (conditionMet)
        {
            Debug.Log($"Aktywacja efektu: {effectName}. {allyName} na stole, {source.data.cardName} wzmacnia rz¹d o +{powerToAdd}.");

            foreach (var card in cardBoard)
            {
                if (card.data.range == source.data.range && card != source && card.currentPower >= 0)
                {
                    card.AddPower(powerToAdd);
                }
            }

            source.effectTriggered = true;
            game.UpdateUI();
        }
        
    }
}
