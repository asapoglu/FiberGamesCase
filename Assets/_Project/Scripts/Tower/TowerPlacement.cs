using UnityEngine;
using System;
using Zenject;

[RequireComponent(typeof(ITower))]
public class TowerPlacement : MonoBehaviour
{
    private TowerPlacementManager _towerPlacementManager;
    private GridManager _gridManager;
    private bool _inPlacementMode = false;
    private GridCell _currentHighlightedCell;
    private bool _isValidPlacement = true;

    [Inject]    
    public void Construct(
        TowerPlacementManager towerPlacementManager, 
        GridManager gridManager)
    {
        _towerPlacementManager = towerPlacementManager;
        _gridManager = gridManager;
    }

    void Update()
    {
        if (!_inPlacementMode) return;

        Vector3 mouseWorldPos = GetMouseWorldPosition();
        GridCell cell = _gridManager.GetCellFromWorldPosition(mouseWorldPos);

        HandleCellHighlighting(cell);
        HandlePlacementPreview(cell);
        HandleInput(cell);
    }

    private void HandleCellHighlighting(GridCell cell)
    {
        if (_currentHighlightedCell != cell)
        {
            if (_currentHighlightedCell != null)
                _currentHighlightedCell.UnHighlight();

            if (cell != null)
            {
                cell.Highlight();
                _currentHighlightedCell = cell;
            }
        }
    }

    private void HandlePlacementPreview(GridCell cell)
    {
        if (cell != null)
        {
            transform.position = cell.transform.position;
            bool isValid = cell.isAvailable;
            
            if (isValid != _isValidPlacement)
            {
                _isValidPlacement = isValid;
            }
        }
    }

    private void HandleInput(GridCell cell)
    {
        if (Input.GetMouseButtonUp(0) && cell != null && cell.isAvailable)
        {
            _towerPlacementManager.PlaceTower(cell);
            _inPlacementMode = false;
        }
        else if (Input.GetMouseButtonUp(1) || Input.GetKeyDown(KeyCode.Escape))
        {
            _towerPlacementManager.CancelPlacement();
            _inPlacementMode = false;
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

    public void SetPlacementMode(bool mode)
    {
        _inPlacementMode = mode;
    }

    private void OnDestroy()
    {
        if (_currentHighlightedCell != null)
        {
            _currentHighlightedCell.UnHighlight();
        }
    }
}