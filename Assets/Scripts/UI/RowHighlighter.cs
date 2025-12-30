using UnityEngine;

public class RowHighlighter : MonoBehaviour
{
    [Header("Config")]
    public GameObject highlightFrame;
    public SpriteRenderer frameRenderer;

    [Header("Sprites")]
    public Sprite dragDropSprite;
    public Sprite targetSprite;

    [Header("Colors")]
    public Color defaultColor = new Color(0f, 1f, 0f, 1f);
    public Color hoverColor = new Color(1f, 0.8f, 0f, 1f);
    public Color targetDefaultColor = new Color(1f, 1f, 1f, 1f);
    public Color targetHoverColor = new Color(1f, 1f, 1f, 0.7f);

    private bool isEligible = false;
    private bool isTargetMode = false;

    void Start()
    {
        if (highlightFrame != null) highlightFrame.SetActive(false);

        if (frameRenderer == null && highlightFrame != null)
            frameRenderer = highlightFrame.GetComponent<SpriteRenderer>();

        if (dragDropSprite == null && frameRenderer != null)
            dragDropSprite = frameRenderer.sprite;
    }

    public void SetEligible(bool eligible)
    {
        isEligible = eligible;
        isTargetMode = false;

        UpdateVisuals(false);
    }

    public void SetTargetMode(bool active)
    {
        isTargetMode = active;
        isEligible = false;

        if (active && highlightFrame != null)
        {
            highlightFrame.SetActive(true);

            if (frameRenderer != null) frameRenderer.sprite = targetSprite;
            frameRenderer.color = targetDefaultColor;
        }
        else ResetRow();
    }

    private void OnMouseEnter()
    {
        UpdateVisuals(true);
    }

    private void OnMouseExit()
    {
        UpdateVisuals(false);
    }

    public void SetHover(bool isHovered)
    {
        if (!isEligible || highlightFrame == null) return;

        if (frameRenderer != null)
            frameRenderer.color = isHovered ? hoverColor : defaultColor;
    }

    public void ResetRow()
    {
        isEligible = false;
        isTargetMode = false;
        if (highlightFrame != null) highlightFrame.SetActive(false);
    }

    private void UpdateVisuals(bool isHovering)
    {
        if (highlightFrame == null) return;

        if (isEligible)
        {
            highlightFrame.SetActive(true);
            if (dragDropSprite != null) frameRenderer.sprite = dragDropSprite;
            frameRenderer.color = isHovering ? hoverColor : defaultColor;
        }
        else if (isTargetMode)
        {
            highlightFrame.SetActive(true);
            if (targetSprite != null) frameRenderer.sprite = targetSprite;
            frameRenderer.color = isHovering ? targetHoverColor : targetDefaultColor;
        }
    }
}
