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
            if (card.currentPower > card.data.power) powerText.color = Color.green;
            else if (card.currentPower < card.data.power) powerText.color = Color.red;
            else powerText.color = Color.black;
        }
    }
}
