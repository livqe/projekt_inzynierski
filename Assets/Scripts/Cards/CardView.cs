using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour 
{
    [Header("UI Elements")]
    public Image artworkImage;
    public TextMeshProUGUI powerText;
    public TextMeshProUGUI nameText;

    [HideInInspector] public CardInstance cardInstance;

    public void LoadCardData(CardInstance card)
    {
        cardInstance = card;

        if (artworkImage != null) artworkImage.sprite = card.data.artwork;
        if (nameText != null) nameText.text = card.data.cardName;

        if (powerText != null)
        {
            if (!string.IsNullOrEmpty(card.data.powerDisplayOverride))
            {
                string overrideVal = card.data.powerDisplayOverride;

                if (overrideVal == "-")
                    powerText.gameObject.SetActive(false);
                else
                {
                    powerText.gameObject.SetActive(true);
                    powerText.text = overrideVal;
                }
            }
            else
            {
                powerText.gameObject.SetActive(true);
                powerText.text = card.currentPower.ToString();

                Color finalColor = Color.black;

                if (card.currentPower > card.basePowerFromEffect) ColorUtility.TryParseHtmlString("#4ACD00", out finalColor);
                else if (card.currentPower < card.basePowerFromEffect) ColorUtility.TryParseHtmlString("#CB0000", out finalColor);
                else ColorUtility.TryParseHtmlString("#333333", out finalColor);

                powerText.color = finalColor;
            }
        }
    }
}
