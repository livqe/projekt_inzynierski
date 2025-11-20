using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEditor.Rendering;

public class DebugPlayTest : MonoBehaviour
{
    [Header("-- Test Panel --")]
    [Tooltip("Tested Card")]
    public CardData testCard;

    [Header("-- Training Target --")]
    [Tooltip("Enemy Card")]
    public CardData testEnemyCard;
    public CardOnBoard enemyTargetVisual;

    private void Start()
    {
        Debug.Log("Spacja - zagraj kartê");
        Debug.Log("E - restart sto³u");
        Debug.Log("D - dobierz kartê");
        Debug.Log("R - dodaj po 2 kuk³y w ka¿dym rzêdzie");

        SpawnTarget(GameController.Instance.enemy, RangeType.Bliski, testEnemyCard.cardName);
    }

    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            CardInstance newCard = new CardInstance(testCard, GameController.Instance.player);
            GameController.Instance.PlayCard(newCard, true);
        }

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            ResetBoard();
            SpawnTarget(GameController.Instance.enemy, RangeType.Bliski, testEnemyCard.cardName);
        }

        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            SpawnTarget(GameController.Instance.enemy, RangeType.Bliski, testEnemyCard.cardName);
        }

        if (Keyboard.current.dKey.wasPressedThisFrame)
        {
            GameController.Instance.DrawCard(GameController.Instance.player, 1);
        }

        if (Keyboard.current.rKey.wasPressedThisFrame)
        {

            PopulateBoardWithTargets();
        }
    }

    private void SpawnTarget(Player owner, RangeType range, string name = "Test")
    {
        CardInstance targetInstance = new CardInstance(testEnemyCard, GameController.Instance.enemy);

        if (name == "Test")
        {
            CardData target = ScriptableObject.CreateInstance<CardData>();
            target.cardName = name;
            target.power = 10;
            target.range = range;

            targetInstance = new CardInstance(target, owner);
        }
        
        if (owner == GameController.Instance.player)
        {
            GameController.Instance.playerBoard.Add(targetInstance);
        }
        else
        {
            GameController.Instance.enemyBoard.Add(targetInstance);
        }

        if (enemyTargetVisual != null)
        {
            enemyTargetVisual.cardInstance = targetInstance;
            enemyTargetVisual.GetComponent<SpriteRenderer>().color = Color.white;
        }

        if (targetInstance.data.effect != null)
        {
            targetInstance.data.effect.ActivateEffect(GameController.Instance, targetInstance);
        }

        GameController.Instance.UpdateUI();
    }

    private void PopulateBoardWithTargets()
    {
        Debug.Log("-- Generowanie celów w ka¿dym rzêdzie --");

        SpawnTarget(GameController.Instance.player, RangeType.Bliski);
        SpawnTarget(GameController.Instance.player, RangeType.Bliski);
        SpawnTarget(GameController.Instance.player, RangeType.Daleki);
        SpawnTarget(GameController.Instance.player, RangeType.Daleki);

        SpawnTarget(GameController.Instance.enemy, RangeType.Bliski);
        SpawnTarget(GameController.Instance.enemy, RangeType.Bliski);
        SpawnTarget(GameController.Instance.enemy, RangeType.Daleki);
        SpawnTarget(GameController.Instance.enemy, RangeType.Daleki);

        GameController.Instance.UpdateUI();
    }

    private void ResetBoard()
    {
        GameController.Instance.playerBoard.Clear();
        GameController.Instance.enemyBoard.Clear();
        GameController.Instance.UpdateUI();
        Debug.Log("-- Wyczyszczono stó³ --");
    }
}
