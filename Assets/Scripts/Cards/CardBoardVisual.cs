using UnityEngine;
using TMPro;

public class CardBoardVisual : MonoBehaviour
{
    public SpriteRenderer artwork;
    public TextMeshPro powerText;

    public void UpdateVisuals(CardInstance card)
    {
        if (artwork != null) artwork.sprite = card.data.artwork;

        if (powerText != null)
        {
            powerText.text = card.currentPower.ToString();

            Color finalColor = Color.black;

            if (card.currentPower > card.basePowerFromEffect) ColorUtility.TryParseHtmlString("#4ACD00", out finalColor);
            else if (card.currentPower < card.basePowerFromEffect) ColorUtility.TryParseHtmlString("#CB0000", out finalColor);
            else ColorUtility.TryParseHtmlString("#333333", out finalColor);

            powerText.color = finalColor;
        }
    }
}