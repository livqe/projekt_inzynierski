using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public enum GameState
{
    Normal,
    WaitingForTarget,
    Mulligan
}

public enum TargetAlignment
{
    Any,
    Friendly,
    Enemy
}

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    [Header("Players")]
    public Player player;
    public Player enemy;

    [Header("UI Managers")]
    public HandManager handManager;
    public MulliganController mulliganController;

    [Header("UI")]
    public TextMeshProUGUI playerPointsText;
    public TextMeshProUGUI enemyPointsText;
    public TextMeshProUGUI roundResultText;
    public GameObject roundResultPanel;
    public GameObject gameResultPanel;
    public TextMeshProUGUI gameResultText;
    public TextMeshProUGUI gameFinalPointsText;

    [Header("Game Config")]
    public int cardsToDrawOnStart = 10;
    public int roundsToWin = 2;
    public string mainMenuSceneName = "Menu";

    [Header("Board State")]
    public List<CardInstance> playerBoard = new List<CardInstance>();
    public List<CardInstance> enemyBoard = new List<CardInstance>();

    private int playerWins = 0;
    private int enemyWins = 0;
    private bool isGameEnded = false;

    [Header("Prefabs")]
    public GameObject cardSpritePrefab;

    [Header("Faction Ability")]
    public FactionAbility elfAbility;
    public FactionAbility dwarfAbility;

    public GameState currentState = GameState.Normal;
    private CardInstance pendingCardSource;
    private CardEffect pendingEffect;
    private List<CardInstance> selectedTargets = new List<CardInstance>();

    private bool playerHasPassed = false;
    private bool enemyHasPassed = false;

    [Header("Debug / Test Deck")]
    public List<CardData> startingDeckAssets;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        //tu trzeba podmieniæ na wybór frakcji w menu
        player = new Player("Gracz", Faction.Elfy);
        enemy = new Player("AI", Faction.Krasnoludy);

        if (roundResultPanel != null) roundResultPanel.SetActive(false);
        if (gameResultPanel != null) gameResultPanel.SetActive(false); 

        //faza wymiany kart na start tutaj

        StartGame();
    }

    private void Update()
    {
        if (isGameEnded)
        {
            bool clicked = false;

            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) clicked = true;
            else if (Input.GetMouseButtonDown(0)) clicked = true;

            if (clicked)
            {
                Debug.Log("Powrót do menu...");
                SceneManager.LoadScene(mainMenuSceneName);
            }
        }
    }

    private void StartGame()
    {
        foreach (CardData data in startingDeckAssets)
        {
            if (data != null)
            {
                CardInstance newCard = new CardInstance(data, player);
                player.cardsInDeck.Add(newCard);
            }
        }

        foreach (CardData data in startingDeckAssets)
        {
            if (data != null)
            {
                CardInstance newCard = new CardInstance(data, enemy);
                enemy.cardsInDeck.Add(newCard);
            }
        }

        HandManager tempHandManager = handManager;
        handManager = null;
        DrawCard(player, cardsToDrawOnStart);
        DrawCard(enemy, cardsToDrawOnStart);
        handManager = tempHandManager;

        StartMulliganPhase();
    }

    private void StartMulliganPhase()
    {
        Debug.Log("--- WYMIANA KART ---");
        currentState = GameState.Mulligan;

        if (mulliganController != null)
            mulliganController.StartMulligan(player);
        else
        {
            Debug.LogError("Brak MulliganController");
            OnMulliganFinished();
        }
    }

    public void OnMulliganFinished()
    {
        currentState = GameState.Normal;

        if (handManager != null)
        {
            foreach (var card in player.cardsInHand)
                handManager.AddCardToHandVisual(card);
        }

        Debug.Log("--- START GRY ---");
        StartPlayerTurn();
        UpdateUI();
    }

    public void StartPlayerTurn()
    {
        Debug.Log("Pocz¹tek tury gracza");
        CheckAutoPlayCards(player);
    }

    private void CheckAutoPlayCards(Player currentPlayer)
    {
        List<CardInstance> handCopy = new List<CardInstance>(currentPlayer.cardsInHand);
        foreach (CardInstance card in handCopy)
        {
            if (card.data.effect is ChanceAutoPlayEffect autoPlay)
            {
                if (autoPlay.TryAutoPlay(true))
                {
                    currentPlayer.cardsInHand.Remove(card);
                    PlayCard(card, true);
                }
            }
        }

        List<CardInstance> deckCopy = new List<CardInstance>(currentPlayer.cardsInDeck);
        foreach (CardInstance card in deckCopy)
        {
            if (card.data.effect is ChanceAutoPlayEffect autoPlay)
            {
                if (autoPlay.TryAutoPlay(false))
                {
                    currentPlayer.cardsInDeck.Remove(card);
                    PlayCard(card, true);
                }
            }
        }
    }

    public void PlayCard(CardInstance card, bool isPlayerPlayed, RangeType? targetRow = null, int insertIndex = -1)
    {
        Debug.Log($"[Game Controller] Zagrano kartê: {card.data.cardName}.");

        List<CardInstance> targetBoard = isPlayerPlayed ? playerBoard : enemyBoard;

        if (insertIndex >= 0 && insertIndex <= targetBoard.Count)
        {
            targetBoard.Insert(insertIndex, card);
        }
        else
        {
            targetBoard.Add(card);
        }

        if (card.data.effect != null)
        {
            Debug.Log($" -> Uruchomiono efekt: {card.data.effect.effectName}.");
            card.data.effect.ActivateEffect(this, card);
        }

        NotifyOtherCardsOnPlay(card);
        SpawnCardOnBoardVisual(card, isPlayerPlayed, targetRow, insertIndex);
        UpdateUI();
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
            if (!playerHasPassed) StartPlayerTurn();
            return;
        }

        Debug.Log("Tura przeciwnika.");

        //logika AI tutaj
        enemyHasPassed = true;
        Debug.Log("Przeciwnik pasuje rundê.");

        ProcessTurnEndEffect(enemyBoard);
        CheckRoundEnd();

        if (!playerHasPassed) StartPlayerTurn();
    }

    private void CheckRoundEnd()
    {
        if (playerHasPassed && enemyHasPassed)
        {
            StartCoroutine(EndRoundSequence());
        }
    }

    private IEnumerator EndRoundSequence()
    {
        Debug.Log("[GameController] Koniec rundy");

        int playerScore = CalculateScore(playerBoard);
        int enemyScore = CalculateScore(enemyBoard);
        string resultMsg = "";

        if (playerScore > enemyScore)
        {
            playerWins++;
            player.lostLastRound = false;
            enemy.lostLastRound = true;
            resultMsg = "Wygra³eœ rundê!";
        }
        else if (enemyScore > playerScore)
        {
            enemyWins++;
            player.lostLastRound = true;
            enemy.lostLastRound = false;
            resultMsg = "Przegra³eœ rundê!";
        }
        else
        {
            playerWins++;
            enemyWins++;
            player.lostLastRound= false;
            enemy.lostLastRound = false;
            resultMsg = "Remis";
        }

        ActivateFactionAbility(player);
        ActivateFactionAbility(enemy);

        Debug.Log($"Wynik: Gracz {playerWins} : {enemyWins} Przeciwnik");

        if (roundResultText != null) roundResultText.text = resultMsg;
        if (roundResultPanel != null) roundResultPanel.SetActive(true);

        yield return new WaitForSeconds(4.0f);

        if (roundResultPanel != null) roundResultPanel.SetActive(false);

        if (playerWins >= roundsToWin || enemyWins >= roundsToWin)
        {
            EndGame();
        }
        else
        {
            CleanUpBoard();
            StartNextRound();
        }
    }

    private void CleanUpBoard()
    {
        Debug.Log("Czyszczenie sto³u...");

        BoardRow[] rows = FindObjectsByType<BoardRow>(FindObjectsSortMode.None);
        
        foreach (var row in rows)
        {
            foreach (Transform child in row.transform)
            {
                if (child.name.Contains("Ghost")) continue;
                Destroy(child.gameObject);
            }
        }

        //mo¿e cmentarz tutaj
        playerBoard.Clear();
        enemyBoard.Clear();

        UpdateUI();
    }

    private void StartNextRound()
    {
        Debug.Log("Start nowej rundy");

        playerHasPassed = false;
        enemyHasPassed = false;
        currentState = GameState.Normal;

        if (PlayerLostLastRound(player))
        {
            StartEnemyTurn();
        }
        else
        {
            StartPlayerTurn();
        }
    }

    private void EndGame()
    {
        Debug.Log("--- KONIEC GRY ---");
        isGameEnded = true;

        string finalMsg = "";
        if (playerWins >= roundsToWin && enemyWins >= roundsToWin)
            finalMsg = "REMIS";
        else if (playerWins >= roundsToWin)
            finalMsg = "ZWYCIÊSTWO!";
        else
            finalMsg = "PORA¯KA...";

        if (gameResultText != null) gameResultText.text = finalMsg;
        if (gameFinalPointsText != null) gameFinalPointsText.text = $"{playerWins} : {enemyWins}";
        if (gameResultPanel != null) gameResultPanel.SetActive(true);

        currentState = GameState.WaitingForTarget;
    }

    private int CalculateScore(List<CardInstance> board)
    {
        int score = 0;
        foreach (var card in board) score += card.currentPower;
        return score;
    }

    private void ActivateFactionAbility(Player p)
    {
        FactionAbility ability = (p.faction == Faction.Elfy) ? elfAbility : dwarfAbility;
        if(ability != null) ability.OnRoundEnd(this, p);
    }

    public void OnCardDeath(CardInstance deadCard)
    {
        if (deadCard.data.effect is IOnDeathEffect selfDeathEffect)
        {
            selfDeathEffect.OnDeath(this, deadCard);
        }

        Debug.Log($"[GameController] Przetwarzanie œmierci karty: {deadCard.data.cardName}.");

        if (playerBoard.Contains(deadCard))
        {
            playerBoard.Remove(deadCard);
        }
        else if (enemyBoard.Contains(deadCard))
        {
            enemyBoard.Remove(deadCard);
        }

        //wizualne usuwanie karty z planszy tutaj

        NotifyOtherCardsOnDeath(deadCard);
        UpdateUI();
    }

    private void NotifyOtherCardsOnDeath(CardInstance deadCard)
    {
        List<CardInstance> allCards = new List<CardInstance>();
        allCards.AddRange(playerBoard);
        allCards.AddRange(enemyBoard);

        foreach (CardInstance card in allCards)
        {
            if (card.data.effect is IOnOtherCardDeathEffect deathEffect)
            {
                deathEffect.OnOtherCardDeath(this, card, deadCard);
            }
        }
    }

    public void UpdateUI()
    {
        int playerPoints = CalculateScore(playerBoard);
        int enemyPoints = CalculateScore(enemyBoard);

        if (playerPointsText != null) playerPointsText.text = playerPoints.ToString();
        if (enemyPointsText != null) enemyPointsText.text = enemyPoints.ToString();
    }

    public void DrawCard(Player drawingPlayer, int amount = 1)
    {
        for (int i = 0; i < amount; i++)
        {
            if (drawingPlayer.cardsInDeck.Count == 0)
            {
                Debug.Log($"[GameController] {player.playerName} nie ma ju¿ kart w talii.");
                return;
            }

            CardInstance drawnCard = drawingPlayer.cardsInDeck[0];
            drawingPlayer.cardsInDeck.RemoveAt(0);
            drawingPlayer.cardsInHand.Add(drawnCard);

            if (drawingPlayer == player && handManager != null)
                handManager.AddCardToHandVisual(drawnCard);
        }
        UpdateUI();
    }

    private void NotifyOtherCardsOnPlay(CardInstance newCard)
    {
        List<CardInstance> allCards = new List<CardInstance>();
        allCards.AddRange(playerBoard);
        allCards.AddRange(enemyBoard);

        foreach (var card in allCards)
        {
            if (card == newCard) continue;
            if (card.data.effect is IOnOtherCardPlayedEffect reactionEffect)
                reactionEffect.OnOtherCardPlayed(this, card, newCard);
        }
    }

    public void SpawnCardOnBoardVisual(CardInstance card, bool isPlayerPlayed, RangeType? specificRow, int insertIndex)
    {
        if (cardSpritePrefab == null) return;

        string zoneName = "";
        RangeType finalRow = card.data.range;

        if (specificRow.HasValue)
        {
            finalRow = specificRow.Value;
        }
        else if (card.data.range == RangeType.Dowolny)
        {
            finalRow = (Random.value > 0.5f) ? RangeType.Bliski : RangeType.Daleki;
        }

        if (isPlayerPlayed)
            zoneName = (finalRow == RangeType.Daleki) ? "PlayerRangeRow" : "PlayerMeleeRow";
        else
            zoneName = (finalRow == RangeType.Daleki) ? "EnemyRangeRow" : "EnemyMeleeRow";

        GameObject zoneObj = GameObject.Find(zoneName);
        if (zoneObj != null)
        {
            GameObject newCardObj = Instantiate(cardSpritePrefab, zoneObj.transform);
            if (insertIndex >= 0) newCardObj.transform.SetSiblingIndex(insertIndex);

            newCardObj.GetComponent<SpriteRenderer>().sprite = card.data.artwork;
            if (newCardObj.TryGetComponent<CardOnBoard>(out var cardOnBoard)) cardOnBoard.cardInstance = card;

            var visual = newCardObj.GetComponent<CardBoardVisual>();
            if (visual != null) visual.UpdateVisuals(card);

            newCardObj.transform.localPosition = Vector3.zero;
        }
    }

    public void StartTargeting(CardInstance source, CardEffect effect)
    {
        currentState = GameState.WaitingForTarget;
        pendingCardSource = source;
        pendingEffect = effect;
        selectedTargets.Clear();

        if (effect is ITargetableEffect cardTargetEffect)
        {
            Debug.Log($"-- Tryb Celowania -- Kliknij {cardTargetEffect.GetTargetCount()} cel(e). Typ: {cardTargetEffect.GetTargetAlignment()}.");
        }
        else if (effect is IRowTargetableEffect rowTargetableEffect)
        {
            Debug.Log($"-- Tryb Celowania -- Kliknij rz¹d.");

        }

        //tutaj jakieœ opcje UI (np. podœwietlanie celów)
    }

    public void CardClicked(CardInstance target)
    {
        if (currentState != GameState.WaitingForTarget) return;

        if (pendingEffect is ITargetableEffect targetEffect)
        {
            if (target == pendingCardSource)
            {
                Debug.LogWarning("Nie celuj w kartê u¿ywaj¹c¹ efektu.");
                return;
            }

            Debug.Log($"Wybrano cel: {target.data.cardName}.");

            TargetAlignment align = targetEffect.GetTargetAlignment();
            bool isMyCard = (target.owner == pendingCardSource.owner);

            if (align == TargetAlignment.Friendly && !isMyCard)
            {
                Debug.LogWarning("Wybra³eœ wrog¹ kartê zamiast sojusznika.");
                return;
            }
            if (align == TargetAlignment.Enemy && isMyCard)
            {
                Debug.LogWarning("Wybra³eœ swoj¹ kartê zamiast wrogiej.");
                return;
            }

            if (selectedTargets.Contains(target))
            {
                Debug.Log("Wybra³eœ ju¿ t¹ kartê.");
                return;
            }

            selectedTargets.Add(target);

            if (selectedTargets.Count >= targetEffect.GetTargetCount())
            {
                currentState = GameState.Normal;

                targetEffect.ExecuteWithTarget(new List<CardInstance>(selectedTargets));

                pendingCardSource = null;
                pendingEffect = null;
                selectedTargets.Clear();
                UpdateUI();
            }
        }
        else
        {
            Debug.Log("Klikniêto kartê, ale gra czeka na wybór rz¹du.");
        }
    }

    public void RowClicked(RangeType range, bool isPlayerRow)
    {
        if (currentState != GameState.WaitingForTarget) return;

        if (pendingEffect is IRowTargetableEffect rowEffect)
        {
            Debug.Log($"[GameController] Wybrano rz¹d: {range}.");

            currentState = GameState.Normal;
            rowEffect.ExecuteWithRowTarget(pendingCardSource, range, isPlayerRow);
            pendingCardSource = null;
            pendingEffect = null;
            UpdateUI();
        }
        else
        {
            Debug.Log("Klikniêto rz¹d, ale efekt oczekuje czegoœ innego.");
        }
    }

    private void ProcessTurnEndEffect(List<CardInstance> board)
    {
        foreach (CardInstance card in board)
        {
            if (card.data.effect != null && card.data.effect is IOnTurnEndEffect)
            {
                IOnTurnEndEffect turnEffect = (IOnTurnEndEffect)card.data.effect;
                turnEffect.OnTurnEnd(this, card);
            }
        }

        Player boardOwner = (board == playerBoard) ? player : enemy;
        FactionAbility ability = (boardOwner.faction == Faction.Elfy) ? elfAbility : dwarfAbility;

        if (ability != null)
        {
            ability.OnTurnEnd(this, boardOwner);
        }

        UpdateUI();
    }

    public bool PlayerLostLastRound(Player player) => player.lostLastRound;

    public List<CardInstance> GetPlayerCards(Player player) => player.cardsOnBoard;
}
