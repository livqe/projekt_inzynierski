using UnityEngine;
using System.Collections.Generic;

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
        Debug.Log($"Aktywacja efektu: {effectName}. {source.data.cardName} ¿¹da wyboru celu.");

        game.StartTargeting(source, this);
    }

    public void ExecuteWithTarget(List<CardInstance> targets)
    {
        if (targets.Count > 0)
        {
            foreach (var target in targets)
            {
                Debug.Log($"[Effect] Zadajê {damageToDeal} obra¿eñ {target.data.cardName}.");
                target.TakeDamage(damageToDeal);
            }
        }
    }
}
