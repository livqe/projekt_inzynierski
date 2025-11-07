using UnityEngine;

[CreateAssetMenu(menuName = "Card/Effects/Special/ChanceAutoPlayEffect")]
public class ChanceAutoPlayEffect : CardEffect
{
    [SerializeField] private int deckPercent;
    [SerializeField] private int handPercent;

    public void Initialize(int chanceFromDeck, int chanceFromHand)
    {
        this.deckPercent = chanceFromDeck;
        this.handPercent = chanceFromHand;
    }

    public override void ActivateEffect(GameController game, CardInstance source)
    {
        Debug.Log($"Aktywacja efektu: {effectName}. {source.data.cardName} ma {deckPercent}% na zagranie z talii i {handPercent}% z rêki.");

        //logika karty tutaj
    }
}
