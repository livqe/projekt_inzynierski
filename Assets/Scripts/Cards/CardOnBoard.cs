using UnityEngine;
using UnityEngine.EventSystems;

public class CardOnBoard : MonoBehaviour, IPointerClickHandler
{
    public CardInstance cardInstance;

    public  void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"--Klikniêto obiekt {gameObject.name}--");

        if (cardInstance == null) return;

        GameController.Instance.CardClicked(cardInstance);
    }

    //podœwietlenie po najechaniu myszk¹
    private void OnMouseEnter()
    {
        if (GameController.Instance.currentState == GameState.WaitingForTarget)
        {
            GetComponent<SpriteRenderer>().color = Color.red;
        }
    }

    private void OnMouseExit()
    {
        GetComponent<SpriteRenderer>().color = Color.white;
    }
}