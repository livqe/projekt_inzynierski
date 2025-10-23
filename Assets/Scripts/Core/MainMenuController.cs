using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void OnNewGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void OnEditDeck()
    {
        SceneManager.LoadScene("DeckBuilder");
    }

    public void OnExit()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
