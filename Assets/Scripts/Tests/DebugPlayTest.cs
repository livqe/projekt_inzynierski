using UnityEngine;
using UnityEngine.InputSystem;

public class DebugPlayTest : MonoBehaviour
{
    public CardData testCard;

    [Header("Enemy Card")]
    public CardData testEnemyCard;
    public CardOnBoard enemyTargetVisual;

    private void Start()
    {
        CardInstance enemy = new CardInstance(testEnemyCard, GameController.Instance.enemy);

        enemy.isImunne = true;

        GameController.Instance.enemyBoard.Add(enemy);

        if (enemyTargetVisual != null )
        {
            enemyTargetVisual.cardInstance = enemy;
            Debug.Log($"--- Test: Przeciwnik czeka na planszy (Moc: {enemy.currentPower}) ---");
        }
        else
        {
            Debug.Log("Nie przypisano 'enemyTargetVisual");
        }
    }

    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (testCard == null)
            {
                Debug.Log("Przypisz kartê w inspektorze.");
                return;
            }

            CardInstance newCard = new CardInstance(testCard, GameController.Instance.player);
            GameController.Instance.PlayCard(newCard, true);
        }
    }
}
