using UnityEngine;

[CreateAssetMenu(menuName = "Card/Effects/Buff/BuffSelfEffect")]
public class BuffSelfEffect : CardEffect, IOnTurnEndEffect
{
    [SerializeField] private int powerToAdd;
    [SerializeField] private int turnFrequency;

    public void Initialize(int powerAmount, int frequency)
    {
        this.powerToAdd = powerAmount;
        this.turnFrequency = frequency;
    }

    public override void ActivateEffect(GameController game, CardInstance source)
    {
        Debug.Log($"Aktywacja efektu: {effectName}. {source.data.cardName} zostaje zagrany/a, aktywuje siê za {turnFrequency} tur.");
        source.effectTurnCounter = 1;
    }

    public void OnTurnEnd(GameController game, CardInstance source)
    {
        if (source.currentPower <= 0) return;

        if (source.effectTurnCounter >= turnFrequency)
        {
            Debug.Log($"[Effect] {source.data.cardName} otrzymuje +{powerToAdd} mocy.");
            source.AddPower(powerToAdd);

            source.effectTurnCounter = 0;
        }
        else
        {
            source.effectTurnCounter++;
            Debug.Log($"[Effect] {source.data.cardName} czeka na odpowiedni¹ turê. Licznik tur: {source.effectTurnCounter}/{turnFrequency}.");
        }
    }
}
