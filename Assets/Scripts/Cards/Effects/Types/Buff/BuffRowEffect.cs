using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Card/Effects/Buff/BuffRowEffect")]
public class BuffRowEffect : CardEffect
{
    [SerializeField] private int powerToAdd;
    
    [Header("Visual Effects")]
    public GameObject vfxPrefab;

    public void Initialize(int powerAmount)
    {
        this.powerToAdd = powerAmount;
    }

    public override void ActivateEffect(GameController game, CardInstance source)
    {
        Debug.Log($"Aktywacja efektu: {effectName}. {source.data.cardName} wzmacnia swój rz¹d o +{powerToAdd}.");

        CardOnBoard sourceVisual = FindVisualForCard(source);

        if (sourceVisual == null)
        {
            Debug.LogWarning("B³¹d: Nie znaleziono wizualnej karty na stole.");
            return;
        }

        BoardRow rowObject = sourceVisual.GetComponentInParent<BoardRow>();

        if (rowObject == null)
        {
            Debug.LogWarning("B³¹d: Karta nie jest przypisana do ¿adnego rzêdu.");
            return;
        }

        CardOnBoard[] neighbors = rowObject.GetComponentsInChildren<CardOnBoard>();

        if (vfxPrefab != null) Instantiate(vfxPrefab, rowObject.transform.position, Quaternion.identity);

        foreach (var visualCard in neighbors)
        {
            CardInstance card = visualCard.cardInstance;

            if (card != null && card != source && card.currentPower >= 0)
            {
                card.AddPower(powerToAdd);
            }
        }
        game.UpdateUI();
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
