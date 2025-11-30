using UnityEngine;

[ExecuteInEditMode]
public class SpriteScaler : MonoBehaviour
{
    [Tooltip("Width")]
    public float targetWidth = 1.5f;

    [Tooltip("Keep Aspect Ratio?")]
    public bool keepAspectRatio = true;

    private void Start()
    {
        AdjustSize();
    }

    private void OnValidate()
    {
        AdjustSize();
    }

    public void AdjustSize()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null || sr.sprite == null) return;

        float currentWidth = sr.sprite.bounds.size.x;
        if (currentWidth == 0) return;

        float newScale = targetWidth / currentWidth;

        if (keepAspectRatio)
            transform.localScale = new Vector3(newScale, newScale, 1);
        else
            transform.localScale = new Vector3(newScale, transform.localScale.y, 1);
    }
}
