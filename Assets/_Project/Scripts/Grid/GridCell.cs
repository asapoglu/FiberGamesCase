using UnityEngine;

public class GridCell : MonoBehaviour
{
    public int row;
    public int column;
    public bool isAvailable = true;
    [SerializeField] private SpriteRenderer highlightRenderer;
    [SerializeField] private SpriteRenderer cellRenderer;
    public void Initialize(int _row, int _column)
    {
        row = _row;
        column = _column;
    }
    public void Highlight()
    {
        highlightRenderer.enabled = true;
    }

    // Vurgulamayı kaldır
    public void UnHighlight()
    {
        highlightRenderer.enabled = false;
    }
}