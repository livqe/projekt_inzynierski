using UnityEngine;
using UnityEngine.EventSystems;

public class CardOnBoard : MonoBehaviour, IPointerClickHandler
{
    public CardInstance cardInstance;

    public  void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        Debug.Log($"--Klikniêto obiekt {gameObject.name}--");

        if (cardInstance == null) return;

        GameController.Instance.CardClicked(cardInstance);
    }
}