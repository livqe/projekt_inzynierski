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

    private SimpleRowLayout currentTargetLayout;

    private bool isDragActive = false;
    private bool wasPlayed = false;

    public static System.Action<CardInstance> OnCardDragStart;
    public static System.Action OnCardDragEnd;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        cardView = GetComponent<CardView>();
        canvas = GetComponentInParent<Canvas>();
    }

    void Update()
    {
        if (isDragActive && Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame)
        {
            Debug.Log("Anulowano przeci¹ganie.");
            CancelDragManually();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (wasPlayed) return;
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

        if (cardView != null && cardView.cardInstance != null)
        {
            OnCardDragStart?.Invoke(cardView.cardInstance);
            HighlightRows(true);
        }
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
        canvasGroup.blocksRaycasts = true;

        Debug.Log("Puszczam kartê.");

        HighlightRows(false);

        SimpleRowLayout targetLayout = currentTargetLayout;
        int index = -1;

        if (targetLayout != null)
        {
            Vector3 mousePosWorld = Camera.main.ScreenToWorldPoint(eventData.position);
            index = targetLayout.CalculateIndexForX(mousePosWorld.x);
            targetLayout.RemoveGhost();
            currentTargetLayout = null;
        }

        if (targetLayout != null)
        {
            BoardRow rowData = targetLayout.GetComponentInParent<BoardRow>();

            if (rowData != null && IsMoveValid(rowData))
            {
                if (targetLayout.transform.childCount >= 9)
                {
                    Debug.LogWarning("Ten rz¹d jest pe³ny.");
                    ReturnToHand();
                }
                else
                {
                    PlayCardOnBoard(rowData.rowType, targetLayout, index);
                }
            }
            else
            {
                ReturnToHand();
            }
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

        HighlightRows(false);

        if (currentTargetLayout != null)
        {
            currentTargetLayout.RemoveGhost();
            currentTargetLayout = null;
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
                if (rowScript.linkedLayout != null) currentRow = rowScript.linkedLayout;
                else Debug.LogError($"Rz¹d {rowScript.name} nie ma przypisanego Linked Layout.");
            }
        }

        if (currentTargetLayout != null && currentTargetLayout != currentRow)
        {
            currentTargetLayout.RemoveGhost();

            var visuals = currentTargetLayout.GetComponentInParent<RowHighlighter>();
            if (visuals != null) visuals.SetHover(false);
        }

        if (currentRow != null)
        {
            if (currentRow != currentTargetLayout)
            {
                var visuals = currentRow.GetComponentInParent<RowHighlighter>();
                if (visuals != null) visuals.SetHover(true);
            }
        }

        currentTargetLayout = currentRow;

        if (currentTargetLayout != null)
        {
            currentTargetLayout.UpdateGhostPosition(mousePos.x);
        }
    }

    private bool IsMoveValid(BoardRow row)
    {
        if (!row.isPlayerRow) return false;
        if (cardView == null || cardView.cardInstance == null) return false;

        RangeType cardRange = cardView.cardInstance.data.range;

        if (cardRange == RangeType.Dowolny) return true;
        if (cardRange == row.rowType) return true;

        return false;
    }

    private void PlayCardOnBoard(RangeType droppedRowType, SimpleRowLayout layoutScript, int index)
    {
        wasPlayed = true;
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
        wasPlayed = false;
    }

    private void HighlightRows(bool show)
    {
        BoardRow[] allRows = FindObjectsByType<BoardRow>(FindObjectsSortMode.None);

        foreach (var row in allRows)
        {
            RowHighlighter visual = row.GetComponent<RowHighlighter>();
            if (visual == null) continue;

            if (show)
            {
                if (IsMoveValid(row)) visual.SetEligible(true);
            }
            else
            {
                visual.ResetRow();
            }
        }
    }
}
