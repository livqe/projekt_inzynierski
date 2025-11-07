using UnityEngine;

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

        //logika karty tutaj
    }
}
