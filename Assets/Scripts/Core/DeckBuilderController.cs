using UnityEngine;
using UnityEngine.SceneManagement;

public class DeckBuilderController : MonoBehaviour
{
    public Transform cardParent;
    public GameObject cardPrefab;

    private void Start()
    {
        LoadAllCards();
    }

    void LoadAllCards()
    {
        CardData[] allCards = Resources.LoadAll<CardData>("CardData");

        foreach (var data in allCards)
        {
            var cardObj = Instantiate(cardPrefab, cardParent);
            var view = cardObj.GetComponent<CardView>();
            var tempDisplayInstance = new CardInstance(data, null);
            
            view.LoadCardData(tempDisplayInstance);
        }
    }

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
