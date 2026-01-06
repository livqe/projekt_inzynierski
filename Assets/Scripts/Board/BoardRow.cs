using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;

public class BoardRow : MonoBehaviour, IPointerClickHandler
{
    [Header("Row Config")]
    public RangeType rowType;
    public bool isPlayerRow;
    public SimpleRowLayout linkedLayout;

    [Header("UI")]
    public TextMeshProUGUI rowScoreText;

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"---Klikniêto rz¹d: {(isPlayerRow ? "Gracz" : "Wróg")} - {rowType}---");

        GameController.Instance.RowClicked(rowType, isPlayerRow);
    }

    public void UpdateRowScore()
    {
        int score = 0;
        Transform container = (linkedLayout != null) ? linkedLayout.transform : this.transform;

        foreach (Transform child in container)
        {
            if (child.name.Contains("Ghost")) continue;

            CardOnBoard cob = child.GetComponent<CardOnBoard>();
            if (cob != null && cob.cardInstance != null) 
                if (GameController.Instance.IsCardAlive(cob.cardInstance))
                    score += cob.cardInstance.currentPower;
        }

        if (rowScoreText != null) rowScoreText.text = score.ToString();
    }
}
