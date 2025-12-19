using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UnityEditor;

public class DeckBuilderController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject deckSelectionPanel;
    public GameObject factionSelectionPanel;
    public GameObject editorPanel;

    [Header("UI Controls")]
    public Button saveButton;
    public TextMeshProUGUI warningText;

    [Header("Deck Selection UI")]
    public Transform deckListContainer;
    public GameObject deckSlotPrefab;

    [Header("Editor UI - Library")]
    public Transform libraryContainer;
    public GameObject libraryItemPrefab;

    [Header("Editor UI - Current Deck")]
    public Transform currentDeckContainer;
    public GameObject deckItemPrefab;
    public TextMeshProUGUI deckCountText;
    public TextMeshProUGUI currentDeckNameText;
    public TMP_InputField deckNameInput;

    [Header("Data")]
    public int maxDeckSize = 25;
    private List<SavedDeck> allDecks;
    private SavedDeck currentDeck;
    private List<CardData> allCardAssets;

    private List<BuilderLibraryItem> spawnedLibraryItem = new List<BuilderLibraryItem>();

    void Start()
    {
        allCardAssets = Resources.LoadAll<CardData>("CardData").ToList();
        allDecks = DeckStorage.LoadDecks();

        if (saveButton != null)
        {
            saveButton.onClick.AddListener(SaveAndExit);
        }

        ShowDeckSelection();
    }

    public void ShowDeckSelection()
    {
        deckSelectionPanel.SetActive(true);
        factionSelectionPanel.SetActive(false); 
        editorPanel.SetActive(false);
        RefreshDeckList();
    }

    public void ShowFactionSelection()
    {
        deckSelectionPanel.SetActive(false);
        factionSelectionPanel.SetActive(true);
        editorPanel.SetActive(false);
    }

    public void CreateNewDeck(Faction faction)
    {
        currentDeck = new SavedDeck("Nowa Talia", faction);
        allDecks.Add(currentDeck);

        OpenEditor(currentDeck);
    }

    public void OpenEditor(SavedDeck deck)
    {
        currentDeck = deck;
        deckSelectionPanel.SetActive(false);
        factionSelectionPanel.SetActive(false);
        editorPanel.SetActive(true);

        deckNameInput.text = currentDeck.deckName;
        currentDeckNameText.text = currentDeck.faction.ToString();

        RefreshLibrary();
        RefreshCurrentDeckUI();
        ValidateDeck();
    }

    private void RefreshLibrary()
    {
        foreach (Transform child in libraryContainer) Destroy(child.gameObject);
        spawnedLibraryItem.Clear();

        foreach (var card in allCardAssets)
        {
            if (card.cardName == "Promotorzy") continue;
            if (card.faction != currentDeck.faction && card.faction != Faction.Neutralne) continue;

            GameObject item = Instantiate(libraryItemPrefab, libraryContainer);
            
            item.transform.localScale = Vector3.one;
            item.transform.localPosition = Vector3.zero;
            
            BuilderLibraryItem script = item.GetComponent<BuilderLibraryItem>();
            script.Setup(card, this);
            
            spawnedLibraryItem.Add(script);
        }
    }

    private void RefreshCurrentDeckUI()
    {
        foreach (Transform child in currentDeckContainer) Destroy(child.gameObject);

        int count = 0;
        foreach (string cardId in currentDeck.cardIds)
        {
            CardData data = allCardAssets.Find(c => c.cardName == cardId);
            if (data != null)
            {
                GameObject item = Instantiate(deckItemPrefab, currentDeckContainer);

                item.transform.localScale = Vector3.one;
                item.transform.localPosition = Vector3.zero;

                item.GetComponent<BuilderDeckItem>().Setup(data, this);
                count++;
            }
        }

        deckCountText.text = $"{count} / {maxDeckSize}";
        deckCountText.color = (count == maxDeckSize) ? Color.green : Color.white;
    }

    public int GetCardCountInDeck(CardData card)
    {
        return currentDeck.cardIds.Count(id => id == card.cardName);
    }

    public void AddCardToDeck(CardData card)
    {
        if (currentDeck.cardIds.Count >= maxDeckSize)
        {
            Debug.Log("Talia jest pe³na.");
            return;
        }

        currentDeck.cardIds.Add(card.cardName);
        RefreshCurrentDeckUI();
        ValidateDeck();
        UpdateLibraryState();
    }

    public void RemoveCardFromDeck(CardData card, BuilderDeckItem itemVis)
    {
        currentDeck.cardIds.Remove(card.cardName);
        Destroy(itemVis.gameObject);

        deckCountText.text = $"{currentDeck.cardIds.Count} / {maxDeckSize}";

        ValidateDeck();
        UpdateLibraryState();
    }

    private void UpdateLibraryState()
    {
        foreach (var item in spawnedLibraryItem)
        {
            if (item != null) item.RefreshAvailability();
        }
    }

    private void ValidateDeck()
    {
        int currentCount = currentDeck.cardIds.Count;
        bool isValid = currentCount == maxDeckSize;

        saveButton.interactable = isValid;

        if (warningText != null)
        {
            warningText.text = isValid ? "" : "Nie masz wystarczaj¹cej iloœci kart.";
            warningText.color = isValid ? Color.white : Color.red;
        }
    }

    public void SaveAndExit()
    {
        currentDeck.deckName = deckNameInput.text;
        DeckStorage.SaveDecks(allDecks);
        ShowDeckSelection();
    }

    private void RefreshDeckList()
    {
        foreach (Transform child in deckListContainer) Destroy(child.gameObject);

        foreach (var deck in allDecks)
        {
            GameObject slot = Instantiate(deckSlotPrefab, deckListContainer);
            slot.GetComponentInChildren<TextMeshProUGUI>().text = $"{deck.deckName}";
            slot.GetComponent<Button>().onClick.AddListener(() => OpenEditor(deck));
        }
    }

    public void OnReturn()
    {
        SceneManager.LoadScene("Menu");
    }

    public void CreateDwarvesDeck() => CreateNewDeck(Faction.Krasnoludy);
    public void CreateElvesDeck() => CreateNewDeck(Faction.Elfy);
}
