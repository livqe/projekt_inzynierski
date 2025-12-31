using UnityEngine;
using UnityEngine.UI;

public class RoundGemVisual : MonoBehaviour
{
    [Header("Icons")]
    public Image gem1;
    public Image gem2;

    [Header("Sprites")]
    public Sprite activeSprite;
    public Sprite lostSprite;

    public void UpdateLives(int roundsWonByEnemy)
    {
        if (gem1 != null) gem1.sprite = (roundsWonByEnemy >= 1) ? lostSprite : activeSprite;
        if (gem2 != null) gem2.sprite = (roundsWonByEnemy >= 2) ? lostSprite : activeSprite;
    }
}
