using UnityEngine;

[CreateAssetMenu(menuName = "Card/Effects/Buff/BuffSelfEffect")]
public class BuffSelfEffect : CardEffect, IOnTurnEndEffect
{
    [SerializeField] private int powerToAdd;
    [SerializeField] private int turnFrequency;

    private int turnCounter = 0;

    public void Initialize(int powerAmount, int frequency)
    {
        this.powerToAdd = powerAmount;
        this.turnFrequency = frequency;
        this.turnCounter = 1;
    }

    public override void ActivateEffect(GameController game, CardInstance source)
    {
        Debug.Log($"Aktywacja efektu: {effectName}. {source.data.cardName} zostaje zagrany/a, aktywuje siê za {turnFrequency} tur.");
    }

    public void OnTurnEnd(GameController game, CardInstance source)
    {
        if (source.currentPower <= 0) return;

        if (turnCounter >= turnFrequency)
        {
            Debug.Log($"[Effect] {source.data.cardName} otrzymuje +{powerToAdd} mocy.");
            source.AddPower(powerToAdd);

            turnCounter = 0;
        }
        else
        {
            turnCounter++;
            Debug.Log($"[Effect] {source.data.cardName} czeka na odpowiedni¹ turê. Licznik tur: {turnCounter}/{turnFrequency}.");
        }
    }
}
