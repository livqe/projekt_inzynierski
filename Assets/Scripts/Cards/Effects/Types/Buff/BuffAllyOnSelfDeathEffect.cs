using UnityEngine;

[CreateAssetMenu(menuName = "Card/Effects/Buff/BuffAllyOnSelfDeathEffect")]
public class BuffAllyOnSelfDeathEffect : CardEffect
{
    [SerializeField] private int powerToAdd;
    [SerializeField] private string allyToBuff;

    public void Initialize(int powerAmount, string ally)
    {
        this.powerToAdd = powerAmount;
        this.allyToBuff = ally;
    }

    public override void ActivateEffect(GameController game, CardInstance source)
    {
        Debug.Log($"Aktywacja efektu: {effectName}. Gdy {source.data.cardName} zginie, {allyToBuff} otrzyma {powerToAdd} mocy.");

        //logika karty tutaj
    }
}
