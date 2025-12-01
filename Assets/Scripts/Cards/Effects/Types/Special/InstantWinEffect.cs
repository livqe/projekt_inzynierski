using UnityEngine;

[CreateAssetMenu(menuName = "Card/Effects/Special/InstantWinEffect")]
public class InstantWinEffect : CardEffect
{
    public override void ActivateEffect(GameController game, CardInstance source)
    {
        Debug.Log($"[Effect] {source.data.cardName} wchodz¹ na stó³. ZDAJESZ EGZAMIN!");

        game.ForceWinGame(source.owner);
    }
}
