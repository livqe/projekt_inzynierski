using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public enum GameState
{
    Normal,
    WaitingForTarget
}

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    [Header("Players")]
    public Player player;
    public Player enemy;

    [Header("UI")]
    public Text playerPointsText;
    public Text enemyPointsText;

    [Header("Faction Ability")]
    public FactionAbility elfAbility;
    public FactionAbility dwarfAbility;

    [Header("Game State")]
    public GameState currentState = GameState.Normal;

    [Header("Board State")]
    public List<CardInstance> playerBoard = new List<CardInstance>();
    public List<CardInstance> enemyBoard = new List<CardInstance>();

    private CardInstance pendingCardSource;
    private CardEffect pendingEffect;

    private bool playerHasPassed = false, enemyHasPassed = false;

    void Start()
    {
        //tu trzeba podmieniæ na wybór frakcji w menu
        player = new Player("Gracz", Faction.Elfy);
        enemy = new Player("AI", Faction.Krasnoludy);

        UpdateUI();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }


    public void PlayCard(CardInstance card, bool isPlayerPlayed)
    {
        Debug.Log($"[Game Controller] Zagrano kartê: {card.data.cardName}.");

        if (isPlayerPlayed)
        {
            playerBoard.Add(card);

            //tutaj karty bêd¹ przenoszone na planszê (UI)
        }
        else
        {
            enemyBoard.Add(card);
        }

        if (card.data.effect != null)
        {
            Debug.Log($" -> Uruchomiono efekt: {card.data.effect.effectName}.");
            card.data.effect.ActivateEffect(this, card);
        }
        else
        {
            Debug.Log($" -> Karta {card.data.cardName} nie posiada efektu.");
        }

        UpdateUI();
    }

    public void StartTargeting(CardInstance source, CardEffect effect)
    {
        currentState = GameState.WaitingForTarget;
        pendingCardSource = source;
        pendingEffect = effect;
        Debug.Log("-- Tryb Celowania -- Kliknij cel");

        //tutaj jakieœ opcje UI (np. podœwietlanie celów)
    }

    public void CardClicked(CardInstance target)
    {
        if (currentState != GameState.WaitingForTarget) return;

        Debug.Log($"Wybrano cel: {target.data.cardName}.");

        currentState = GameState.Normal;

        if (pendingEffect is ITargetableEffect targetableEffect)
        {
            targetableEffect.ExecuteWithTarget(target);
        }
        else
        {
            Debug.Log("Efekt czeka³ na cel, ale nie obs³uguje celowania.");
        }

        pendingCardSource = null;
        pendingEffect = null;
        UpdateUI();
    }

    void UpdateUI()
    {
        Debug.Log("-- Odœwie¿anie planszy --");

        int playerPoints = 0;
        foreach (CardInstance card in playerBoard)
        {
            playerPoints += card.currentPower;
        }

        int enemyPoints = 0;
        foreach (CardInstance card in enemyBoard)
        {
            enemyPoints += card.currentPower;
        }

        if (playerPointsText != null)
        {
            playerPointsText.text = playerPoints.ToString();
        }
        else
        {
            Debug.Log("Nie przypisano 'playerPointsText");
        }

        if (enemyPointsText != null)
        {
            enemyPointsText.text = enemyPoints.ToString();
        }
        else
        {
            Debug.Log("Nie przypisano 'enemyPointsText");
        }

        foreach (var card in playerBoard)
            {
                Debug.Log($"[Stó³ Gracza] {card.data.cardName} (moc: {card.currentPower}, tarcza: {card.shield})");
            }
    }

    public void EndPlayerTurn()
    {
        if (playerHasPassed)
        {
            Debug.Log("Gracz spasowa³ w tej rundzie, nie mo¿e wykonywaæ ruchów.");
            return;
        }

        Debug.Log("[GameController] Gracz koñczy turê.");
        ProcessTurnEndEffect(playerBoard);

        StartEnemyTurn();
    }

    public void PlayerPassRound()
    {
        Debug.Log("[GameController] Gracz spasowa³");
        playerHasPassed = true;

        ProcessTurnEndEffect(playerBoard);
        CheckRoundEnd();

        if (!enemyHasPassed) StartEnemyTurn();
    }

    public void StartEnemyTurn()
    {
        if (enemyHasPassed)
        {
            Debug.Log("Przeciwnik spasowa³, powrót do gracza.");
            return;
        }

        Debug.Log("Tura przeciwnika.");

        //logika AI tutaj

        ProcessTurnEndEffect(enemyBoard);

        Debug.Log("Przeciwnik koñczy turê.");
    }

    private void CheckRoundEnd()
    {
        if (playerHasPassed && enemyHasPassed) EndRound();
    }

    private void EndRound()
    {
            //logika podsumowania rundy tutaj
            Debug.Log("[GameController] Koniec tury");

            ActivateFactionAbility(player);
            ActivateFactionAbility(enemy);

            playerHasPassed = false;
            playerHasPassed = false;

    }

    private void ActivateFactionAbility(Player player)
    {
        if(player.faction == Faction.Elfy && elfAbility != null)
            elfAbility.OnRoundEnd(this, player);
        else if (player.faction == Faction.Krasnoludy && dwarfAbility != null)
            dwarfAbility.OnRoundEnd(this, player);
    }

    private void ProcessTurnEndEffect(List<CardInstance> board)
    {
        Debug.Log("Przetwarzanie efektów koñca tury.");

        foreach (CardInstance card in board)
        {
            if (card.data.effect != null && card.data.effect is IOnTurnEndEffect)
            {
                IOnTurnEndEffect turnEffect = (IOnTurnEndEffect)card.data.effect;
                turnEffect.OnTurnEnd(this, card);
            }
        }
        UpdateUI();
    }

    public bool PlayerLostLastRound(Player player) => player.lostLastRound;

    public List<CardInstance> GetPlayerCards(Player player) => player.cardsOnBoard;
}
