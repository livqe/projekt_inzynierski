using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance;

    [Header("UI References")]
    public GameObject tooltipPanel;
    public RectTransform tooltipRect;
    public TextMeshProUGUI headerText;
    public TextMeshProUGUI contentText;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this.gameObject);
        else Instance = this;

        Hide();
    }

    private void Update()
    {
        if (tooltipPanel.activeSelf)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            mousePos += new Vector2(15f, -15f);
            tooltipPanel.transform.position = mousePos;
        }
    }

    public static void Show(string header, string content)
    {
        if (Instance == null) return;

        Instance.headerText.text = header;
        Instance.contentText.text = content;

        Instance.tooltipPanel.SetActive(true);

        LayoutRebuilder.ForceRebuildLayoutImmediate(Instance.tooltipRect);

        Instance.transform.SetAsLastSibling();
    }

    public static void Hide()
    {
        if (Instance == null) return;
        Instance.tooltipPanel.SetActive(false);
    }
}
