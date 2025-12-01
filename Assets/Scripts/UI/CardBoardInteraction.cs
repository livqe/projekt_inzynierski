using UnityEngine;
using UnityEngine.EventSystems;

public class CardBoardInteraction : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private CardOnBoard cardOnBoard;
    private Vector3 originalScale;
    private SpriteRenderer spriteRenderer;
    private CardBoardVisual visual;

    private bool isHovered = false;

    void Awake()
    {
        cardOnBoard = GetComponent<CardOnBoard>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        visual = GetComponent<CardBoardVisual>();
    }

    void Start()
    {
        originalScale = transform.localScale;
        if (originalScale == Vector3.zero) originalScale = Vector3.one;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!GameController.Instance.CanInteract()) return;

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (cardOnBoard != null && cardOnBoard.cardInstance != null)
                CardZoomManager.Instance.ShowZoom(cardOnBoard.cardInstance);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (GameController.Instance.currentState == GameState.WaitingForTarget) return;
        if (GameController.Instance.isDragging) return;
        if (isHovered) return;

        isHovered = true;

        transform.localScale = originalScale * 1.15f;

        if (spriteRenderer) spriteRenderer.sortingOrder += 10;
        if (visual && visual.powerText) visual.powerText.sortingOrder += 10;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isHovered) return;
        isHovered = false;

        transform.localScale = originalScale;

        if (spriteRenderer) spriteRenderer.sortingOrder -= 10;
        if (visual && visual.powerText) visual.powerText.sortingOrder -= 10;
    }

    void OnDisable()
    {
        if (isHovered)
        {
            transform.localScale = originalScale;
            if (spriteRenderer) spriteRenderer.sortingOrder -= 10;
            if (visual && visual.powerText) visual.powerText.sortingOrder -= 10;
            isHovered = false;
        }
    }
}
