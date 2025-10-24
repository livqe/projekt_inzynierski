using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "LaskaEldarow", menuName = "Factions/£aska Eldarów")]
public class LaskaEldarow : FactionAbility
{
    public override void OnRoundEnd(GameController game, Player owner)
    {
        var elves = game.GetPlayerCards(owner)
            .Where(c => c.Faction == Faction.Elfy)
            .ToList();

        if (elves.Count == 0) return;

        var randomElf = elves[Random.Range(0, elves.Count)];
        randomElf.AddPower(1);

        Debug.Log($"{abilityName}: {randomElf.Name} odzyskuje 1 punkt mocy.");
    }
}
