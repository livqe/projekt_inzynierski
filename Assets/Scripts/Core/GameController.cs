using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public enum GameState
{
    Normal,
    WaitingForTarget
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
    private ITargetableEffect pendingEffect;
    private List<CardInstance> selectedTargets = new List<CardInstance>();

    private bool playerHasPassed = false, enemyHasPassed = false;

    void Start()
    {
        //tu trzeba podmieniæ na wybór frakcji w menu
        player = new Player("Gracz", Faction.Elfy);
        enemy = new Player("AI", Faction.Krasnoludy);

        StartPlayerTurn();
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

    public void StartPlayerTurn()
    {
        Debug.Log("Pocz¹tek tury gracza");
        playerHasPassed = false;

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

        NotifyOtherCardsOnPlay(card);

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
            {
                reactionEffect.OnOtherCardPlayed(this, card, newCard);
            }
        }
    }

    public void DrawCard(Player playre, int amount = 1)
    {
        for (int i = 0; i < amount; i++)
        {
            if (player.cardsInDeck.Count == 0)
            {
                Debug.Log($"[GameController] {player.playerName} nie ma ju¿ kart w talii.");
                return;
            }

            CardInstance drawnCard = player.cardsInDeck[0];
            player.cardsInDeck.RemoveAt(0);

            player.cardsInHand.Add(drawnCard);
            Debug.Log($"[GameController] {player.playerName} dobiera kartê: {drawnCard.data.cardName}");

            //odœwie¿enie widoku rêki w UI tutaj
        }
    }

    public void StartTargeting(CardInstance source, ITargetableEffect effect)
    {
        currentState = GameState.WaitingForTarget;
        pendingCardSource = source;
        pendingEffect = effect;
        selectedTargets.Clear();

        Debug.Log($"-- Tryb Celowania -- Kliknij {effect.GetTargetCount()} cel(e). Typ: {effect.GetTargetAlignment()}.");

        //tutaj jakieœ opcje UI (np. podœwietlanie celów)
    }

    public void CardClicked(CardInstance target)
    {
        if (currentState != GameState.WaitingForTarget) return;

        if (target == pendingCardSource)
        {
            Debug.LogWarning("Nie celuj w kartê u¿ywaj¹c¹ efektu.");
            return;
        }

        Debug.Log($"Wybrano cel: {target.data.cardName}.");

        TargetAlignment align = pendingEffect.GetTargetAlignment();
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

        if (selectedTargets.Count >= pendingEffect.GetTargetCount())
        {
            currentState = GameState.Normal;

            pendingEffect.ExecuteWithTarget(new List<CardInstance>(selectedTargets));
            
            pendingCardSource = null;
            pendingEffect = null;
            selectedTargets.Clear();
            UpdateUI();
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

    public void UpdateUI()
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
        foreach (var card in enemyBoard)
        {
            Debug.Log($"[Stó³ Przeciwnika] {card.data.cardName} (moc: {card.currentPower}, tarcza: {card.shield})");
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

        StartPlayerTurn();

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

    public bool PlayerLostLastRound(Player player) => player.lostLastRound;

    public List<CardInstance> GetPlayerCards(Player player) => player.cardsOnBoard;

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

        UpdateUI();
    }
}
