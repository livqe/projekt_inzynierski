using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Card/Effects/Special/MutualDestructionEffect")]
public class MutualDestructionEffect : CardEffect
{
    [SerializeField] private string linkedEnemy;

    public void Initialize(string enemy)
    {
        this.linkedEnemy = enemy;
    }

    public override void ActivateEffect(GameController game, CardInstance source)
    {
        Debug.Log($"Aktywacja efektu: {effectName}. {source.data.cardName} i {linkedEnemy} gin¹, jeœli oboje znajduj¹ siê na planszy.");

        List<CardInstance> cardBoard = (source.owner == game.player) ? game.enemyBoard : game.playerBoard;
        CardInstance rival = null;

        foreach (CardInstance card in cardBoard)
        {
            if (card.data.cardName == linkedEnemy && card.currentPower > 0)
            {
                rival = card;
                break;
            }
        }

        if (rival != null)
        {
            Debug.Log($"[Effect] {source.data.cardName} i {linkedEnemy} s¹ razem na planszy, oboje umieraj¹.");

            rival.TakeDamage(rival.currentPower + rival.shield);
            source.TakeDamage(source.currentPower + source.shield);
        }
        else
        {
            Debug.Log($"[Effect] {linkedEnemy} nieobecny, {source.data.cardName} gra normalnie.");
        }
    }
}
