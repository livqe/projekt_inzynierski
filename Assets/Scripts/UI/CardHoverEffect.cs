using UnityEngine;
using UnityEngine.EventSystems;

public class CardHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Vector3 originalScale;
    private bool isHovered = false;

    void Start()
    {
        originalScale = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (GameController.Instance.currentState == GameState.WaitingForTarget) return;

        if (isHovered) return;
        isHovered = true;

        transform.localScale = originalScale * 1.2f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        transform.localScale = originalScale;
    }

    void OnDisable()
    {
        transform.localScale = originalScale;
        isHovered = false;
    }
}
