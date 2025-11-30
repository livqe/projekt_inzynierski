using UnityEngine;

[CreateAssetMenu(menuName = "Card/Effects/Special/SpyEffect")]
public class SpyEffect : CardEffect
{
    [SerializeField] private int cardsToDraw;

    public void Initialize(int drawCount)
    {
        this.cardsToDraw = drawCount;
    }

    public override void ActivateEffect(GameController game, CardInstance source)
    {
        Debug.Log($"Aktywacja efektu: {effectName}. {source.data.cardName} szpieguje przeciwnika.");

        if (game.playerBoard.Contains(source))
        {
            game.playerBoard.Remove(source);
            game.enemyBoard.Add(source);
        }
        else if (game.enemyBoard.Contains(source))
        {
            game.enemyBoard.Remove(source);
            game.playerBoard.Add(source);
        }

        game.MoveCardToOtherSide(source);

        Debug.Log($"[Effect] Gracz {source.owner.playerName} dobiera {cardsToDraw} kart.");
        game.DrawCard(source.owner, cardsToDraw);
        game.UpdateUI();
    }
}
