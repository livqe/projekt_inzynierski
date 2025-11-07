using UnityEngine;

[CreateAssetMenu(menuName = "Card/Effects/Buff/BuffSelfEffect")]
public class BuffSelfEffect : CardEffect
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
        Debug.Log($"Aktywacja efektu: {effectName}. {source.data.cardName} wzmacnia siê o {powerToAdd} co {turnFrequency} tur.");

        //logika karty tutaj
    }
}
