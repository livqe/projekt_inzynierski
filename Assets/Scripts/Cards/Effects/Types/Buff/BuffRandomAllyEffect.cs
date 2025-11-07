using UnityEngine;

[CreateAssetMenu(menuName = "Card/Effects/Buff/BuffRandomAllyEffect")]
public class BuffRandomAllyEffect :CardEffect
{
    [SerializeField] private int powerToAdd;

    public void Initialize(int powerAmount)
    {
        this.powerToAdd = powerAmount;
    }

    public override void ActivateEffect(GameController game, CardInstance source)
    {
        Debug.Log($"Aktywacja efektu: {effectName}. {source.data.cardName} wzmacnia losowego sojusznika o {powerToAdd}.");

        //logika karty tutaj
    }
}
