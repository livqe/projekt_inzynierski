using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AIController : MonoBehaviour
{
    public static AIController Instance { get; private set; }

    [Header("Settings")]
    public float thinkingTime = 1.5f;

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
            PassTurn();
            yield break;
        }

        if (GameController.Instance.enemy.cardsInHand.Count == 0)
        {
            Debug.Log("[AI] Brak kart. Pasujê.");
            PassTurn();
            yield break;
        }

        PlayCardLogic();
    }

    private void PlayCardLogic()
    {
        Player AIPlayer = GameController.Instance.enemy;

        int randomIndex = Random.Range(0, AIPlayer.cardsInHand.Count);
        CardInstance cardToPlay = AIPlayer.cardsInHand[randomIndex];

        AIPlayer.cardsInHand.RemoveAt(randomIndex);

        Debug.Log($"[AI] Zagrywam {cardToPlay.data.cardName}.");

        RangeType targetRow = cardToPlay.data.range;
        if (targetRow == RangeType.Dowolny)
        {
            targetRow = (Random.value > 0.5f) ? RangeType.Bliski : RangeType.Daleki;
        }

        GameController.Instance.PlayCard(cardToPlay, false, targetRow);

        if (GameController.Instance.currentState == GameState.WaitingForTarget)
            StartCoroutine(PerformAITargeting());
        else
            EndAITurn();
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
            c.currentPower > 0 &&
            !(align == TargetAlignment.Enemy && c.isImunne)
        ).ToList();

        if (potentialTargets.Count > 0)
        {
            CardInstance chosenTarget = potentialTargets[Random.Range(0, potentialTargets.Count)];

            Debug.Log($"[AI] Mój cel: {chosenTarget.data.cardName}.");
            GameController.Instance.CardClicked(chosenTarget);
        }
        else
        {
            Debug.Log("[AI] Brak celów.");
            GameController.Instance.CancelPlay();
        }

        EndAITurn();
    }

    private void HandleRowTargeting(IRowTargetableEffect effect, CardInstance source)
    {
        RangeType randomRow = (Random.value > 0.5f) ? RangeType.Bliski : RangeType.Daleki;

        bool isPlayerRow = true;

        Debug.Log($"[AI] Wybieram rz¹d: {randomRow}.");
        GameController.Instance.RowClicked(randomRow, isPlayerRow);

        EndAITurn();
    }

    private void PassTurn()
    {
        GameController.Instance.EnemyPassRound();
    }

    private void EndAITurn()
    {
        GameController.Instance.EndEnemyTurn();
    }

    private int GetBoardPoints(List<CardInstance> board)
    {
        int sum = 0;
        foreach (var c in board) sum += c.currentPower;
        return sum;
    }
}
