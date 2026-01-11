using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class MenuController : MonoBehaviour
{
    [Header("UI")]
    public GameObject menuPanel;
    public GameObject confirmationPanel;
    public GameObject settingsPanel;

    public static bool IsGamePaused = false;

    void Start()
    {
        if (menuPanel != null) menuPanel.SetActive(false);
        if (confirmationPanel != null) confirmationPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);

        Time.timeScale = 1f;
        IsGamePaused = false;
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (confirmationPanel != null && confirmationPanel.activeSelf) CancelSurrender();
            else if (settingsPanel != null && settingsPanel.activeSelf) CloseSettings();
            else if (IsGamePaused) Resume();
            else Pause();
        }
    }

    public void Resume()
    {
        if (menuPanel != null) menuPanel.SetActive(false);
        if (confirmationPanel != null) confirmationPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);

        Time.timeScale = 1f;
        IsGamePaused = false;
    }

    public void Pause()
    {
        if (menuPanel != null) menuPanel.SetActive(true);
        if (confirmationPanel != null) confirmationPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);

        Time.timeScale = 0f;
        IsGamePaused = true;
    }

    public void OnExitClicked()
    {
        if (confirmationPanel != null) confirmationPanel.SetActive(true);
    }

    public void CancelSurrender()
    {
        if (confirmationPanel != null) confirmationPanel.SetActive(false);
    }

    public void Exit()
    {
        Time.timeScale = 1f;
        IsGamePaused = false;

        MagicSceneFader.Instance.FadeToScene("Menu");
    }

    public void OpenSettings()
    {
        if (menuPanel != null) menuPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (menuPanel != null) menuPanel.SetActive(true);
    }
}
