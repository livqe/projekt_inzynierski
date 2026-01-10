using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    public TMP_InputField playerNameInput;

    [Header("Animation")]
    public GameObject animator;

    [Header("Tutorial Animation")]
    public CanvasGroup tutorialCanvasGroup;

    [SerializeField] private float tutorialAnimDuration = 0.6f;
    [SerializeField] private Vector3 tutorialStartScale = new Vector3(0.9f, 0.9f, 1f);

    private const string PLAYER_NAME_KEY = "PlayerName";

    private void Start()
    {
        if (MagicSceneFader.Instance == null) Debug.LogWarning("Brak MagicSceneFader.");

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
            LoadSceneWithFade("DeckBuilder");
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
        LoadSceneWithFade("DeckBuilder");
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

        LoadSceneWithFade("Game");
    }

    public void OnEditDeck()
    {
        LoadSceneWithFade("DeckBuilder");
    }

    public void OnSettings()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
            if (animator != null) animator.SetActive(true);
            var settingsCtrl = settingsPanel.GetComponent<SettingsMenuController>();
            if (settingsCtrl != null) settingsCtrl.InitializeSettings();
        }
    }

    public void OnTutorial()
    {
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(true);
            StopAllCoroutines();
            StartCoroutine(AnimateTutorialPanel());
        }


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

    public void OnCloseTutorial()
    {
        StopAllCoroutines();
        StartCoroutine(AnimateTutorialPanelClose());
    }

    private void LoadSceneWithFade(string sceneName)
    {
        if (MagicSceneFader.Instance != null) 
            MagicSceneFader.Instance.FadeToScene(sceneName);
        else
        {
            Debug.LogWarning("Fader nie znaleziony.");
            SceneManager.LoadScene(sceneName);
        }
    }

    private IEnumerator AnimateTutorialPanel()
    {
        float time = 0f;

        RectTransform rt = tutorialPanel.GetComponent<RectTransform>();

        Vector2 endPos = Vector2.zero;
        Vector2 startPos = endPos + new Vector2(0f, Screen.height);

        tutorialCanvasGroup.alpha = 0f;
        tutorialPanel.transform.localScale = tutorialStartScale;
        rt.anchoredPosition = startPos;

        while (time < tutorialAnimDuration)
        {
            time += Time.deltaTime;
            float t = time / tutorialAnimDuration;
            float eased = Mathf.SmoothStep(0f, 1f, t);

            tutorialCanvasGroup.alpha = eased;
            tutorialPanel.transform.localScale = Vector3.Lerp(tutorialStartScale, Vector3.one, eased);
            rt.anchoredPosition = Vector2.Lerp(startPos, endPos, eased);

            yield return null;
        }

        tutorialCanvasGroup.alpha = 1f;
        tutorialPanel.transform.localScale = Vector3.one;
        rt.anchoredPosition = endPos;
    }

    private IEnumerator AnimateTutorialPanelClose()
    {
        float time = 0f;

        RectTransform rt = tutorialPanel.GetComponent<RectTransform>();

        Vector2 currentPos = rt.anchoredPosition;
        Vector2 targetPos = currentPos + new Vector2(0f, Screen.height);

        while (time < tutorialAnimDuration)
        {
            time += Time.deltaTime;
            float t = time / tutorialAnimDuration;

            float eased = Mathf.SmoothStep(0f, 1f, t);

            tutorialCanvasGroup.alpha = Mathf.Lerp(1f, 0f, eased);
            tutorialPanel.transform.localScale = Vector3.Lerp(Vector3.one, tutorialStartScale, eased);
            rt.anchoredPosition = Vector2.Lerp(currentPos, targetPos, eased);

            yield return null;
        }

        tutorialCanvasGroup.alpha = 0f;
        tutorialPanel.SetActive(false);
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