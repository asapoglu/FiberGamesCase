using UnityEngine;
using Zenject;

public class TowerPlacementManager : MonoBehaviour, ITowerPlacementManager
{
    private IGridManager _gridManager;
    private TowerPlacement _currentTower;
    [Inject]
    private DiContainer _container;
    public GameObject towerPrefab; // Tower prefab'ı Inspector'dan atanmalı.

    [Inject]
    public void Construct(IGridManager gridManager)
    {
        _gridManager = gridManager;
    }

    public void SpawnTower()
    {
        if (_currentTower != null) return;

        _gridManager.ShowGrid(true);        
        _currentTower = _container.InstantiatePrefabForComponent<TowerPlacement>(towerPrefab, Vector3.zero, Quaternion.identity, null);
        _currentTower.SetPlacementMode(true);
    }

    public void PlaceTower(GridCell cell)
    {
        if (cell != null && cell.isAvailable)
        {
            cell.isAvailable = false;
            _currentTower.transform.position = cell.transform.position;
            _currentTower.SetPlacementMode(false);
            _currentTower = null;
            _gridManager.ShowGrid(false);
        }
    }

    public void CancelPlacement()
    {
        if (_currentTower != null)
        {
            Destroy(_currentTower.gameObject);
            _currentTower = null;
        }
        _gridManager.ShowGrid(false);
    }
}