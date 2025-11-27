using UnityEngine;
using System.Collections.Generic;

public class SimpleRowLayout : MonoBehaviour
{
    [Header("Settings")]
    public float spacing = 1.5f;
    public GameObject ghostPrefab;

    private GameObject currentGhost;

    public void UpdateGhostPosition(float mouseX)
    {
        if (currentGhost == null)
        {
            if (ghostPrefab != null)
            {
                currentGhost = Instantiate(ghostPrefab, transform);
            }
            else
            {
                currentGhost = new GameObject("GhostSpacer");
                currentGhost.transform.SetParent(transform);
            }
        }

        int bestIndex = CalculateIndexForX(mouseX);
        currentGhost.transform.SetSiblingIndex(bestIndex);

        UpdateLayout();
    }

    public void RemoveGhost()
    {
        if (currentGhost != null)
        {
            Destroy(currentGhost);
            currentGhost = null;
            UpdateLayout();
        }
    }

    public int GetGhostIndex()
    {
        if (currentGhost != null) return currentGhost.transform.GetSiblingIndex();
        return transform.childCount;
    }

    private int CalculateIndexForX(float x)
    {
        int childCount = transform.childCount;
        for (int i = 0;  i < childCount; i++)
        {
            Transform child = transform.GetChild(i);

            if (x < child.position.x)
            {
                return i;
            }
        }

        return childCount;
    }

    [ContextMenu("Arrange the cards")]
    public void UpdateLayout()
    {
        List<Transform> cards = new List<Transform>();
        foreach (Transform child in transform)
        {
            if (child.gameObject.activeSelf) cards.Add(child);
        }

        int count = cards.Count;
        if (count == 0) return;

        float totalWidth = (count - 1) * spacing;
        float startX = -totalWidth / 2f;

        for (int i = 0; i < count; i++)
        {
            float newX = startX + (i * spacing);
            Vector3 targetPos = new Vector3(newX, 0, 0);
            cards[i].localPosition = Vector3.Lerp(cards[i].localPosition, targetPos, Time.deltaTime * 10f);
        }
    }

    void LateUpdate()
    {
        if (transform.childCount > 0) UpdateLayout();
    }
}
