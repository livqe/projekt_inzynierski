using UnityEngine;

[CreateAssetMenu(menuName = "Card/Effects/Special/DeathLinkEffect")]
public class DeathLinkEffect : CardEffect, IOnOtherCardDeathEffect
{
    [SerializeField] private string linkedCard;

    public void Initialize(string card)
    {
        this.linkedCard = card;
    }

    public override void ActivateEffect(GameController game, CardInstance source)
    {
        Debug.Log($"Aktywacja efektu: {effectName}. {source.data.cardName} zginie, jeœli zginie {linkedCard}.");
    }

    public void OnOtherCardDeath(GameController game, CardInstance source, CardInstance deadCard)
    {
        if (deadCard.data.cardName == linkedCard && deadCard.owner == source.owner)
        {
            Debug.Log($"[Effect] {linkedCard} zgin¹³, {source.data.cardName} ginie.");
            source.TakeDamage(source.currentPower + source.shield);
        }
    }
}
