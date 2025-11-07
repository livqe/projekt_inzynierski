using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(menuName = "Card/Effects/Buff/ConditionalBuffSelfEffect")]
public class ConditionalBuffSelfEffect : CardEffect
{
    [SerializeField] private int powerToAdd;
    [SerializeField] private List<string> alliesNames = new List<string>();

    public void Initialize(int powerAmount, string[] allies)
    {
        this.powerToAdd = powerAmount;
        this.alliesNames = allies.ToList();
    }

    public override void ActivateEffect(GameController game, CardInstance source)
    {
        string alliesList = string.Join(", ", alliesNames);
        Debug.Log($"Aktywacja efektu: {effectName}. {source.data.cardName} otrzyma +{powerToAdd}, jeœli na planszy jest/s¹: {alliesList}.");

        //logika karty tutaj
    }
}
