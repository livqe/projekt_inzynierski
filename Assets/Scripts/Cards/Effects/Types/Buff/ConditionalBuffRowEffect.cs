using UnityEngine;

[CreateAssetMenu(menuName = "Card/Effects/Buff/ConditionalBuffRowEffect")]
public class ConditionalBuffRowEffect : CardEffect
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
        Debug.Log($"Aktywacja efektu: {effectName}. {source.data.cardName} wzmocni rz¹d o +{powerToAdd}, jeœli na planszy jest {allyName}.");

        //logika karty tutaj
    }
}
