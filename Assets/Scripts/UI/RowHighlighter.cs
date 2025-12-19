using UnityEngine;

public class RowHighlighter : MonoBehaviour
{
    private BoardRow rowLogic;
    private SpriteRenderer rowImage;
    private Color defaultColor;
    public Color highlightColor = new Color(0.3f, 1f, 0.3f, 0.5f);

    void Start()
    {
        rowLogic = GetComponent<BoardRow>();
        rowImage = GetComponent<SpriteRenderer>();
        if (rowImage != null) defaultColor = rowImage.color;

        DragAndPlay.OnCardDragStart += HandleDragStart;
        DragAndPlay.OnCardDragEnd += HandleDragEnd;
    }

    private void OnDestroy()
    {
        DragAndPlay.OnCardDragStart -= HandleDragStart;
        DragAndPlay.OnCardDragEnd -= HandleDragEnd;
    }

    private void HandleDragStart(CardInstance card)
    {
        bool isMyRow = rowLogic.isPlayerRow;
        bool matchesRange = (card.data.range == RangeType.Dowolny) || (card.data.range == rowLogic.rowType);

        if (isMyRow && matchesRange)
        {
            if (rowImage != null) rowImage.color = highlightColor;
        }
    }

    private void HandleDragEnd()
    {
        if (rowImage != null) rowImage.color = defaultColor;
    }
}
