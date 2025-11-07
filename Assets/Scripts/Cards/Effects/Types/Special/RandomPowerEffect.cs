using UnityEngine;

[CreateAssetMenu(menuName = "Card/Effects/Special/RandomPowerEffect")]
public class RandomPowerEffect : CardEffect
{
    [SerializeField] private int minPower;
    [SerializeField] private int maxPower;

    public void Initialize(int min, int max)
    {
        this.minPower = min;
        this.maxPower = max;
    }

    public override void ActivateEffect(GameController game, CardInstance source)
    {
        Debug.Log($"Aktywacja efektu: {effectName}. {source.data.cardName} wybiera losowo swoj¹ moc miêdzy {minPower} a {maxPower}.");

        //logika karty tutaj
    }
}
