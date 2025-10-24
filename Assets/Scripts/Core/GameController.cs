using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    [Header("Players")]
    public Player player;
    public Player enemy;
    
    [Header("Areas")]
    public RectTransform playerArea;
    public RectTransform enemyArea;

    [Header("UI")]
    public Text playerPointsText;
    public Text enemyPointsText;
    public Button passButton;

    [Header("Faction Ability")]
    public FactionAbility elfAbility;
    public FactionAbility dwarfAbility;

    private bool playerHasPassed = false, enemyHasPassed = false;

    void Start()
    {
        //tu trzeba podmieniæ na wybór frakcji w menu
        player = new Player("Gracz", Faction.Elfy);
        enemy = new Player("AI", Faction.Krasnoludy);

        passButton.onClick.AddListener(OnPlayerPass);
        UpdatePointsUI();
    }

    public void PlayCard(CardData card)
    {
        Debug.Log($"Zagrano kartê: {card.cardName}");

        var instance = new CardInstance(card, player);
        player.AddCardToBoard(instance);
        UpdatePointsUI();

        if (card.effect != null)
        {
            card.effect.ActivateEffect(this, instance);
        }
    }

    public void AddPlayerPoint(int amount)
    {
        player.totalPoints += amount;
        UpdatePointsUI();
    }

    public void AddEnemyPoints(int amount)
    {
        enemy.totalPoints += amount;
        UpdatePointsUI();
    }

    void UpdatePointsUI()
    {
        playerPointsText.text = player.totalPoints.ToString();
        enemyPointsText.text = enemy.totalPoints.ToString();
    }

    void OnPlayerPass()
    {
        playerHasPassed = true;
        passButton.interactable = false;
        Debug.Log("Gracz spasowa³");
        EndRound();
    }

    //placeholder:
    public void OnEnemyPass()
    {
        enemyHasPassed = true;
        Debug.Log("Przeciwnik spasowa³");
        EndRound();
    }

    void EndRound()
    {
        if (playerHasPassed && enemyHasPassed)
        {
            //logika podsumowania rundy tutaj
            Debug.Log("Koniec rundy");

            ActivateFactionAbility(player);
            ActivateFactionAbility(enemy);

            playerHasPassed = false;
            playerHasPassed = false;
        }

    }

    private void ActivateFactionAbility(Player player)
    {
        if(player.faction == Faction.Elfy && elfAbility != null)
            elfAbility.OnRoundEnd(this, player);
        else if (player.faction == Faction.Krasnoludy && dwarfAbility != null)
            dwarfAbility.OnRoundEnd(this, player);
    }

    public bool PlayerLostLastRound(Player player) => player.lostLastRound;

    public List<CardInstance> GetPlayerCards(Player player) => player.cardsOnBoard;
}
