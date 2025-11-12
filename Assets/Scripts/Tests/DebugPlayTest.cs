using UnityEngine;
using UnityEngine.InputSystem;

public class DebugPlayTest : MonoBehaviour
{
    public CardData testCard;
    public CardOnBoard OrkOnScene;

    private void Start()
    {
        CardData enemy = ScriptableObject.CreateInstance<CardData>();
        enemy.cardName = "Ork";
        enemy.power = 10;
        CardInstance enemyOrk = new CardInstance(enemy);

        GameController.Instance.enemyBoard.Add(enemyOrk);
        if (enemyOrk != null )
        {
            OrkOnScene.cardInstance = enemyOrk;
            Debug.Log("--- Test: Ork czeka na przyjêcie ciosu (Moc: 10) ---");
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

            CardInstance newCard = new CardInstance(testCard);
            GameController.Instance.PlayCard(newCard, true);
        }
    }
}
