using UnityEngine;

public class HandManager : MonoBehaviour
{
    [Header("Config")]
    public GameObject cardUIPrefab;
    public Transform handContainer;

    public void AddCardToHandVisual(CardInstance card, int index = -1)
    {
        if (cardUIPrefab == null || handContainer == null)
        {
            Debug.Log("[HandManager] Nie przypisano prefabu lub kontenera.");
            return;
        }

        GameObject newCardObj = Instantiate(cardUIPrefab, handContainer);

        if (index >= 0)
            newCardObj.transform.SetSiblingIndex(index);

        var cardView = newCardObj.GetComponent<CardView>();
        if (cardView != null )
        {
            cardView.LoadCardData(card);
        }
        else
        {
            Debug.Log("Prefab karty UI nie ma komponentu CardView.");
        }
    }

    public void RemoveCardVisual(CardInstance card)
    {
        foreach (Transform child in handContainer)
        {
            var view = child.GetComponent<CardView>();
            if (view != null && view.cardInstance == card)
            {
                Destroy(child.gameObject);
                return;
            }
        }
    }

    public void RefreshHandVisuals()
    {
        if (handContainer == null) return;

        foreach (Transform child in handContainer)
        {
            var cardView = child.GetComponent<CardView>();
            
            if (cardView != null && cardView.cardInstance != null) cardView.LoadCardData(cardView.cardInstance);
        }
    }
}
