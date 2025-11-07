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
        Debug.Log($"Aktywacja efektu: {effectName}. {source.data.cardName} szpieguje przeciwnika, dobierasz {cardsToDraw} kart.");

        //logika karty tutaj
    }
}
