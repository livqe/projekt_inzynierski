using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject deckSelectionPanel;

    [Header("Deck Selection Elements")]
    public Transform deckButtonsContainer;
    public GameObject deckButtonPrefab;

    [Header("Buttons inside Deck Panel")]
    public Button createNewDeckButton;
    public Button backButton;
    MagicSceneFader sceneFader;


    private void Start()
    {
        //sceneFader = FindObjectOfType<MagicSceneFader>();

        //if (sceneFader == null)
            //Debug.LogError("Brak MagicSceneFader w scenie!");
        
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (deckSelectionPanel != null) deckSelectionPanel.SetActive(false);

        if (createNewDeckButton != null) createNewDeckButton.onClick.AddListener(OnEditDeck);

        if (backButton != null) backButton.onClick.AddListener(BackToMenu);
    }
    public void OnNewGame()
    {
        List<SavedDeck> decks = DeckStorage.LoadDecks();

        if (decks == null || decks.Count == 0)
        {
            Debug.Log("Brak talii. Idziemy do edytora.");
            SceneManager.LoadScene("DeckBuilder");
            //sceneFader.FadeToScene("DeckBuilder");
        }
        else
        {
            ShowDeckSelection(decks);
        }
    }

    private void ShowDeckSelection(List<SavedDeck> decks)
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (deckSelectionPanel != null) deckSelectionPanel.SetActive(true);

        foreach (Transform child in deckButtonsContainer) Destroy(child.gameObject);

        foreach (var deck in decks)
        {
            GameObject btnObj = Instantiate(deckButtonPrefab, deckButtonsContainer);

            DeckSlotVisual slot = btnObj.GetComponent<DeckSlotVisual>();
            if (slot != null) slot.Setup(deck, StartGameWithDeck);
        }
    }

    public void CreateNewDeckButton()
    {
        SceneDataTransfer.openInCreateMode = true;
        SceneManager.LoadScene("DeckBuilder");
        //sceneFader.FadeToScene("DeckBuilder");
    }

    private void StartGameWithDeck(SavedDeck deck)
    {
        GameSetup.selectedDeck = deck;
        SceneManager.LoadScene("Game");
        //sceneFader.FadeToScene("Game");
    }

    public void OnEditDeck()
    {
        SceneManager.LoadScene("DeckBuilder");
        //sceneFader.FadeToScene("DeckBuilder");
    }

    public void OnExit()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    public void BackToMenu()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (deckSelectionPanel != null) deckSelectionPanel.SetActive(false);
    }
}

public static class GameSetup
{
    public static SavedDeck selectedDeck;
}

public static class SceneDataTransfer
{
    public static bool openInCreateMode = false;
}