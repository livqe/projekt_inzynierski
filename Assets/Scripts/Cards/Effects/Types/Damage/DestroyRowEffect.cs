using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(menuName = "Card/Effects/Damage/DestroyRowEffect")]
public class DestroyRowEffect : CardEffect, IRowTargetableEffect
{
    [Header("Effects")]
    public GameObject fireParticlePrefab;
    public AudioClip fireAudioSource;

    public override void ActivateEffect(GameController game, CardInstance source)
    {
        Debug.Log($"Aktywacja efektu: {effectName}. {source.data.cardName} czeka na wybranie rzêdu.");
        game.StartTargeting(source, this);
    }

    public void ExecuteWithRowTarget(CardInstance source, RangeType range, bool isPlayerRow)
    {
        GameController game = GameController.Instance;

        if ((isPlayerRow && source.owner == game.player) || (!isPlayerRow && source.owner == game.enemy))
        {
            Debug.LogWarning("Nie mo¿esz spaliæ w³asnego rzêdu.");
            return;
        }

        BoardRow targetRowObject = GetRowObject(range,  isPlayerRow);

        if (targetRowObject == null)
        {
            Debug.LogWarning($"Nie znaleziono rzêdu {(isPlayerRow ? "Gracz" : " Wróg")} {range}");
            return;
        }

        if (fireParticlePrefab != null)
        {
            GameObject vfx = Instantiate(fireParticlePrefab, targetRowObject.transform.position, Quaternion.identity);
            Destroy(vfx, 2.0f);
        }

        if (GameController.Instance.audioSource != null && fireAudioSource != null)
            GameController.Instance.audioSource.PlayOneShot(fireAudioSource);

        CardOnBoard[] cardVisuals = targetRowObject.GetComponentsInChildren<CardOnBoard>();
        List<CardInstance> cardsToDestroy = new List<CardInstance>();

        foreach (var visual in cardVisuals)
        {
            if (visual.cardInstance != null)
            {
                cardsToDestroy.Add(visual.cardInstance);
            }
        }

        if (cardsToDestroy.Count > 0)
        {
            foreach (var card in cardsToDestroy)
            {
                if (card.isImunne)
                {
                    Debug.Log($"Karta {card.data.cardName} jest odporna na obra¿enia.");
                    continue;
                }

                Debug.Log($"Niszczenie karty: {card.data.cardName}");
                game.OnCardDeath(card);
            }
        }
        else
        {
            Debug.Log("Wybrany rz¹d by³ pusty lub karty s¹ odporne.");
        }

        game.EndTargeting();
    }

    private BoardRow GetRowObject(RangeType range, bool isPlayerRow)
    {
        BoardRow[] allRows = FindObjectsByType<BoardRow>(FindObjectsSortMode.None);

        foreach (var row in allRows)
        {
            if (row.rowType == range && row.isPlayerRow == isPlayerRow)
            {
                return row;
            }
        }
        return null;
    }
}
