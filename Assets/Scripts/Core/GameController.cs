using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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

[System.Serializable]
public struct FactionVisuals
{
    public Faction faction;
    public Sprite cardBack;
    public Sprite avatarSprite;
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
    public Image gameResultPlayerAvatar;
    public Image gameResultEnemyAvatar;
    public DeckVisualizer playerDeckVisual;
    public DeckVisualizer enemyDeckVisual;
    public OpponentHandManager enemyHandVisual;
    public RoundGemVisual playerRoundGem;
    public RoundGemVisual enemyRoundGem;
    public Image playerAvatarImage;
    public Image enemyAvatarImage;

    [Header("Prefabs")]
    public GameObject cardSpritePrefab;

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

    [Header("Faction Assets")]
    public FactionAbility elfAbility;
    public FactionAbility dwarfAbility;
    public List<FactionVisuals> factionVisuals;

    public GameState currentState = GameState.Normal;
    private CardInstance pendingCardSource;
    private CardEffect pendingEffect;
    private List<CardInstance> selectedTargets = new List<CardInstance>();

    private int pendingCardHandIndex = -1;

    private bool playerHasPassed = false;
    private bool enemyHasPassed = false;
    public bool isPlayerTurn = false;

    private bool hasPlayedCardThisTurn = false;

    public bool isDragging = false;
    private float interactionalBlockTimer = 0f;

    [Header("Secret Card")]
    public CardData promoterCard;
    [Range(0f, 1f)] public float promoterChance = 0.05f;
    [Range(0f, 1f)] public float promoterDrawChance = 0.001f;

    [Header("End Game Animation")]
    [SerializeField] private float animationDuration = 0.35f;
    [SerializeField] private Vector3 startScale = new Vector3(0.9f, 0.9f, 0.9f);

    private CanvasGroup resultCanvasGroup;

    [SerializeField] private Image gameResultImage;
    [SerializeField] private Sprite winSprite;
    [SerializeField] private Sprite loseSprite;
    [SerializeField] private Sprite drawSprite;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioClip loseSound;
    [SerializeField] private AudioClip drawSound;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // ---- DODANE: inicjalizacja panelu koñca gry ----
        if (gameResultPanel != null)
        {
            resultCanvasGroup = gameResultPanel.GetComponent<CanvasGroup>();
            if (resultCanvasGroup == null)
                resultCanvasGroup = gameResultPanel.AddComponent<CanvasGroup>();

            resultCanvasGroup.alpha = 0f;
            gameResultPanel.transform.localScale = startScale;
            gameResultPanel.SetActive(false);
        }
    }


    void Start()
    {

        if (GameSetup.selectedDeck != null)
        {
            Debug.Log($"Wczytywanie talii gracza: {GameSetup.selectedDeck.deckName}.");

            player = new Player("Gracz", GameSetup.selectedDeck.faction);

            BuildPlayerDeck(GameSetup.selectedDeck);
        }
        else
        {
            Debug.LogWarning("Brak wybranej talii.");
        }

        Faction aiFaction = (player.faction == Faction.Elfy) ? Faction.Krasnoludy : Faction.Elfy;
        enemy = new Player("AI", aiFaction);

        Debug.Log($"Frakcja gracza: {player.faction}. Frakcja AI: {enemy.faction}.");

        GenerateAIDeck(enemy, aiFaction);

        if (roundResultPanel != null) roundResultPanel.SetActive(false);
        if (gameResultPanel != null) gameResultPanel.SetActive(false); 

        StartGame();
    }

    private void Update()
    {
        if (interactionalBlockTimer > 0) interactionalBlockTimer -= Time.deltaTime;

        if (isGameEnded)
        {
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
                SceneManager.LoadScene(mainMenuSceneName);
        }

        if (currentState == GameState.WaitingForTarget)
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
                CancelPlay();
        }
    }

    private void BuildPlayerDeck(SavedDeck savedDeck)
    {
        player.cardsInDeck.Clear();
        var allCards = Resources.LoadAll<CardData>("CardData");

        foreach (string id in savedDeck.cardIds)
        {
            CardData data = System.Array.Find(allCards, x => x.cardName == id);
            if (data != null)
            {
                player.cardsInDeck.Add(new CardInstance(data, player));
            }
        }
    }

    private void GenerateAIDeck(Player aiPlayer, Faction faction)
    {
        aiPlayer.cardsInDeck.Clear();
        CardData[] allCards = Resources.LoadAll<CardData>("CardData");

        List<CardData> factionCards = new List<CardData>();
        foreach (var card in allCards)
        {
            if (card.faction == faction || card.faction == Faction.Neutralne)
            {
                if (card.cardName == "Promotorzy") continue;
                factionCards.Add(card);
            }
        }

        Dictionary<string, int> cardsAddedCount = new Dictionary<string, int>();

        int attempts = 0;
        while (aiPlayer.cardsInDeck.Count < 25 && attempts < 1000)
        {
            attempts++;
            if (factionCards.Count == 0) break;

            CardData randomCard = factionCards[Random.Range(0, factionCards.Count)];

            int maxCopies = 1;
            maxCopies = randomCard.maxCopies;

            if (!cardsAddedCount.ContainsKey(randomCard.cardName))
                cardsAddedCount[randomCard.cardName] = 0;

            if (cardsAddedCount[randomCard.cardName] < maxCopies)
            {
                aiPlayer.cardsInDeck.Add(new CardInstance(randomCard, aiPlayer));
                cardsAddedCount[randomCard.cardName]++;
            }
        }

        Debug.Log($"Wygenerowano taliê AI: {aiPlayer.cardsInDeck.Count} kart.");
    }

    private void StartGame()
    {
        if (promoterCard != null)
        {
            if (Random.value <= promoterChance)
            {
                Debug.Log("Robi siê powa¿nie. Promotorzy do³¹czyli do talii gracza.");
                player.cardsInDeck.Add(new CardInstance(promoterCard, player));
            }

            if (Random.value <= promoterChance)
            {
                Debug.Log("Robi siê powa¿nie. Promotorzy do³¹czyli do talii przeciwnika.");
                enemy.cardsInDeck.Add(new CardInstance(promoterCard, enemy));
            }
        }

        Sprite playerBack = GetCardBackForFaction(player.faction);
        Sprite enemyBack = GetCardBackForFaction(enemy.faction);

        if (playerDeckVisual != null) playerDeckVisual.SetFactionVisual(playerBack);
        if (enemyDeckVisual != null) enemyDeckVisual.SetFactionVisual(enemyBack);
        if (enemyHandVisual != null) enemyHandVisual.setCardBackSprite(enemyBack);

        if (playerAvatarImage != null) playerAvatarImage.sprite = GetAvatarForFaction(player.faction);
        if (enemyAvatarImage != null) enemyAvatarImage.sprite = GetAvatarForFaction(enemy.faction);

        ShuffleDeck(player.cardsInDeck);
        ShuffleDeck(enemy.cardsInDeck);

        HandManager tempHandManager = handManager;
        handManager = null;

        if (playerDeckVisual != null) playerDeckVisual.UpdateCount(player.cardsInDeck.Count);
        if (enemyDeckVisual != null) enemyDeckVisual.UpdateCount(enemy.cardsInDeck.Count);
        if (enemyHandVisual != null) enemyHandVisual.ClearHand();

        DrawCard(player, cardsToDrawOnStart);
        DrawCard(enemy, cardsToDrawOnStart);

        handManager = tempHandManager;

        StartMulliganPhase();
    }

    private void ShuffleDeck(List<CardInstance> deck)
    {
        for (int i = 0; i < deck.Count; i++)
        {
            CardInstance temp = deck[i];
            int randomIndex = Random.Range(i, deck.Count);
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }

    private Sprite GetCardBackForFaction(Faction f)
    {
        foreach (var visual in factionVisuals)
            if (visual.faction == f) return visual.cardBack;

        return null;
    }

    private Sprite GetAvatarForFaction(Faction f)
    {
        foreach (var item in factionVisuals)
            if (item.faction == f) return item.avatarSprite;

        return null;
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
        isPlayerTurn = true;

        Debug.Log("Pocz¹tek tury gracza");

        hasPlayedCardThisTurn = false;

        if (player.cardsInHand.Count == 0)
        {
            Debug.Log("Brak kart w rêce. Musisz spasowaæ.");
            CheckAutoPlayCards(player);
            PlayerPassRound();
            return;
        }

        CheckAutoPlayCards(player);
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

        if (AIController.Instance != null)
            AIController.Instance.MakeDecision();
        else
            Debug.LogError("Brak AIController na scenie!");

        CheckAutoPlayCards(enemy);
    }

    private void CheckAutoPlayCards(Player currentPlayer)
    {
        List<CardInstance> handCopy = new List<CardInstance>(currentPlayer.cardsInHand);
        foreach (CardInstance card in handCopy)
        {
            if (card.data.effect is ChanceAutoPlayEffect autoPlay)
            {
                autoPlay.TryAutoPlay(this, card, true);
            }
        }

        List<CardInstance> deckCopy = new List<CardInstance>(currentPlayer.cardsInDeck);
        foreach (CardInstance card in deckCopy)
        {
            if (card.data.effect is ChanceAutoPlayEffect autoPlay)
            {
                autoPlay.TryAutoPlay(this, card, false);
            }
        }
    }

    public void PlayCard(CardInstance card, bool isPlayerPlayed, bool countsAsAction, RangeType? targetRow = null, int insertIndex = -1)
    {
        if (isPlayerPlayed && countsAsAction)
        {
            if (hasPlayedCardThisTurn)
            {
                Debug.LogWarning("Ju¿ zagra³eœ kartê w tej turze.");

                if (handManager != null) handManager.AddCardToHandVisual(card);
                return;
            }

            if (player.cardsInHand.Contains(card))
            {
                pendingCardHandIndex = player.cardsInHand.IndexOf(card);
                player.cardsInHand.Remove(card);
            }

            hasPlayedCardThisTurn = true;
        }

        if (isPlayerPlayed && player.cardsInHand.Contains(card))
        {
            player.cardsInHand.Remove(card);
            if (handManager != null) handManager.RemoveCardVisual(card);
        }
        else if (!isPlayerPlayed && enemy.cardsInHand.Contains(card))
        {
            enemy.cardsInHand.Remove(card);
            if (enemyHandVisual != null) enemyHandVisual.RemoveCard();
        }

        if (isPlayerPlayed & countsAsAction) hasPlayedCardThisTurn = true;

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

        SpawnCardOnBoardVisual(card, isPlayerPlayed, targetRow, insertIndex);

        if (card.data.effect != null)
        {
            Debug.Log($" -> Uruchomiono efekt: {card.data.effect.effectName}.");
            card.data.effect.ActivateEffect(this, card);
        }

        NotifyOtherCardsOnPlay(card);
        UpdateUI();

        if (isPlayerPlayed && currentState == GameState.Normal)
        {
            CheckForAutoPass();
        }

        if (playerDeckVisual != null) playerDeckVisual.UpdateCount(player.cardsInDeck.Count);
        if (enemyDeckVisual != null) enemyDeckVisual.UpdateCount(enemy.cardsInDeck.Count);
    }

    public void CancelPlay()
    {
        if (pendingCardSource == null) return;

        ClearAllHighlights();

        Debug.Log("Anulowanie zagrania karty.");

        CardInstance cardToReturn = pendingCardSource;

        if (playerBoard.Contains(cardToReturn)) playerBoard.Remove(cardToReturn);
        else if (enemyBoard.Contains(cardToReturn)) enemyBoard.Remove(cardToReturn);

        CardBoardVisual[] visuals = FindObjectsByType<CardBoardVisual>(FindObjectsSortMode.None);
        foreach (var v in visuals)
        {
            var cob = v.GetComponent<CardOnBoard>();
            if (cob != null && cob.cardInstance == cardToReturn)
            {
                Destroy(v.gameObject);
                break;
            }
        }

        if (pendingCardHandIndex >= 0 && pendingCardHandIndex <= player.cardsInHand.Count)
            player.cardsInHand.Insert(pendingCardHandIndex, cardToReturn);
        else
            player.cardsInHand.Add(cardToReturn);

        if (handManager != null) handManager.AddCardToHandVisual(cardToReturn, pendingCardHandIndex);

        hasPlayedCardThisTurn = false;
        currentState = GameState.Normal;
        pendingCardSource = null;
        pendingEffect = null;
        selectedTargets.Clear();
        pendingCardHandIndex = -1;

        UpdateUI();
    }

    public void BlockInteractionFor(float duration)
    {
        interactionalBlockTimer = duration;
    }

    public bool CanInteract()
    {
        if (interactionalBlockTimer > 0) return false;
        if (isDragging) return false;
        return true;
    }

    public void EndPlayerTurn()
    {
        if (!isPlayerTurn) return;

        if (playerHasPassed)
        {
            Debug.Log("Gracz spasowa³ w tej rundzie, nie mo¿e wykonywaæ ruchów.");
            return;
        }

        if (player.cardsInHand.Count > 0 && !hasPlayedCardThisTurn)
        {
            Debug.Log("Nie mo¿esz spasowaæ bez zagrania karty.");
            if (handManager != null)
            {

            }
            return;
        }

        if (player.cardsInHand.Count == 0 && !hasPlayedCardThisTurn)
        {
            Debug.Log("Brak kart. Pasowanie rundy.");
            PlayerPassRound();
            return;
        }

        if (currentState == GameState.WaitingForTarget)
        {
            CancelPlay();
        }
        else
        {
            ClearAllHighlights();
        }

        isPlayerTurn = false;

        Debug.Log("[GameController] Gracz koñczy turê.");
        ProcessTurnEndEffect(playerBoard);
        StartEnemyTurn();
    }

    public void EndEnemyTurn()
    {
        Debug.Log("[GameController] AI koñczy turê.");
        ProcessTurnEndEffect(enemyBoard);

        if (!playerHasPassed) StartPlayerTurn();
        else StartEnemyTurn();
    }

    public void PlayerPassRound()
    {
        isPlayerTurn = false;

        Debug.Log("[GameController] Gracz spasowa³");
        playerHasPassed = true;

        ProcessTurnEndEffect(playerBoard);
        CheckRoundEnd();

        if (!enemyHasPassed) StartEnemyTurn();
    }

    public void EnemyPassRound()
    {
        Debug.Log("[GameController] AI pasuje rundê.");
        enemyHasPassed = true;

        ProcessTurnEndEffect(enemyBoard);
        CheckRoundEnd();

        if (!playerHasPassed) StartPlayerTurn();
    }

    public void CheckForAutoPass()
    {
        if (player.cardsInHand.Count == 0)
        {
            Debug.Log("Rêka pusta. Automatyczny pas rundy.");
            PlayerPassRound();
        }
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

        if (playerRoundGem != null) playerRoundGem.UpdateLives(enemyWins);
        if (enemyRoundGem != null) enemyRoundGem.UpdateLives(playerWins);

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

        ClearAllHighlights();

        BoardRow[] rows = FindObjectsByType<BoardRow>(FindObjectsSortMode.None);
        
        foreach (var row in rows)
        {
            Transform container = row.transform;
            if (row.linkedLayout != null) container = row.linkedLayout.transform;

            for (int i = container.childCount - 1; i >= 0; i--)
            {
                Transform child = container.GetChild(i);
                if (child.name.Contains("Ghost")) continue;
                Destroy(child.gameObject);
            }

            row.GetComponent<BoardRow>().linkedLayout?.RemoveGhost();
        }

        //mo¿e cmentarz tutaj
        playerBoard.Clear();
        enemyBoard.Clear();

        hasPlayedCardThisTurn = false;

        UpdateUI();
    }

    private void StartNextRound()
    {
        Debug.Log("Start nowej rundy");

        playerHasPassed = false;
        enemyHasPassed = false;

        currentState = GameState.Normal;
        pendingCardSource = null;
        pendingEffect = null;
        selectedTargets.Clear();

        if (PlayerLostLastRound(player))
        {
            StartEnemyTurn();
        }
        else
        {
            StartPlayerTurn();
        }
    }

    private IEnumerator AnimateEndGamePanel()
    {
        float time = 0f;

        resultCanvasGroup.alpha = 0f;
        gameResultPanel.transform.localScale = startScale;

        while (time < animationDuration)
        {
            time += Time.deltaTime;
            float t = time / animationDuration;

            // easing (lekko miêkkie wejœcie)
            float eased = Mathf.SmoothStep(0f, 1f, t);

            resultCanvasGroup.alpha = eased;
            gameResultPanel.transform.localScale = Vector3.Lerp(startScale, Vector3.one, eased);

            yield return null;
        }

        resultCanvasGroup.alpha = 1f;
        gameResultPanel.transform.localScale = Vector3.one;
    }

    private void EndGame()
    {
        Debug.Log("--- KONIEC GRY ---");
        isGameEnded = true;

        string finalMsg = "";
        Sprite resultSprite = null;
        AudioClip resultSound = null;

        if (playerWins >= roundsToWin && enemyWins >= roundsToWin)
        {
            finalMsg = "REMIS";
            resultSprite = drawSprite;
            resultSound = drawSound;
        }
        else if (playerWins >= roundsToWin)
        {
            finalMsg = "ZWYCIÊSTWO!";
            resultSprite = winSprite;
            resultSound = winSound;
        }
        else
        {
            finalMsg = "PORA¯KA...";
            resultSprite = loseSprite;
            resultSound = loseSound;
        }

        if (gameResultText != null)
            gameResultText.text = finalMsg;

        if (gameFinalPointsText != null)
            gameFinalPointsText.text = $"{playerWins} : {enemyWins}";

        if (gameResultPlayerAvatar != null && playerAvatarImage != null)
            gameResultPlayerAvatar.sprite = playerAvatarImage.sprite;

        if (gameResultEnemyAvatar != null && enemyAvatarImage != null)
            gameResultEnemyAvatar.sprite = enemyAvatarImage.sprite;

        if (gameResultImage != null && resultSprite != null)
            gameResultImage.sprite = resultSprite;

        if (gameResultPanel != null)
        {
            gameResultPanel.SetActive(true);
            StopAllCoroutines();
            StartCoroutine(AnimateEndGamePanel());
        }

        if (audioSource != null && resultSound != null)
            audioSource.PlayOneShot(resultSound);

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

        if (playerBoard.Contains(deadCard)) playerBoard.Remove(deadCard);
        else if (enemyBoard.Contains(deadCard)) enemyBoard.Remove(deadCard);

        CardOnBoard[] allVisuals = FindObjectsByType<CardOnBoard>(FindObjectsSortMode.None);
        foreach (var visual in allVisuals)
        {
            if (visual.cardInstance == deadCard)
            {
                Destroy(visual.gameObject);
                break;
            }
        }

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

        BoardRow[] rows = FindObjectsByType<BoardRow>(FindObjectsSortMode.None);
        foreach (var row in rows)
        {
            Transform cardsContainer = row.transform;

            if (row.linkedLayout != null) cardsContainer = row.linkedLayout.transform;

            foreach (Transform child in cardsContainer)
            {
                if (child.name.Contains("Ghost")) continue;

                CardOnBoard dataHolder = child.GetComponent<CardOnBoard>();
                CardBoardVisual visual = child.GetComponent<CardBoardVisual>();

                if (dataHolder != null && visual != null && dataHolder.cardInstance != null)
                    visual.UpdateVisuals(dataHolder.cardInstance);
            }
        }
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

            CardInstance cardToDraw = null;

            CardInstance promoterInDeck = drawingPlayer.cardsInDeck.Find(c => c.data == promoterCard);

            if (promoterInDeck != null)
            {
                if (Random.value <= promoterDrawChance)
                    cardToDraw = promoterInDeck;
            }

            if (cardToDraw == null)
            {
                CardInstance topCard = drawingPlayer.cardsInDeck[0];

                if (topCard.data == promoterCard)
                {
                    if (drawingPlayer.cardsInDeck.Count > 1)
                    {
                        cardToDraw = drawingPlayer.cardsInDeck[1];
                    }
                    else
                    {
                        cardToDraw = topCard;
                    }
                }
                else
                    cardToDraw = topCard;
            }

            if (cardToDraw != null)
            {

                drawingPlayer.cardsInDeck.Remove(cardToDraw);
                drawingPlayer.cardsInHand.Add(cardToDraw);

                Debug.Log($"Gracz {drawingPlayer.playerName} dobiera: {cardToDraw.data.cardName}");

                if (drawingPlayer == player)
                {
                    if (handManager != null) handManager.AddCardToHandVisual(cardToDraw);
                    if (playerDeckVisual != null) playerDeckVisual.UpdateCount(player.cardsInDeck.Count);
                }
                else
                {
                    if (enemyHandVisual != null) enemyHandVisual.AddCard();
                    if (enemyDeckVisual != null) enemyDeckVisual.UpdateCount(enemy.cardsInDeck.Count);
                }
            }
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

        if (specificRow.HasValue) finalRow = specificRow.Value;
        else if (card.data.range == RangeType.Dowolny) finalRow = (Random.value > 0.5f) ? RangeType.Bliski : RangeType.Daleki;

        if (isPlayerPlayed) zoneName = (finalRow == RangeType.Daleki) ? "PlayerRangeRow" : "PlayerMeleeRow";
        else zoneName = (finalRow == RangeType.Daleki) ? "EnemyRangeRow" : "EnemyMeleeRow";

        GameObject zoneObj = GameObject.Find(zoneName);
        Transform targetParent = null;

        if (zoneObj != null)
        {
            targetParent = zoneObj.transform;
            BoardRow rowScript = zoneObj.GetComponent<BoardRow>();
            if (rowScript != null && rowScript.linkedLayout != null)
            {
                targetParent = rowScript.linkedLayout.transform;
            }
        }
        else
        {
            Debug.LogWarning($"Rz¹d {zoneName} nie ma przypisanego LinkedLayout.");
            return;
        }

        GameObject newCardObj = Instantiate(cardSpritePrefab, targetParent);

        if (insertIndex >= 0 && insertIndex <= targetParent.childCount) 
            newCardObj.transform.SetSiblingIndex(insertIndex);

        SpriteRenderer sr = newCardObj.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            if (card.data.artwork != null) sr.sprite = card.data.artwork;
        }
        else
        {
            Debug.LogWarning($"Karta {card.data.cardName} nie ma przypisanego obrazka.");
            sr.enabled = false;
        }
        
        if (newCardObj.TryGetComponent<CardOnBoard>(out var cardOnBoard)) cardOnBoard.cardInstance = card;

        var visual = newCardObj.GetComponent<CardBoardVisual>();
        if (visual != null) visual.UpdateVisuals(card);

        newCardObj.transform.localPosition = Vector3.zero;
    }

    public void MoveCardToOtherSide(CardInstance card)
    {
        CardBoardVisual[] visuals = FindObjectsByType<CardBoardVisual>(FindObjectsSortMode.None);
        foreach (var v in visuals)
        {
            var cob = v.GetComponent<CardOnBoard>();
            if (cob != null && cob.cardInstance == card)
            {
                Destroy(v.gameObject);
                break;
            }
        }

        bool isNowPlayerSide = playerBoard.Contains(card);

        SpawnCardOnBoardVisual(card, isNowPlayerSide, card.data.range, -1);
    }

    public void StartTargeting(CardInstance source, CardEffect effect)
    {
        currentState = GameState.WaitingForTarget;
        pendingCardSource = source;
        pendingEffect = effect;
        selectedTargets.Clear();

        if (source.owner == player)
        {
            if (effect is ITargetableEffect cardTargetEffect)
            {
                Debug.Log($"-- Tryb Celowania -- Kliknij {cardTargetEffect.GetTargetCount()} cel(e). Typ: {cardTargetEffect.GetTargetAlignment()}.");

                TargetAlignment align = cardTargetEffect.GetTargetAlignment();
                bool isEnemyAction = (align == TargetAlignment.Enemy);

                CardOnBoard[] allCards = FindObjectsByType<CardOnBoard>(FindObjectsSortMode.None);

                foreach (var cardObj in allCards)
                {
                    CardInstance card = cardObj.cardInstance;
                    if (card == null || card == source) continue;

                    bool isMyCard = playerBoard.Contains(card);
                    bool isTargetValid = false;

                    if (align == TargetAlignment.Any) isTargetValid = true;
                    else if (align == TargetAlignment.Friendly && isMyCard) isTargetValid = true;
                    else if (align == TargetAlignment.Enemy && !isMyCard) isTargetValid = true;

                    if (isTargetValid && align == TargetAlignment.Enemy && card.isImunne)
                        isTargetValid = false;

                    if (isTargetValid)
                    {
                        var hl = cardObj.GetComponent<UnitHighlighter>();
                        if (hl != null) hl.ShowTarget(true, isEnemyAction);
                    }
                }
            }
            else if (effect is IRowTargetableEffect rowTargetableEffect)
            {
                Debug.Log($"-- Tryb Celowania -- Kliknij rz¹d.");

                BoardRow[] allRows = FindObjectsByType<BoardRow>(FindObjectsSortMode.None);

                bool isGlobalEffect = (source.data.cardName == "Tolkien");

                foreach (var row in allRows)
                {
                    bool shouldHighlight = false;

                    if (isGlobalEffect) shouldHighlight = true;
                    else if (!row.isPlayerRow) shouldHighlight = true;

                    if (shouldHighlight)
                    {
                        var hl = row.GetComponent<RowHighlighter>();
                        if (hl != null) hl.SetTargetMode(true);
                    }
                }
            }
        }
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

            bool targetIsOnMyBoard = playerBoard.Contains(target);
            bool sourceIsOnMyBoard = playerBoard.Contains(pendingCardSource);
            bool isFriendlySide = (targetIsOnMyBoard == sourceIsOnMyBoard);

            TargetAlignment align = targetEffect.GetTargetAlignment();

            if (align == TargetAlignment.Friendly && !isFriendlySide)
            {
                Debug.LogWarning("Wybra³eœ wrog¹ kartê zamiast sojusznika.");
                return;
            }
            if (align == TargetAlignment.Enemy && isFriendlySide)
            {
                Debug.LogWarning("Wybra³eœ swoj¹ kartê zamiast wrogiej.");
                return;
            }
            if (target.isImunne && align == TargetAlignment.Enemy)
            {
                Debug.LogWarning("Ta karta jest odporna!");
                return;
            }
            if (selectedTargets.Contains(target))
            {
                Debug.Log("Wybra³eœ ju¿ t¹ kartê.");
                return;
            }

            selectedTargets.Add(target);
            Debug.Log($"Wybrano cel: {target.data.cardName}.");

            CardOnBoard[] allVisuals = FindObjectsByType<CardOnBoard>(FindObjectsSortMode.None);
            foreach (var v in allVisuals)
            {
                if (v.cardInstance == target)
                {
                    var hl = v.GetComponent<UnitHighlighter>();
                    if (hl != null) hl.ShowTarget(false, false);
                    break;
                }
            }

            if (selectedTargets.Count >= targetEffect.GetTargetCount())
            {
                targetEffect.ExecuteWithTarget(new List<CardInstance>(selectedTargets));
                EndTargeting();
                selectedTargets.Clear();
            }
        }
        else
        {
            Debug.Log("Klikniêto kartê, ale gra czeka na wybór rzêdu.");
        }
    }

    public void RowClicked(RangeType range, bool isPlayerRow)
    {
        if (currentState != GameState.WaitingForTarget) return;

        if (pendingEffect is IRowTargetableEffect rowEffect)
        {
            Debug.Log($"[GameController] Wybrano rz¹d: {range}.");

            rowEffect.ExecuteWithRowTarget(pendingCardSource, range, isPlayerRow);

            EndTargeting();
        }
        else
        {
            Debug.Log("Klikniêto rz¹d, ale efekt oczekuje czegoœ innego.");
        }
    }

    public void EndTargeting()
    {
        ClearAllHighlights();

        currentState = GameState.Normal;
        pendingCardSource = null;
        pendingEffect = null;
        UpdateUI();

        CheckForAutoPass();

        Debug.Log("[GameController] Zakoñczono celowanie.");
    }

    private void ClearAllHighlights()
    {
        UnitHighlighter[] unitHls = FindObjectsByType<UnitHighlighter>(FindObjectsSortMode.None);
        foreach (var hl in unitHls) hl.ShowTarget(false, false);

        RowHighlighter[] rowHls = FindObjectsByType<RowHighlighter>(FindObjectsSortMode.None);
        foreach (var hl in rowHls) hl.ResetRow();
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

    public bool HasPlayerPassed() => playerHasPassed;

    public CardEffect GetPendingEffect() => pendingEffect;

    public CardInstance GetPendingSource() => pendingCardSource;

    public List<CardInstance> GetPlayerCards(Player player) => player.cardsOnBoard;

    public void ForceWinGame(Player winner)
    {
        Debug.Log($"NATYCHMIASTOWE ZWYCIÊSTWO {winner.playerName}!");
        isGameEnded = true;

        string finalMsg = "";

        if (winner == player)
        {
            playerWins = roundsToWin;
            finalMsg = "EGZAMIN ZDANY!";
        }
        else
        {
            enemyWins = roundsToWin;
            finalMsg = "OBLANY EGZAMIN...";
        }

        if (gameResultText != null) gameResultText.text = finalMsg;
        if (gameResultPanel != null) gameResultPanel.SetActive(true);
        if (gameFinalPointsText != null) gameFinalPointsText.text = $"{playerWins} : {enemyWins}";

        currentState = GameState.WaitingForTarget;
    }
}
