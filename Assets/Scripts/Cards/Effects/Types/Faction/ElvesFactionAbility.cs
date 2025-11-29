using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Factions/£aska Eldarów")]
public class ElvesFactionAbility : FactionAbility
{
    public override void OnTurnEnd(GameController game, Player owner)
    {
        List<CardInstance> board = (owner == game.player) ? game.playerBoard : game.enemyBoard;

        var injuredElves = board
            .Where(c => c.data.faction == Faction.Elfy && c.currentPower < c.data.power)
            .ToList();

        if (injuredElves.Count == 0) return;

        var luckyElf = injuredElves[Random.Range(0, injuredElves.Count)];

        Debug.Log($"[£aska Eldarów] {luckyElf.data.cardName} zostaje uleczony o +1.");
        luckyElf.AddPower(1);
    }
}
