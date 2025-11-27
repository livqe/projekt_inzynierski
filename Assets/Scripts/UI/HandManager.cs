using UnityEngine;

public class HandManager : MonoBehaviour
{
    [Header("Config")]
    public GameObject cardUIPrefab;
    public Transform handContainer;

    public void AddCardToHandVisual(CardInstance card)
    {
        if (cardUIPrefab == null || handContainer == null)
        {
            Debug.Log("[HandManager] Nie przypisano prefabu lub kontenera.");
            return;
        }

        GameObject newCardObj = Instantiate(cardUIPrefab, handContainer);

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
}
