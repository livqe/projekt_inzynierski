using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class CardZoomManager : MonoBehaviour
{
    public static CardZoomManager Instance {  get; private set; }

    [Header("UI Elements")]
    public GameObject zoomPanel;
    public Image zoomImage;
    public TextMeshProUGUI zoomPowerText;
    public TextMeshProUGUI zoomEffectText;

    private bool isZoomed = false;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (zoomPanel != null) zoomPanel.SetActive(false);    
    }

    public void ShowZoom(CardInstance card)
    {
        if (zoomPanel == null) return;

        isZoomed = true;
        zoomPanel.SetActive(true);

        if (zoomImage != null) zoomImage.sprite = card.data.artwork;
        if (zoomPowerText != null) zoomPowerText.text = card.currentPower.ToString();
        if (zoomEffectText != null) zoomEffectText.text = card.data.effectDescription;
    }

    public void HideZoom()
    {
        if (zoomPanel != null) zoomPanel.SetActive(false);
        isZoomed = false;
    }

    void Update()
    {
        if (isZoomed)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
                HideZoom();
        }
    }
}
