using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour 
{
    [Header("UI Elements")]
    public Image artworkImage;
    public TextMeshProUGUI powerText;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI shieldText;

    [HideInInspector] public CardInstance cardInstance;

    public void LoadCardData(CardInstance card)
    {
        cardInstance = card;

        if (artworkImage != null) artworkImage.sprite = card.data.artwork;
        if (nameText != null) nameText.text = card.data.cardName;

        if (powerText != null)
        {
            bool useOverride = !string.IsNullOrEmpty(card.data.powerDisplayOverride) && (card.currentPower == card.data.power);
            
            if (useOverride)
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

            if (shieldText != null)
            {
                int amount = card.shield;

                if (amount > 0)
                {
                    shieldText.text = amount.ToString();
                    shieldText.gameObject.SetActive(true);
                }
                else
                {
                    shieldText.gameObject.SetActive(false);
                }
            }
        }
    }
}
