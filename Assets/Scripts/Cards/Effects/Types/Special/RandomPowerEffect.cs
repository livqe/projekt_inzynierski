using UnityEngine;

[CreateAssetMenu(menuName = "Card/Effects/Special/RandomPowerEffect")]
public class RandomPowerEffect : CardEffect, IOnTurnEndEffect
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
        Debug.Log($"Aktywacja efektu: {effectName}. {source.data.cardName} jego moc waha siê miêdzy {minPower} a {maxPower}.");

        source.survivor = true;

        RandomizePower(source);
    }

    public void OnTurnEnd(GameController game, CardInstance source)
    {
        if (source.currentPower < 0) return;

        if (source.currentPower == 0 && !source.survivor) return;

        Debug.Log($"[Effect] Koniec tury, {source.data.cardName} zmienia moc...");
        RandomizePower(source);
    }

    private void RandomizePower(CardInstance source)
    {
        int newPower = Random.Range(minPower, maxPower + 1);
        source.currentPower = newPower;

        GameController.Instance.UpdateUI();

        Debug.Log($"[Effect] {source.data.cardName} ma teraz {newPower} mocy.");
    }
}
