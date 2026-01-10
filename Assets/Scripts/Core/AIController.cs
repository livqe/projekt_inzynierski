using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AIController : MonoBehaviour
{
    public static AIController Instance { get; private set; }

    [Header("Settings")]
    public float thinkingTime = 1.5f;

    private int actionRetries = 0;
    private const int MAX_RETRIES = 3;

    private void Awake()
    {
        Instance = this;
    }

    public void MakeDecision()
    {
        StartCoroutine(ThinkAndAct());
    }

    private IEnumerator ThinkAndAct()
    {
        Debug.Log("[AI] Myœlê...");
        yield return new WaitForSeconds(thinkingTime);

        int myPoints = GetBoardPoints(GameController.Instance.enemyBoard);
        int playerPoints = GetBoardPoints(GameController.Instance.playerBoard);
        bool playerPassed = GameController.Instance.HasPlayerPassed();

        if (myPoints > playerPoints && playerPassed)
        {
            Debug.Log("[AI] Pasujê.");
            ResetRetries();
            PassTurn();
            yield break;
        }

        if (GameController.Instance.enemy.cardsInHand.Count == 0)
        {
            Debug.Log("[AI] Brak kart. Pasujê.");
            ResetRetries();
            PassTurn();
            yield break;
        }

        if (actionRetries >= MAX_RETRIES)
        {
            Debug.Log("[AI] Osi¹gniêto limit prób. Pasujê.");
            ResetRetries();
            PassTurn();
            yield break;
        }

        yield return StartCoroutine(PlayCardLogic());
    }

    private IEnumerator PlayCardLogic()
    {
        Player AIPlayer = GameController.Instance.enemy;

        int randomIndex = Random.Range(0, AIPlayer.cardsInHand.Count);
        CardInstance cardToPlay = AIPlayer.cardsInHand[randomIndex];

        Debug.Log($"[AI] Zagrywam {cardToPlay.data.cardName}.");

        bool forcePlayWithoutEffect = (actionRetries >= 2);

        if (forcePlayWithoutEffect)
            Debug.Log("[AI] Zbyt wiele prób celowania. Zagrywam kartê bez efektu.");

        actionRetries++;

        RangeType targetRow = cardToPlay.data.range;
        if (targetRow == RangeType.Dowolny)
        {
            targetRow = (Random.value > 0.5f) ? RangeType.Bliski : RangeType.Daleki;
        }

        GameController.Instance.PlayCard(cardToPlay, false, true, targetRow, -1, forcePlayWithoutEffect);

        yield return new WaitForSeconds(0.7f);

        if (GameController.Instance.currentState == GameState.WaitingForTarget)
            StartCoroutine(PerformAITargeting());
        else
        {
            ResetRetries();
        }
    }

    private IEnumerator PerformAITargeting()
    {
        Debug.Log("[AI] Wybieram cel...");
        yield return new WaitForSeconds(1.0f);

        var effect = GameController.Instance.GetPendingEffect();
        var source = GameController.Instance.GetPendingSource();

        if (effect is ITargetableEffect targetEffect)
            HandleCardTargeting(targetEffect, source);
        else if (effect is IRowTargetableEffect rowEffect)
            HandleRowTargeting(rowEffect, source);
    }

    private void HandleCardTargeting(ITargetableEffect effect, CardInstance source)
    {
        TargetAlignment align = effect.GetTargetAlignment();
        List<CardInstance> potentialTargets = new List<CardInstance>();

        if (align == TargetAlignment.Enemy || align == TargetAlignment.Any)
        {
            potentialTargets.AddRange(GameController.Instance.playerBoard);
        }
        if (align == TargetAlignment.Friendly || align == TargetAlignment.Any)
        {
            potentialTargets.AddRange(GameController.Instance.enemyBoard);
        }

        potentialTargets = potentialTargets.Where(c => 
            c != source && 
            c.currentPower >= 0 &&
            !(align == TargetAlignment.Enemy && c.isImunne)
        ).ToList();

        int requiredCount = effect.GetTargetCount();
        int availableCount = potentialTargets.Count;
        int targetsToPick = Mathf.Min(requiredCount, availableCount);

        if (targetsToPick > 0)
        {
            for (int i = 0; i < targetsToPick; i++)
            {
                if (potentialTargets.Count == 0) break;

                int randIdx = Random.Range(0, potentialTargets.Count);
                CardInstance chosenTarget = potentialTargets[randIdx];

                Debug.Log($"[AI] Klikam cel {i + 1}/{targetsToPick}: {chosenTarget.data.cardName}.");

                ResetRetries();

                GameController.Instance.CardClicked(chosenTarget);

                potentialTargets.RemoveAt(randIdx);
            }
        }
        else
        {
            Debug.Log("[AI] Brak celów.");
            GameController.Instance.CancelPlay();
        }
    }

    private void HandleRowTargeting(IRowTargetableEffect effect, CardInstance source)
    {
        RangeType randomRow = (Random.value > 0.5f) ? RangeType.Bliski : RangeType.Daleki;

        bool isPlayerRow = true;

        Debug.Log($"[AI] Wybieram rz¹d: {randomRow}.");

        ResetRetries();

        GameController.Instance.RowClicked(randomRow, isPlayerRow);
    }

    private void PassTurn()
    {
        ResetRetries();
        GameController.Instance.EnemyPassRound();
    }

    public void ResetRetries()
    {
        actionRetries = 0;
    }

    private int GetBoardPoints(List<CardInstance> board)
    {
        int sum = 0;
        foreach (var c in board) sum += c.currentPower;
        return sum;
    }
}
