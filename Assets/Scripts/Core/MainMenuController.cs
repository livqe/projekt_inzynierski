using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject deckSelectionPanel;
    public GameObject settingsPanel;
    public GameObject tutorialPanel;

    [Header("Deck Selection Elements")]
    public Transform deckButtonsContainer;
    public GameObject deckButtonPrefab;

    [Header("Buttons inside Deck Panel")]
    public Button createNewDeckButton;
    public Button backButton;

    [Header("UI")]
    MagicSceneFader sceneFader;
    public TMP_InputField playerNameInput;

    [Header("Animation")]
    public GameObject animator;

    private const string PLAYER_NAME_KEY = "PlayerName";

    private void Start()
    {
        //sceneFader = FindObjectOfType<MagicSceneFader>();

        //if (sceneFader == null)
        //Debug.LogError("Brak MagicSceneFader w scenie!");
        if (PlayerPrefs.HasKey(PLAYER_NAME_KEY))
        {
            string savedName = PlayerPrefs.GetString(PLAYER_NAME_KEY);
            if (playerNameInput != null) playerNameInput.text = savedName;

            GameSetup.playerName = savedName;
        }

        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (deckSelectionPanel != null) deckSelectionPanel.SetActive(false);

        if (createNewDeckButton != null) createNewDeckButton.onClick.AddListener(OnEditDeck);

        if (backButton != null) backButton.onClick.AddListener(BackToMenu);

        if (animator != null) animator.SetActive(false);
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
        if (animator != null) animator.SetActive(true);

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
        string nameToSave = "Bezimienny";

        if (playerNameInput != null && !string.IsNullOrEmpty(playerNameInput.text))
            nameToSave = playerNameInput.text;

        PlayerPrefs.SetString(PLAYER_NAME_KEY, nameToSave);
        PlayerPrefs.Save();

        GameSetup.playerName = nameToSave;
        GameSetup.selectedDeck = deck;

        SceneManager.LoadScene("Game");
        //sceneFader.FadeToScene("Game");
    }

    public void OnEditDeck()
    {
        SceneManager.LoadScene("DeckBuilder");
        //sceneFader.FadeToScene("DeckBuilder");
    }

    public void OnSettings()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
            var settingsCtrl = settingsPanel.GetComponent<SettingsMenuController>();
            if (settingsCtrl != null) settingsCtrl.InitializeSettings();
        }
    }

    public void OnTutorial()
    {
        if (tutorialPanel != null) tutorialPanel.SetActive(true);
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
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (tutorialPanel != null) tutorialPanel.SetActive(false);
        if (animator != null) animator.SetActive(false);
    }
}

public static class GameSetup
{
    public static SavedDeck selectedDeck;
    public static string playerName = "Gracz";
}

public static class SceneDataTransfer
{
    public static bool openInCreateMode = false;
}