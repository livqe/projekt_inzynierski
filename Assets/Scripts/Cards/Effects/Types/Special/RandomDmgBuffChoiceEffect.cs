using UnityEngine;

[CreateAssetMenu(menuName = "Card/Effects/Special/RandomDmgBuffChoiceEffect")]
public class RandomDmgBuffChoiceEffect : CardEffect
{
    [SerializeField] private int damageToDeal;
    [SerializeField] private int powerToAdd;

    public void Initialize(int damage, int power)
    {
        this.damageToDeal = damage;
        this.powerToAdd = power;
    }

    public override void ActivateEffect(GameController game, CardInstance source)
    {
        Debug.Log($"Aktywacja efektu: {effectName}. {source.data.cardName} losowo wybierze: -{damageToDeal} obra¿eñ lub +{powerToAdd} mocy losowej przyjaznej karcie.");

        //logika karty tutaj
    }
}
