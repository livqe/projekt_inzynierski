using UnityEngine;
using UnityEngine.EventSystems;

public class CardInteraction : MonoBehaviour, IPointerClickHandler
{
    private CardView cardView;

    void Awake()
    {
        cardView = GetComponent<CardView>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!GameController.Instance.CanInteract()) return;
        if (GameController.Instance.currentState == GameState.WaitingForTarget) return;

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log("Inspekcja karty...");
            if (cardView != null && cardView.cardInstance != null)
                CardZoomManager.Instance.ShowZoom(cardView.cardInstance);
        }
    }
}
