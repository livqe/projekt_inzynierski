using UnityEngine;

[CreateAssetMenu(menuName = "Card/Effects/Buff/AddShieldEffect")]
public class AddShieldEffect : CardEffect
{
    [SerializeField] private int shieldToAdd;

    public void Initialize(int shieldAmount)
    {
        this.shieldToAdd = shieldAmount;
    }

    public override void ActivateEffect(GameController game, CardInstance source)
    {
        Debug.Log($"Aktywacja efektu: {effectName}. Karta {source.data.cardName} otrzymuje {shieldToAdd} tarczy.");

        //logika karty tutaj
    }
}
