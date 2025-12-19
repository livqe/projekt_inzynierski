using UnityEngine;

[CreateAssetMenu(menuName = "Card/Effects/Buff/ConditionalBuffRowEffect")]
public class ConditionalBuffRowEffect : CardEffect, IOnOtherCardPlayedEffect
{
    [SerializeField] private int powerToAdd;
    [SerializeField] private string allyName;
    
    [Header("Visual Effects")]
    public GameObject vfxPrefab;

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

            CardOnBoard sourceVisual = FindVisualForCard(source);
            if (sourceVisual == null) return;

            BoardRow rowObject = sourceVisual.GetComponentInParent<BoardRow>();
            if (rowObject == null) return;

            if (vfxPrefab != null) Instantiate(vfxPrefab, rowObject.transform.position, Quaternion.identity);

            CardOnBoard[] neighbors = rowObject.GetComponentsInChildren<CardOnBoard>();
            foreach (var visualCard in neighbors)
            {
                CardInstance card = visualCard.cardInstance;

                if (card != null && card != source && card.currentPower >= 0)
                {
                    card.AddPower(powerToAdd);
                }
            }

            source.effectTriggered = true;
            game.UpdateUI();
        } 
    }

    private CardOnBoard FindVisualForCard(CardInstance cardData)
    {
        CardOnBoard[] allVisuals = FindObjectsByType<CardOnBoard>(FindObjectsSortMode.None);
        foreach (var visual in allVisuals)
        {
            if (visual.cardInstance == cardData) return visual;
        }
        return null;
    }
}
