using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class OpponentHandManager : MonoBehaviour
{
    public GameObject cardBackPrefab;
    public Transform handContainer;

    private Sprite currentFactionSprite;
    private List<GameObject> spawnedCards = new List<GameObject>();

    public void setCardBackSprite(Sprite sprite)
    {
        currentFactionSprite = sprite;
    }

    public void AddCard()
    {
        if (cardBackPrefab != null && handContainer != null)
        {
            GameObject newCard = Instantiate(cardBackPrefab, handContainer);

            Image img = newCard.GetComponent<Image>();
            if (img != null && currentFactionSprite != null) img.sprite = currentFactionSprite;

            spawnedCards.Add(newCard);
        }
    }

    public void RemoveCard()
    {
        if (spawnedCards.Count > 0)
        {
            GameObject cardToRemove = spawnedCards[spawnedCards.Count - 1];
            spawnedCards.RemoveAt(spawnedCards.Count - 1);
            Destroy(cardToRemove);
        }
    }

    public void ClearHand()
    {
        foreach (var c in spawnedCards) Destroy(c);
        spawnedCards.Clear();
    }
}
