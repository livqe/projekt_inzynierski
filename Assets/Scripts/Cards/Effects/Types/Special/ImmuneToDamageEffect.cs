using UnityEngine;

[CreateAssetMenu(menuName = "Card/Effects/Special/ImmuneToDamageEffect")]
public class ImmuneToDamageEffect : CardEffect
{
    public override void ActivateEffect(GameController game, CardInstance source)
    {
        Debug.Log($"Aktywacja efektu: {effectName}. {source.data.cardName} jest odporny na obra¿enia.");

        //logika karty tutaj
    }
}
