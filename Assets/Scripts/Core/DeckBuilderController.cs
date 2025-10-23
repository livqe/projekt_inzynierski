using UnityEngine;
using UnityEngine.SceneManagement;

public class DeckBuilderController : MonoBehaviour
{
    public void OnReturn()
    {
        SceneManager.LoadScene("Menu");
    }

    public void OnSaveDeck()
    {
        //zapisywanie talii tutaj
        Debug.Log("Zapisano taliê");
    }
}
