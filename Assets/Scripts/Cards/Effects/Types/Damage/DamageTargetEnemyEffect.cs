using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(menuName = "Card/Effects/Damage/DamageTargetEnemyEffect")]
public class DamageTargetEnemyEffect : CardEffect, ITargetableEffect
{
    [SerializeField] private int damageToDeal;
    [SerializeField] private int targetCount = 1;

    public void Initialize(int damageAmount, int count = 1)
    {
        this.damageToDeal = damageAmount;
        this.targetCount = count;
    }

    public int GetTargetCount() { return targetCount; }
    public TargetAlignment GetTargetAlignment() { return TargetAlignment.Enemy; }

    public override void ActivateEffect(GameController game, CardInstance source)
    {
        List<CardInstance> enemyBoard = (source.owner == game.player) ? game.enemyBoard : game.playerBoard;

        bool anyValidTarget = enemyBoard.Any(c => c.currentPower >= 0 && !c.isImunne);

        if (anyValidTarget)
        {
            Debug.Log($"Aktywacja efektu: {effectName}. {source.data.cardName} ¿¹da wyboru celu.");
            game.StartTargeting(source, this);
        }
        else
            Debug.Log($"[Effect] Brak wa¿nych celów dla {source.data.cardName}.");
    }

    public void ExecuteWithTarget(List<CardInstance> targets)
    {
        foreach (var target in targets)
        {
            Debug.Log($"[Effect] Zadajê {damageToDeal} obra¿eñ {target.data.cardName}.");
            target.TakeDamage(damageToDeal);
        }
    }
}
