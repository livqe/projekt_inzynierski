using UnityEngine;

[CreateAssetMenu(menuName = "Card/Effects/Buff/BuffTargetEffect")]
public class BuffTargetEffect : CardEffect
{
    [SerializeField] private int powerToAdd;
    [SerializeField] private int targetCount;

    public void Initialize(int powerAmount, int count)
    {
        this.powerToAdd = powerAmount;
        this.targetCount = count;
    }

    public override void ActivateEffect(GameController game, CardInstance source)
    {
        Debug.Log($"Aktywacja efektu: {effectName}. {source.data.cardName} chce wzmocniæ {targetCount} sojusznika/ów o +{powerToAdd}.");

        //logika karty tutaj
    }
}
