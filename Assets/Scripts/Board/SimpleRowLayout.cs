using UnityEngine;
using System.Collections.Generic;

public class SimpleRowLayout : MonoBehaviour
{
    [Header("Settings")]
    public float spacing = 1.5f;
    public float moveSpeed = 10f;

    [Header("Ghost")]
    public GameObject ghostPrefab;
    private GameObject currentGhost;

    private void OnTransformChildrenChanged()
    {
        UpdateLayout();
    }

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

        if (currentGhost.transform.GetSiblingIndex() != bestIndex)
        {
            currentGhost.transform.SetSiblingIndex(bestIndex);
            UpdateLayout();
        }
    }

    public void RemoveGhost()
    {
        if (currentGhost != null)
        {
            currentGhost.transform.SetParent(null);
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

    public int CalculateIndexForX(float x)
    {
        int i = 0;

        foreach (Transform child in transform)
        {
            if (currentGhost != null && child == currentGhost.transform) continue;
            if (!child.gameObject.activeSelf) continue;

            if (x < child.position.x)
            {
                return i;
            }
            i++;
        }

        return transform.childCount;
    }

    void LateUpdate()
    {
        if (transform.childCount > 0) UpdateLayout();
    }

    [ContextMenu("Arrange the cards")]
    public void UpdateLayout()
    {
        List<Transform> cards = new List<Transform>();
        foreach (Transform child in transform)
        {
            if (child.gameObject.activeInHierarchy) cards.Add(child);
        }

        int count = cards.Count;
        if (count == 0) return;

        float totalWidth = (count - 1) * spacing;
        float startX = -totalWidth / 2f;

        for (int i = 0; i < count; i++)
        {
            Transform card = cards[i];

            float newX = startX + (i * spacing);
            Vector3 targetPos = new Vector3(newX, 0, 0);

            if (currentGhost != null && card == currentGhost.transform)
                card.localPosition = targetPos;
            else
                card.localPosition = Vector3.Lerp(card.localPosition, targetPos, Time.deltaTime * moveSpeed);
        }
    }
}
