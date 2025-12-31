using UnityEngine;
using UnityEngine.InputSystem;

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
    public Color hoverColor = new Color(1f, 1f, 1f, 0.4f);

    private bool isActive = false;
    private Collider2D myCollider;

    private void OnValidate()
    {
        if (frameRenderer == null && targetFrame != null)
            frameRenderer = targetFrame.GetComponent<SpriteRenderer>();
    }

    void Awake()
    {
        myCollider = GetComponent<Collider2D>();

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

    void Update()
    {
        if (!isActive || myCollider == null || frameRenderer == null) return;
            
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        if (myCollider.OverlapPoint(mousePos)) frameRenderer.color = hoverColor;
        else frameRenderer.color = defaultColor;
    }
}
