using UnityEngine;
using UnityEngine.EventSystems;

public class BuilderLibraryItem : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public CardView cardView;
    public CanvasGroup canvasGroup;

    private CardData data;
    private DeckBuilderController controller;
    private int availableCopies;

    public void Setup(CardData cardData, DeckBuilderController ctrl)
    {
        data = cardData;
        controller = ctrl;

        CardInstance dummyInstance = new CardInstance(data,null);

        if (cardView != null) cardView.LoadCardData(dummyInstance);
        else Debug.Log("CardView nie jest przypisany w prefabie BuilderLibraryItem.");

        RefreshAvailability();
    }

    public void RefreshAvailability()
    {
        int currentCounterInDeck = controller.GetCardCountInDeck(data);
        availableCopies = data.maxCopies - currentCounterInDeck;

        if (availableCopies <= 0)
        {
            canvasGroup.alpha = 0.5f;
            canvasGroup.interactable = false;
        }
        else
        {
            canvasGroup.alpha = 1.0f;
            canvasGroup.interactable = true;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (availableCopies > 0)
        {
            controller.AddCardToDeck(data);
            OnPointerEnter(eventData);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        string status = availableCopies > 0
            ? $"<color=white>Dostêpne: {availableCopies + " / " + data.maxCopies}</color>"
            : $"<color=red>Limit osi¹gniêty {data.maxCopies}</color>";

        string fullContent = $"{data.effectDescription}\n\n{status}";

        TooltipManager.Show(data.cardName, fullContent);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipManager.Hide();
    }
}
