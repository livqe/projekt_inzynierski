using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "Factions/KuŸnia Ereboru")]
public class DwarvesFactionAbility : FactionAbility
{
    public override void OnRoundEnd(GameController game, Player owner)
    {
        if (!game.PlayerLostLastRound(owner)) return;

        var dwarvesInHand = owner.cardsInHand
            .Where(c  => c.data.faction == Faction.Krasnoludy)
            .ToList();
        
        foreach (var dwarf in dwarvesInHand)
        {
            dwarf.AddPower(1);
        }

        Debug.Log($"{abilityName}: wszystkie krasnoludy w rêce zyska³y +1 si³y.");
    }
}
