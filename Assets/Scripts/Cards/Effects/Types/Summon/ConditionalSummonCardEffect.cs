using UnityEngine;

[CreateAssetMenu(menuName = "Card/Effects/Summon/ConditionalSummonCardEffect")]
public class ConditionalSummonCardEffect : CardEffect
{
    [SerializeField] private string cardToSummon;
    [SerializeField] private string requiredAlly;

    public void Initialize(string cardName, string allyName)
    {
        this.cardToSummon = cardName;
        this.requiredAlly = allyName;
    }

    public override void ActivateEffect(GameController game, CardInstance source)
    {
        Debug.Log($"Aktywacja efektu: {effectName}. {source.data.cardName} przyzwie {cardToSummon}, jeœli {requiredAlly} jest na planszy.");

        //logika karty tutaj
    }
}
