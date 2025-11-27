using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour 
{
    [Header("UI Elements")]
    public Image artworkImage;
    public TextMeshProUGUI powerText;

    [HideInInspector] public CardInstance cardInstance;

    public void LoadCardData(CardInstance card)
    {
        cardInstance = card;

        if (card.data.artwork != null) artworkImage.sprite = card.data.artwork;
        powerText.text = card.data.power.ToString();
    }
}
