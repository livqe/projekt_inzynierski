using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "KuzniaEreboru", menuName = "Factions/KuŸnia Ereboru")]
public class KuzniaEreboru : FactionAbility
{
    public override void OnRoundEnd(GameController game, Player owner)
    {
        if (!game.PlayerLostLastRound(owner)) return;

        var dwarves = game.GetPlayerCards(owner)
            .Where(c  => c.Faction == Faction.Krasnoludy)
            .ToList();
        
        foreach (var d in dwarves)
        {
            d.AddPower(1);
        }

        Debug.Log($"{abilityName}: wszystkie krasnoludy zyska³y +1 si³y.");
    }
}
