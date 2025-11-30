using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class MulliganController : MonoBehaviour
{
    [Header("UI Config")]
    public GameObject mulliganPanel;
    public Transform gridContainer;
    public GameObject cardUIPrefab;
    public Button finishButton;
    public TextMeshProUGUI replacementsLeftText;

    [Header("Preview Panel")]
    public GameObject previewPanel;
    public Image previewImage;
    public TextMeshProUGUI previewPower;
    public TextMeshProUGUI previewEffect;

    [Header("Settings")]
    public int maxReplacements = 2;

    private int replacementsUsed = 0;
    private Player localPlayer;
    private List<GameObject> displayedCards = new List<GameObject>();

    void Start()
    {
        if (previewPanel != null) previewPanel.SetActive(false);

        if (finishButton != null) 
            finishButton.onClick.AddListener(FinishMulligan);
    }

    public void StartMulligan(Player player)
    {
        localPlayer = player;
        replacementsUsed = 0;

        if (mulliganPanel != null) mulliganPanel.SetActive(true);
        if (finishButton != null)
        {
            finishButton.interactable = true;
            Text btnText = finishButton.GetComponentInChildren<Text>();
            if (btnText != null) btnText.text = "Gotowe";
        }

        UpdateReplacementsText();
        GenerateCardsGrid();
    }

    private void GenerateCardsGrid()
    {
        foreach (Transform child in gridContainer)
        {
            Destroy(child.gameObject);
        }
        displayedCards.Clear();

        foreach (CardInstance card in localPlayer.cardsInHand)
        {
            GameObject newCardObj = Instantiate(cardUIPrefab, gridContainer);

            CardView view = newCardObj.GetComponent<CardView>();
            if (view != null) view.LoadCardData(card);

            CanvasGroup cg = newCardObj.GetComponent<CanvasGroup>();
            if (cg == null) cg = newCardObj.AddComponent<CanvasGroup>();
            cg.blocksRaycasts = true;

            EventTrigger trigger = newCardObj.AddComponent<EventTrigger>();

            EventTrigger.Entry entryClick = new EventTrigger.Entry();
            entryClick.eventID = EventTriggerType.PointerClick;
            entryClick.callback.AddListener((data) => OnCardClicked(card, newCardObj));
            trigger.triggers.Add(entryClick);

            EventTrigger.Entry entryEnter = new EventTrigger.Entry();
            entryEnter.eventID = EventTriggerType.PointerEnter;
            entryEnter.callback.AddListener((data) => ShowPreview(card));
            trigger.triggers.Add(entryEnter);

            EventTrigger.Entry entryExit = new EventTrigger.Entry();
            entryExit.eventID = EventTriggerType.PointerExit;
            entryExit.callback.AddListener((data) => {
                if (previewPanel != null) previewPanel.SetActive(false);
            });
            trigger.triggers.Add(entryExit);

            displayedCards.Add(newCardObj);
        }
    }

    private void OnCardClicked(CardInstance cardToRemove, GameObject cardObject)
    {
        if (replacementsUsed >= maxReplacements)
        {
            Debug.Log("Wykorzystano limit wymian.");
            return;
        }

        if (localPlayer.cardsInDeck.Count == 0)
        {
            Debug.Log("Brak kart w talii na wymianê.");
            return;
        }

        Debug.Log($"Wymiana karty: {cardToRemove.data.cardName}");

        localPlayer.cardsInHand.Remove(cardToRemove);

        int randomIndex = Random.Range(0, localPlayer.cardsInDeck.Count);
        CardInstance newCard = localPlayer.cardsInDeck[randomIndex];

        localPlayer.cardsInDeck.RemoveAt(randomIndex);
        localPlayer.cardsInHand.Add(newCard);

        int insertIndex = Random.Range(0, localPlayer.cardsInDeck.Count + 1);
        localPlayer.cardsInDeck.Insert(insertIndex, cardToRemove);

        replacementsUsed++;
        UpdateReplacementsText();

        CardView view = cardObject.GetComponent<CardView>();
        if (view != null) view.LoadCardData(newCard);

        EventTrigger trigger = cardObject.GetComponent<EventTrigger>();
        trigger.triggers.Clear();

        EventTrigger.Entry entryClick = new EventTrigger.Entry();
        entryClick.eventID = EventTriggerType.PointerClick;
        entryClick.callback.AddListener((data) => OnCardClicked(newCard, cardObject));
        trigger.triggers.Add(entryClick);

        EventTrigger.Entry entryEnter = new EventTrigger.Entry();
        entryEnter.eventID = EventTriggerType.PointerEnter;
        entryEnter.callback.AddListener((data) => ShowPreview(newCard));
        trigger.triggers.Add(entryEnter);

        EventTrigger.Entry entryExit = new EventTrigger.Entry();
        entryExit.eventID = EventTriggerType.PointerExit;
        entryExit.callback.AddListener((data) => {
            if (previewPanel != null) previewPanel.SetActive(false);
        });
        trigger.triggers.Add(entryExit);

        ShowPreview(newCard);

        if (replacementsUsed >= maxReplacements)
        {
            Debug.Log("Wykorzystano limit. Zamykam mulligan...");
            StartCoroutine(AutoFinishSequence());
        }
    }

    private void ShowPreview(CardInstance card)
    {
        if (previewPanel != null)
        {
            previewPanel.SetActive(true);
            if (previewImage != null) previewImage.sprite = card.data.artwork;
            if (previewPower != null) previewPower.text = card.data.power.ToString();
            if (previewEffect != null) previewEffect.text = card.data.effectDescription;
        }
    }

    private void UpdateReplacementsText()
    {
        if (replacementsLeftText != null)
            replacementsLeftText.text = $"{maxReplacements - replacementsUsed}/{maxReplacements}";
    }

    public void FinishMulligan()
    {
        Debug.Log("Zakoñczono fazê wymiany.");
        if (mulliganPanel != null) mulliganPanel.SetActive(false);
        if (previewPanel != null) previewPanel.SetActive(false);

        GameController.Instance.OnMulliganFinished();
    }

    private IEnumerator AutoFinishSequence()
    {
        if (finishButton != null)
        {
            finishButton.interactable = false;
            Text btnText = finishButton.GetComponentInChildren<Text>();
            if (btnText != null) btnText.text = "...";
        }

        yield return new WaitForSeconds(2.0f);

        FinishMulligan();
    }
}
