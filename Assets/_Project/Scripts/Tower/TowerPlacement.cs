using UnityEngine;
using Zenject;

public class TowerPlacement : MonoBehaviour
{
    private ITowerPlacementManager _towerPlacementManager;
    private IGridManager _gridManager;
    private bool _inPlacementMode = true;
    private GridCell _currentHighlightedCell;
    
    [Inject]
    public void Construct(ITowerPlacementManager towerPlacementManager, IGridManager gridManager)
    {
        _towerPlacementManager = towerPlacementManager;
        _gridManager = gridManager;
    }
    
    void Update()
    {
        if (!_inPlacementMode) return;
        
        Vector3 mouseWorldPos = GetMouseWorldPosition();
        GridCell cell = _gridManager.GetCellFromWorldPosition(mouseWorldPos);
        if (cell != null)
        {
            // Tower'ı grid hücresinin merkezine snapler
            transform.position = cell.transform.position;
            
            if (_currentHighlightedCell != cell)
            {
                if (_currentHighlightedCell != null)
                    _currentHighlightedCell.UnHighlight();
                cell.Highlight();
                _currentHighlightedCell = cell;
            }
            
            // Sol mouse tuşu bırakıldığında yerleştirme tamamlanır
            if (Input.GetMouseButtonUp(0))
            {
                _towerPlacementManager.PlaceTower(cell);
                cell.UnHighlight();
                _inPlacementMode = false;
                // Yerleştirme tamamlandıktan sonra bu bileşeni devre dışı bırakabiliriz
                this.enabled = false;
            }
        }
        else
        {
            if (_currentHighlightedCell != null)
            {
                _currentHighlightedCell.UnHighlight();
                _currentHighlightedCell = null;
            }
        }
    }
    
    private Vector3 GetMouseWorldPosition()
    {
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(ray, out float distance))
            return ray.GetPoint(distance);
        return Vector3.zero;
    }
    
    // Dışarıdan yerleştirme modunun durumu ayarlanabilir.
    public void SetPlacementMode(bool mode)
    {
        _inPlacementMode = mode;
    }
}