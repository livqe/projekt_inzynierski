using UnityEngine;

public class CameraFit : MonoBehaviour
{
    [Header("Settings")]
    public SpriteRenderer boardReference;

    public float margin = 1.05f;

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        AdjustCamera();
    }

    void Update()
    {
        #if UNITY_EDITOR
        AdjustCamera();
        #endif
    }

    void AdjustCamera()
    {
        if (boardReference == null || cam == null) return;

        float targetHeight = boardReference.bounds.size.y;
        float targetWidth = boardReference.bounds.size.x;

        float screenRatio = (float)Screen.width / (float)Screen.height;
        float targetRatio = targetWidth / targetHeight;

        if (screenRatio >= targetRatio) cam.orthographicSize = (targetHeight / 2f) * margin;
        else
        {
            float differenceInSize = targetRatio / screenRatio;
            cam.orthographicSize = (targetHeight / 2f) * differenceInSize * margin;
        }
    }
}
