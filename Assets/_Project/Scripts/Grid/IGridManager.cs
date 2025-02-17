using UnityEngine;

public interface IGridManager {
    void ShowGrid(bool visible);
    GridCell GetCellFromWorldPosition(Vector3 worldPos);
}