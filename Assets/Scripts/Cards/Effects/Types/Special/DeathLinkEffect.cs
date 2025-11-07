using UnityEngine;

[CreateAssetMenu(menuName = "Card/Effects/Special/DeathLinkEffect")]
public class DeathLinkEffect : CardEffect
{
    [SerializeField] private string linkedCard;

    public void Initialize(string card)
    {
        this.linkedCard = card;
    }

    public override void ActivateEffect(GameController game, CardInstance source)
    {
        Debug.Log($"Aktywacja efektu: {effectName}. {source.data.cardName} zginie, jeœli zginie {linkedCard}.");

        //logika karty tutaj
    }
}
