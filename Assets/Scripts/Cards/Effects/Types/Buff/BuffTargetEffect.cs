using UnityEngine;

[CreateAssetMenu(menuName = "Card/Effects/Buff/BuffTargetEffect")]
public class BuffTargetEffect : CardEffect, ITargetableEffect
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
        Debug.Log($"Aktywacja efektu: {effectName}. {source.data.cardName} ¿¹da wyboru {targetCount} sojusznika/ów, by ich wzmocniæ o +{powerToAdd}.");

        game.StartTargeting(source, this);
    }

    public void ExecuteWithTarget(CardInstance target)
    {
        Debug.Log($"[Effect] Wzmacniam {target.data.cardName} o {powerToAdd}.");
        target.AddPower(powerToAdd);
    }
}
