using UnityEngine;
using UnityEngine.EventSystems;

public class BoardRow : MonoBehaviour, IPointerClickHandler
{
    public RangeType rowType;

    public bool isPlayerRow;

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"---Klikniêto rz¹d: {(isPlayerRow ? "Gracz" : "Wróg")} - {rowType}---");

        GameController.Instance.RowClicked(rowType, isPlayerRow);
    }
}
