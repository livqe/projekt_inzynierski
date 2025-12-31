using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Card/Effects/Damage/RowDamageEffect")]
public class RowDamageEffect : CardEffect, IRowTargetableEffect
{
    [SerializeField] private int damageToDeal;

    [Header("Visual Effects")]
    public GameObject vfxPrefab;

    public void Initialize(int damageAmount)
    {
        this.damageToDeal = damageAmount;
    }

    public override void ActivateEffect(GameController game, CardInstance source)
    {
        Debug.Log($"Aktywacja efektu: {effectName}. {source.data.cardName} czeka na wybranie rzêdu.");
        game.StartTargeting(source, this);
    }

    public void ExecuteWithRowTarget(CardInstance source, RangeType range, bool isPlayerRow)
    {
        GameController game = GameController.Instance;

        if (source.data.cardName != "Tolkien")
        {
            if (isPlayerRow && source.owner == game.player)
            {
                Debug.LogWarning("Nie atakuj swojego rzêdu.");
                return;
            }

            if (!isPlayerRow && source.owner == game.enemy)
            {
                return;
            }
        }

        Debug.Log($"[Effect] Atakowany rz¹d: {range}.");
        
        List<CardInstance> targets = new List<CardInstance>();

        if (source.data.cardName == "Tolkien")
        {
            BoardRow playerRow = FindRowOnScene(range, true);
            BoardRow enemyRow = FindRowOnScene(range, false);

            if (playerRow != null) targets.AddRange(GetCardsFromRow(playerRow));
            if (enemyRow != null) targets.AddRange(GetCardsFromRow(enemyRow));
        }
        else
        {
            BoardRow targetRow = FindRowOnScene(range, isPlayerRow);
            
            if (targetRow != null)
            {
                if (vfxPrefab != null) Instantiate(vfxPrefab, targetRow.transform.position, Quaternion.identity);

                targets.AddRange(GetCardsFromRow(targetRow));
            }
        }

        if (targets.Count > 0)
        {
            foreach (var card in targets)
            {
                if (card.currentPower >= 0 && !card.isImunne)
                {
                    card.TakeDamage(damageToDeal);
                }
            }
        }
        else
        {
            Debug.Log("[Effect] Rz¹d jest pusty.");
        }

        GameController.Instance.EndTargeting();
    }

    private BoardRow FindRowOnScene(RangeType range, bool isPlayerRow)
    {
        BoardRow[] allRows = FindObjectsByType<BoardRow>(FindObjectsSortMode.None);
        
        foreach (var row in allRows)
        {
            if (row.rowType == range && row.isPlayerRow == isPlayerRow) return row;
        }

        return null;
    }

    private List<CardInstance> GetCardsFromRow(BoardRow rowObject)
    {
        List<CardInstance> cards = new List<CardInstance>();
        CardOnBoard[] visualCards = rowObject.GetComponentsInChildren<CardOnBoard>();

        foreach (var visual in visualCards)
        {
            if (visual.cardInstance != null) cards.Add(visual.cardInstance);
        }
        return cards;
    }
}
