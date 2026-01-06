using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class DeckBuilderController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject deckSelectionPanel;
    public GameObject factionSelectionPanel;
    public GameObject editorPanel;
    public GameObject exitConfirmationPanel;
    public GameObject deleteConfirmationPanel;

    [Header("UI Controls")]
    public Button saveButton;
    public TextMeshProUGUI warningText;

    [Header("Deck Selection UI")]
    public Transform deckListContainer;
    public GameObject deckSlotPrefab;

    [Header("Editor Library UI")]
    public Transform libraryContainer;
    public GameObject libraryItemPrefab;

    [Header("Editor Current Deck UI")]
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

    private SavedDeck deckToDelete;

    private string originalDeckName = "";
    private bool isEditingExistingDeck = false;

    void Start()
    {
        allCardAssets = Resources.LoadAll<CardData>("CardData").ToList();
        allDecks = DeckStorage.LoadDecks();

        if (saveButton != null)
        {
            saveButton.onClick.AddListener(SaveAndExit);
        }

        if (exitConfirmationPanel != null)
            exitConfirmationPanel.SetActive(false);

        if (SceneDataTransfer.openInCreateMode)
        {
            ShowFactionSelection();
            SceneDataTransfer.openInCreateMode = false;
        }
        else ShowDeckSelection();
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
        isEditingExistingDeck = false;
        originalDeckName = "";

        currentDeck = new SavedDeck("Nowa Talia", faction);

        OpenEditor();
    }

    public void RequestDeleteDeck(SavedDeck deck)
    {
        deckToDelete = deck;

        if (deleteConfirmationPanel != null) deleteConfirmationPanel.SetActive(true);
    }

    public void DeleteDeck()
    {
        if (deckToDelete != null)
        {
            if (currentDeck == deckToDelete) CreateNewDeck(deckToDelete.faction);

            allDecks.Remove(deckToDelete);
            DeckStorage.SaveDecks(allDecks);
            RefreshDeckList();
        }

        CloseDeletePanel();
    }

    public void CloseDeletePanel()
    {
        deckToDelete = null;
        if (deleteConfirmationPanel != null) deleteConfirmationPanel.SetActive(false);
    }

    public void OpenEditorForExisting(SavedDeck deck)
    {
        isEditingExistingDeck = true;
        originalDeckName = deck.deckName;
        currentDeck = deck;

        OpenEditor();
    }

    public void OpenEditor()
    {
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
        string deckName = deckNameInput.text.Trim();

        if (string.IsNullOrEmpty(deckName))
        {
            Debug.LogWarning("Nazwa talii nie mo¿ê byæ pusta.");
            return;
        }

        if (!isEditingExistingDeck || (isEditingExistingDeck && deckName != originalDeckName))
        {
            bool nameExists = false;

            foreach (var deck in allDecks)
            {
                if (deck.deckName == deckName)
                {
                    nameExists = true;
                    break;
                }
            }

            if (nameExists)
            {
                Debug.LogWarning($"Talia o nazwie {deckName} ju¿ istnieje.");
                if (warningText != null)
                {
                    warningText.text = "Taka nazwa ju¿ istnieje!";
                    warningText.color = Color.red;
                }
                return;
            }
        }

        currentDeck.deckName = deckName;

        if (!isEditingExistingDeck) allDecks.Add(currentDeck);

        DeckStorage.SaveDecks(allDecks);
        ShowDeckSelection();
    }

    private void RefreshDeckList()
    {
        foreach (Transform child in deckListContainer) Destroy(child.gameObject);

        foreach (var deck in allDecks)
        {
            GameObject slot = Instantiate(deckSlotPrefab, deckListContainer);

            var nameText = slot.transform.Find("DeckNameText");
            var factionText = slot.transform.Find("FactionText");

            if (nameText != null) nameText.GetComponent<TextMeshProUGUI>().text = $"{deck.deckName}";
            if (factionText != null)
            {
                factionText.GetComponent<TextMeshProUGUI>().text = $"{deck.faction.ToString()}";
                factionText.GetComponent<TextMeshProUGUI>().color = (deck.faction == Faction.Elfy) ? Color.green : Color.red;
            }

            Transform artImage = slot.transform.Find("ArtContainer/ArtworkImage");

            if (artImage != null)
            {
                Image imgComponent = artImage.GetComponent<Image>();
                DeckSlotVisual visual = slot.GetComponent<DeckSlotVisual>();

                if (imgComponent != null)
                {
                    imgComponent.sprite = (deck.faction == Faction.Elfy) ? visual.elfSprite : visual.dwarfSprite;
                }
            }

            Button mainButton = slot.GetComponent<Button>();
            if (mainButton != null) 
                mainButton.onClick.AddListener(() => OpenEditorForExisting(deck));

            Button[] buttons = slot.GetComponentsInChildren<Button>();
            foreach (var btn in buttons)
            {
                if (btn != mainButton && btn.gameObject.name.Contains("Delete"))
                    btn.onClick.AddListener(() => RequestDeleteDeck(deck));
            }
        }
    }

    public void OnReturn()
    {
        SceneManager.LoadScene("Menu");
    }

    public void OnDeckEditorReturn()
    {
        if (exitConfirmationPanel != null) exitConfirmationPanel.SetActive(true);
        else ConfirmExitWithoutSaving();
    }

    public void CancelExit()
    {
        if (exitConfirmationPanel != null) exitConfirmationPanel.SetActive(false);
    }

    public void ConfirmExitWithoutSaving()
    {
        SceneManager.LoadScene("Menu");
        SceneManager.LoadScene("DeckBuilder");
    }

    public void CreateDwarvesDeck() => CreateNewDeck(Faction.Krasnoludy);
    public void CreateElvesDeck() => CreateNewDeck(Faction.Elfy);
}
