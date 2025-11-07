using UnityEngine;

[CreateAssetMenu(menuName = "Card/Effects/Buff/BuffRowEffect")]
public class BuffRowEffect : CardEffect
{
    [SerializeField] private int powerToAdd;

    public void Initialize(int powerAmount)
    {
        this.powerToAdd = powerAmount;
    }

    public override void ActivateEffect(GameController game, CardInstance source)
    {
        Debug.Log($"Aktywacja efektu: {effectName}. {source.data.cardName} wzmacnia swój rz¹d o {powerToAdd}.");

        //logika karty tutaj
    }
}
