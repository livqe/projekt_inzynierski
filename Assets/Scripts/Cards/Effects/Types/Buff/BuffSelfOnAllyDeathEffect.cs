using UnityEngine;

[CreateAssetMenu(menuName = "Card/Effects/Buff/BuffSelfOnAllyDeathEffect")]
public class BuffSelfOnAllyDeathEffect : CardEffect
{
    [SerializeField] private int powerToAdd;
    [SerializeField] private string allyName;

    public void Initialize(int powerAmount, string ally)
    {
        this.powerToAdd = powerAmount;
        this.allyName = ally;
    }

    public override void ActivateEffect(GameController game, CardInstance source)
    {
        Debug.Log($"Aktywacja efektu: {effectName}. {source.data.cardName} czeka na œmieræ {allyName}, aby otrzymaæ +{powerToAdd}.");

        //logika karty tutaj
    }
}
