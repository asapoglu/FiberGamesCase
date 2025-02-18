using UnityEngine;
/// <summary>
/// Oyun alanının grid sistemini yöneten sınıf.
/// Kule yerleştirme için grid hücrelerini oluşturur ve yönetir.
/// </summary>
public class GridManager : MonoBehaviour
{
    public int rows = 10;
    public int columns = 10;
    public float cellSize = 1f;
    public GameObject gridCellPrefab;

    private GridCell[,] gridCells;

    void Start()
    {
        CreateGrid();
        ShowGrid(false); 
    }
    /// <summary>
    /// Grid sistemini oluşturur.
    /// Hücreleri yerleştirir ve başlangıç ayarlarını yapar.
    /// </summary>
    void CreateGrid()
    {
        gridCells = new GridCell[rows, columns];
        Vector3 startPosition = transform.position - new Vector3((columns - 1) * cellSize / 2f, 0, (rows - 1) * cellSize / 2f);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                Vector3 cellPosition = startPosition + new Vector3(j * cellSize, 0, i * cellSize);
                GameObject cellObj = Instantiate(gridCellPrefab, cellPosition, Quaternion.identity, transform);
                GridCell cell = cellObj.GetComponent<GridCell>();
                cell.Initialize(i, j);
                gridCells[i, j] = cell;
            }
        }
    }

    public void ShowGrid(bool visible)
    {
        foreach (GridCell cell in gridCells)
        {
            cell.gameObject.SetActive(visible);
        }
    }
    /// <summary>
    /// Dünya koordinatlarına karşılık gelen grid hücresini bulur.
    /// </summary>
    /// <param name="worldPos">Dünya koordinatları</param>
    /// <returns>İlgili grid hücresi veya null</returns>
    public GridCell GetCellFromWorldPosition(Vector3 worldPos)
    {
        Vector3 gridOrigin = transform.position - new Vector3((columns - 1) * cellSize / 2f, 0, (rows - 1) * cellSize / 2f);
        int j = Mathf.RoundToInt((worldPos.x - gridOrigin.x) / cellSize);
        int i = Mathf.RoundToInt((worldPos.z - gridOrigin.z) / cellSize);
        if (i >= 0 && i < rows && j >= 0 && j < columns)
        {
            return gridCells[i, j];
        }
        return null;
    }
}