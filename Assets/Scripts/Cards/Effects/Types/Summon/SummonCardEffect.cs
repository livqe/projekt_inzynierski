using UnityEngine;

[CreateAssetMenu(menuName = "Card/Effects/Summon/SummonCardEffect")]
public class SummonCardEffect : CardEffect
{
    [SerializeField] private string cardToSummon;
    [SerializeField] private string amountToSummon;

    public void Initialize(string cardName, string amount)
    {
        this.cardToSummon = cardName;
        this.amountToSummon = amount;
    }

    public override void ActivateEffect(GameController game, CardInstance source)
    {
        Debug.Log($"Aktywacja efektu: {effectName}. {source.data.cardName} przyzywa {amountToSummon} kopii karty {cardToSummon}.");

        //logika karty tutaj
    }
}
