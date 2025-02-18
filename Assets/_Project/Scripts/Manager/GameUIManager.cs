using UnityEngine;
using TMPro;
using Zenject;

public class GameUIManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI activeTowersText;
    [SerializeField] private TextMeshProUGUI currentWaveText;
    [SerializeField] private TextMeshProUGUI remainingEnemiesText;

    private EnemyManager _enemyManager;
    private TowerPlacementManager _towerManager;

    [Inject]
    public void Construct(EnemyManager enemyManager, TowerPlacementManager towerManager)
    {
        _enemyManager = enemyManager;
        _towerManager = towerManager;

        _enemyManager.OnEnemySpawned += enemy => UpdateEnemiesUI();
        _enemyManager.OnWaveStarted += UpdateWaveUI;
        _enemyManager.OnEnemyDied += _ => UpdateEnemiesUI();
        _towerManager.OnTowerPlaced += _ => UpdateTowersUI();
        _towerManager.OnTowerDestroyed += _ => UpdateTowersUI(); 
    }

    private void Start()
    {
        UpdateAllUI();
    }

    private void UpdateAllUI()
    {
        UpdateWaveUI(_enemyManager.CurrentWaveNumber);
        UpdateEnemiesUI();
        UpdateTowersUI();
    }

    private void UpdateWaveUI(int waveNumber)
    {
        if (currentWaveText != null)
        {
            currentWaveText.text = $"Wave: {waveNumber+1}/{_enemyManager.TotalWaves}";
        }
    }

    private void UpdateEnemiesUI()
    {
        if (remainingEnemiesText != null)
        {
            remainingEnemiesText.text = $"Enemies: {_enemyManager.ActiveEnemyCount}";
        }
    }

    private void UpdateTowersUI()
    {
        if (activeTowersText != null)
        {
            activeTowersText.text = $"Active Towers: {_towerManager.ActiveTowerCount}";
        }
    }

    private void OnDestroy()
    {
        if (_enemyManager != null)
        {
            _enemyManager.OnEnemySpawned -= enemy => UpdateEnemiesUI();
            _enemyManager.OnWaveStarted -= UpdateWaveUI;
            _enemyManager.OnEnemyDied -= _ => UpdateEnemiesUI();
            _towerManager.OnTowerPlaced -= _ => UpdateTowersUI();
            _towerManager.OnTowerDestroyed -= _ => UpdateTowersUI();
        }
    }
}