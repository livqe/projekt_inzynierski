using UnityEngine;
using UnityEngine.InputSystem;

public class DebugPlayTest : MonoBehaviour
{
    public CardData testCard;

    private void Update()
    {
        if (Keyboard.current.gKey.wasPressedThisFrame)
        {
            CardData enemy = ScriptableObject.CreateInstance<CardData>();
            enemy.cardName = "Ork";
            enemy.power = 10;
            CardInstance enemyOrk = new CardInstance(enemy);
            GameController.Instance.enemyBoard.Add(enemyOrk);
            Debug.Log("Dodano wrogiego orka");

            if (testCard == null)
            {
                Debug.Log("Przypisz kartê w inspektorze.");
                return;
            }

            CardInstance newCard = new CardInstance(testCard);
            GameController.Instance.PlayCard(newCard, true);

            if (GameController.Instance.currentState == GameState.WaitingForTarget)
            {
                Debug.Log("Symulacja klikania w orka");
                GameController.Instance.CardClicked(enemyOrk);
            }
        }
    }
}
