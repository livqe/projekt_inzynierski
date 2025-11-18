using UnityEngine;
using UnityEngine.InputSystem;

public class DebugPlayTest : MonoBehaviour
{
    public CardData testCard1;
    public CardData testCard2;

    [Header("Enemy Card")]
    public CardData testEnemyCard;
    public CardOnBoard enemyTargetVisual;

    private void Start()
    {
        CardInstance card1 = new CardInstance(testCard1, GameController.Instance.player);
        card1.currentPower = 1;
        GameController.Instance.playerBoard.Add(card1);

        if (enemyTargetVisual != null )
        {
            enemyTargetVisual.cardInstance = card1;
        }

        CardInstance card2 = new CardInstance(testCard2, GameController.Instance.player);

        if (card2.data.effect == null)
        {
            var effect = ScriptableObject.CreateInstance<BuffSelfOnAllyDeathEffect>();
            effect.Initialize(3, "Ori");
            card2.data.effect = effect;
        }

        GameController.Instance.playerBoard.Add(card2);

        Debug.Log("Dori i Ori na planszy");
        GameController.Instance.UpdateUI();
    }

    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            CardInstance newCard = new CardInstance(testEnemyCard, GameController.Instance.player);
            GameController.Instance.PlayCard(newCard, true);
        }
    }
}
