using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour 
{
    [Header("UI Elements")]
    public Image artworkImage;
    public TMP_Text powerText;

    //³adowanie kart tutaj
    public void LoadCardData(CardData data)
    {
        powerText.text = data.power.ToString();

        if (data.artwork != null)
            artworkImage.sprite = data.artwork;
    }
}
