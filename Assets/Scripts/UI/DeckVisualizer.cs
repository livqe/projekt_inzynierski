using UnityEngine;
using TMPro;

public class DeckVisualizer : MonoBehaviour
{
    public Player owner;
    public TextMeshProUGUI deckCounterText;
    public SpriteRenderer[] cardSprites;

    void Update()
    {
        if (owner == null) return;

        int count = owner.cardsInDeck.Count;
        deckCounterText.text = count.ToString();

        if (cardSprites.Length >= 3)
        {
            cardSprites[0].enabled = count > 0;
            cardSprites[1].enabled = count > 10;
            cardSprites[2].enabled = count > 20;
        }
    }
}
