using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class DragAndPlay : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Transform originalParent;
    private CardView cardView;
    private int originalIndex;
    private SimpleRowLayout lastHoveredRow;
    private bool isDragActive = false;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        cardView = GetComponent<CardView>();
        canvas = GetComponentInParent<Canvas>();
    }

    void Update()
    {
        if (transform.parent == canvas.transform)
        {
            if (isDragActive && Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame)
            {
                Debug.Log("Anulowano przeci¹ganie.");
                CancelDragManually();
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!GameController.Instance.isPlayerTurn) return;
        if (GameController.Instance.currentState == GameState.WaitingForTarget) return;
        if (GetComponent<CardOnBoard>() != null) return;

        Debug.Log("Podnoszê kartê...");

        isDragActive = true;
        GameController.Instance.isDragging = true;

        originalParent = transform.parent;
        originalIndex = transform.GetSiblingIndex();

        transform.SetParent(canvas.transform, true);
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragActive) return;

        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        CheckRowUnderMouse(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(!isDragActive) return;

        isDragActive = false;
        GameController.Instance.isDragging = false;

        if (transform.parent == originalParent) return;

        Debug.Log("Puszczam kartê.");
        canvasGroup.blocksRaycasts = true;

        if (lastHoveredRow != null)
        {
            int finalIndex = lastHoveredRow.GetGhostIndex();
            BoardRow rowData = lastHoveredRow.GetComponent<BoardRow>();

            lastHoveredRow.RemoveGhost();
            lastHoveredRow = null;

            if (rowData.transform.childCount >= 9)
            {
                Debug.LogWarning("Ten rz¹d jest pe³ny.");
                ReturnToHand();
                return;
            }

            if (IsMoveValid(rowData))
                PlayCardOnBoard(rowData.rowType, finalIndex);
            else
                ReturnToHand();
        }
        else
        {
            ReturnToHand();
        }
    }

    private void CancelDragManually()
    {
        isDragActive = false;
        GameController.Instance.isDragging = false;
        GameController.Instance.BlockInteractionFor(0.2f);

        if (lastHoveredRow != null)
        {
            lastHoveredRow.RemoveGhost();
            lastHoveredRow = null;
        }

        canvasGroup.blocksRaycasts = true;
        ReturnToHand();
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
        GameController.Instance.PlayCard(cardView.cardInstance, true, true, droppedRowType, index);
        Destroy(gameObject);
    }

    private void ReturnToHand()
    {
        transform.SetParent(originalParent);
        transform.SetSiblingIndex(originalIndex);
        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one;
    }
}
