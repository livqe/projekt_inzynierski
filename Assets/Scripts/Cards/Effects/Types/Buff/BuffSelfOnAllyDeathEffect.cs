using UnityEngine;

[CreateAssetMenu(menuName = "Card/Effects/Buff/BuffSelfOnAllyDeathEffect")]
public class BuffSelfOnAllyDeathEffect : CardEffect, IOnOtherCardDeathEffect
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
    }

    public void OnOtherCardDeath(GameController game, CardInstance source, CardInstance deadCard)
    {
        if (deadCard.data.cardName == allyName && deadCard.owner == source.owner)
        {
            Debug.Log($"[Effect] {allyName} poleg³, {source.data.cardName} otrzymuje +{powerToAdd} mocy.");
            source.AddPower(powerToAdd);
        }
    }
}
