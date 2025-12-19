using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuilderDeckItem : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI powerText;
    public Image artworkImage;
    public Button removeButton;

    private CardData data;
    private DeckBuilderController controller;

    public void Setup(CardData cardData, DeckBuilderController ctrl)
    {
        data = cardData;
        controller = ctrl;

        nameText.text = data.cardName;
        powerText.text = data.power.ToString();
        artworkImage.sprite = data.artwork;

        removeButton.onClick.RemoveAllListeners();
        removeButton.onClick.AddListener(() => controller.RemoveCardFromDeck(data, this));
    }
}
