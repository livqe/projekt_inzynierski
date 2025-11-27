using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragAndPlay : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Transform originalParent;
    private CardView cardView;
    private int originalIndex;
    private SimpleRowLayout lastHoveredRow;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        cardView = GetComponent<CardView>();

        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Podnoszê kartê...");

        originalParent = transform.parent;
        originalIndex = transform.GetSiblingIndex();

        transform.SetParent(canvas.transform, true);
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

        CheckRowUnderMouse(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("Puszczam kartê.");
        canvasGroup.blocksRaycasts = true;

        if (lastHoveredRow != null)
        {
            int finalIndex = lastHoveredRow.GetGhostIndex();
            BoardRow rowData = lastHoveredRow.GetComponent<BoardRow>();

            lastHoveredRow.RemoveGhost();
            lastHoveredRow = null;

            PlayCardOnBoard(rowData.rowType, finalIndex);
        }
        else
        {
            ReturnToHand();
        }
    }

    private void CheckRowUnderMouse(PointerEventData eventData)
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(eventData.position);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        SimpleRowLayout currentRow = null;

        if (hit.collider != null)
        {
            BoardRow rowScript = hit.collider.GetComponent<BoardRow>();

            if (rowScript != null && IsMoveValid(rowScript))
            {
                currentRow = rowScript.GetComponent<SimpleRowLayout>();
            }
        }

        if (lastHoveredRow != null && lastHoveredRow != currentRow)
        {
            lastHoveredRow.RemoveGhost();
        }

        if (currentRow != null)
        {
            currentRow.UpdateGhostPosition(mousePos.x);
        }

        lastHoveredRow = currentRow;
    }

    private bool IsMoveValid(BoardRow row)
    {
        if (!row.isPlayerRow) return false;

        RangeType cardRange = cardView.cardInstance.data.range;

        if (cardRange == RangeType.Dowolny) return true;
        if (cardRange == row.rowType) return true;

        return false;
    }

    private void PlayCardOnBoard(RangeType droppedRowType, int index)
    {
        Debug.Log($"Udane zagranie karty w rzêdzie {droppedRowType}.");
        GameController.Instance.PlayCard(cardView.cardInstance, true, droppedRowType, index);
        Destroy(gameObject);
    }

    private void ReturnToHand()
    {
        transform.SetParent(originalParent);
        transform.SetSiblingIndex(originalIndex);
        transform.localPosition = Vector3.zero;
    }
}
