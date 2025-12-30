using UnityEngine;

public class UnitHighlighter : MonoBehaviour
{
    [Header("Visuals")]
    public GameObject targetFrame;
    public SpriteRenderer frameRenderer;

    [Header("Sprites")]
    public Sprite friendlySprite;
    public Sprite enemySprite;

    [Header("Colors")]
    public Color defaultColor = new Color(1f, 1f, 1f, 1f);
    public Color hoverColor = new Color(1f, 1f, 1f, 0.7f);

    private bool isActive = false;

    private void OnValidate()
    {
        if (frameRenderer == null && targetFrame != null)
            frameRenderer = targetFrame.GetComponent<SpriteRenderer>();
    }

    void Awake()
    {
        if (targetFrame != null) targetFrame.SetActive(false);

        if (frameRenderer == null && targetFrame != null)
            frameRenderer = targetFrame.GetComponent<SpriteRenderer>();
    }

    public void ShowTarget(bool show, bool isEnemyAction)
    {
        isActive = show;

        if (targetFrame == null) return;

        targetFrame.SetActive(show);

        if (show && frameRenderer != null)
        {
            frameRenderer.sprite = isEnemyAction ? enemySprite : friendlySprite;
            frameRenderer.color = defaultColor;
        }
    }

    private void OnMouseEnter()
    {
        if (isActive && frameRenderer != null)
            frameRenderer.color = hoverColor;
    }

    private void OnMouseExit()
    {
        if (isActive && frameRenderer != null)
            frameRenderer.color = defaultColor;
    }
}
