using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [Header("Areas")]
    public RectTransform playerArea;
    public RectTransform enemyArea;

    [Header("UI")]
    public Text playerPointsText;
    public Text enemyPointsText;
    public Button passButton;

    private int playerPoints = 0, enemyPoints = 0;
    private bool playerHasPassed = false, enemyHasPassed = false;

    void Start()
    {
        passButton.onClick.AddListener(OnPlayerPass);
        UpdatePointsUI();
    }

    public void AddPlayerPoint(int amount)
    {
        playerPoints += amount;
        UpdatePointsUI();
    }

    public void AddEnemyPoints(int amount)
    {
        enemyPoints += amount;
        UpdatePointsUI();
    }

    void UpdatePointsUI()
    {
        playerPointsText.text = playerPoints.ToString();
        enemyPointsText.text = enemyPoints.ToString();
    }

    void OnPlayerPass()
    {
        playerHasPassed = true;
        passButton.interactable = false;
        Debug.Log("Gracz spasowa³");
        CheckRoundEnd();
    }

    //placeholder:
    public void OnEnemyPass()
    {
        enemyHasPassed = true;
        Debug.Log("Przeciwnik spasowa³");
        CheckRoundEnd();
    }

    void CheckRoundEnd()
    {
        if (playerHasPassed && enemyHasPassed)
        {
            //logika podsumowania rundy tutaj
            Debug.Log("Koniec rundy");
        }
    }
}
