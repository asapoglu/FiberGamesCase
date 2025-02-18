using UnityEngine;
using Zenject;
using System;
using System.Collections.Generic;
/// <summary>
/// Kulelerin yerleştirilmesi ve yönetiminden sorumlu sınıf.
/// Grid sistemi üzerinde kule yerleştirme, preview gösterme ve kule durumlarını takip eder.
/// </summary>
public class TowerPlacementManager : MonoBehaviour
{

    private GridManager _gridManager;
    private DiContainer _container;
    private ITower _currentTowerPreview;
    private TowerPlacement _currentPlacement;
    private HealthBarManager _healthBarManager;
    private TowerDatabase _towerDatabase;

    private List<ITower> _activeTowers = new List<ITower>();

    public event Action<ITower> OnTowerPlaced;
    public event Action<ITower> OnTowerDestroyed; // Yeni event ekleyelim

    public int ActiveTowerCount => _activeTowers.Count;

    public event Action OnPlacementCancelled;

    [Inject]
    public void Construct(
        GridManager gridManager,
        DiContainer container,
        HealthBarManager healthBarManager,
        TowerDatabase towerDatabase)
    {
        _gridManager = gridManager;
        _container = container;
        _healthBarManager = healthBarManager;
        _towerDatabase = towerDatabase;
    }
    /// <summary>
    /// Yeni bir kule önizlemesi oluşturur.
    /// </summary>
    /// <param name="towerType">Oluşturulacak kule tipi</param>
    public void SpawnTower(int towerType)
    {
        if (_currentTowerPreview != null) return;

        var towerStats = _towerDatabase.GetTowerStats((TowerType)towerType);
        if (towerStats == null || towerStats.TowerPrefab == null) return;

        _gridManager.ShowGrid(true);

        var previewTower = _container.InstantiatePrefabForComponent<ITower>(
            towerStats.TowerPrefab,
            Vector3.zero,
            Quaternion.identity,
            null);

        if (previewTower != null)
        {
            previewTower.SetStats(towerStats);
            _currentTowerPreview = previewTower;
            _currentPlacement = _container.InstantiateComponent<TowerPlacement>(
                (previewTower as MonoBehaviour).gameObject
            );
            _currentPlacement.SetPlacementMode(true);
        }
    }
    /// <summary>
    /// Kuleyi grid hücresine yerleştirir.
    /// </summary>
    /// <param name="cell">Kulenin yerleştirileceği grid hücresi</param>
    public void PlaceTower(GridCell cell)
    {
        if (cell != null && cell.isAvailable && _currentTowerPreview != null)
        {
            cell.isAvailable = false;

            // Kuleyi hücreye yerleştir
            _currentTowerPreview.Transform.position = cell.transform.position;
            _currentTowerPreview.SetGridCell(cell);

            // Placement modunu kapat ve bileşeni kaldır
            if (_currentPlacement != null)
            {
                Destroy(_currentPlacement);
                _currentPlacement = null;
            }

            // Kuleyi başlat
            _currentTowerPreview.Initialize();

            _healthBarManager.CreateHealthBar(_currentTowerPreview, _currentTowerPreview.Transform);
            _activeTowers.Add(_currentTowerPreview);
            _currentTowerPreview.OnDeath += HandleTowerDestroyed;

            OnTowerPlaced?.Invoke(_currentTowerPreview);

            // Referansları temizle
            _currentTowerPreview = null;
            _gridManager.ShowGrid(false);
        }
    }

    public void CancelPlacement()
    {
        if (_currentTowerPreview != null)
        {
            Destroy(_currentTowerPreview.Transform.gameObject);
            _currentTowerPreview = null;
            _currentPlacement = null;
        }

        _gridManager.ShowGrid(false);
        OnPlacementCancelled?.Invoke();
    }
    private void HandleTowerDestroyed(IDamageable damageable)
    {
        var tower = damageable as ITower;
        if (tower != null && _activeTowers.Contains(tower))
        {
            _activeTowers.Remove(tower);
            OnTowerDestroyed?.Invoke(tower);
        }

    }
}