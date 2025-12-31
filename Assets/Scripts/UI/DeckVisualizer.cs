using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeckVisualizer : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI counterText;
    public GameObject cardStack;
    public Image stackImage;

    public void SetFactionVisual(Sprite cardBack)
    {
        if (stackImage != null) stackImage.sprite = cardBack;
    }

    public void UpdateCount(int count)
    {
        if (counterText != null) counterText.text = count.ToString();

        if (cardStack != null)
            cardStack.SetActive(count > 0);
    }
}
