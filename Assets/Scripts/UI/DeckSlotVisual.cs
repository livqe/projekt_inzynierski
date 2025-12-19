using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DeckSlotVisual : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI deckNameText;
    public TextMeshProUGUI factionText;
    public Button button;

    public void Setup(SavedDeck deck, System.Action<SavedDeck> onClickAction)
    {
        if (deckNameText != null) deckNameText.text = deck.deckName;

        if (factionText != null)
        {
            factionText.text = deck.faction.ToString();

            if (deck.faction == Faction.Elfy) factionText.color = Color.green;
            else if (deck.faction == Faction.Krasnoludy) factionText.color = Color.red;
        }

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClickAction(deck));
    }
}
