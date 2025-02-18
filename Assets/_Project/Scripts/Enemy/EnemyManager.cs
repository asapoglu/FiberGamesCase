using UnityEngine;
using Zenject;
using System.Collections;
using System.Collections.Generic;
using System;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private Transform targetPoint;
    private DiContainer _container;
    private WaveConfiguration _waveConfig;
    private EnemyDatabase _enemyDatabase;
    private HealthBarManager _healthBarManager;
    private int _currentWave = 0;
    private int _remainingEnemies = 0;
    private List<IEnemy> _activeEnemies = new List<IEnemy>();
    public int ActiveEnemyCount => _activeEnemies.Count;

    private bool _isWaveInProgress = false;

    /// <summary>
    /// Yeni bir düşman dalgası başlattığında çağrılır.
    /// Wave sayısını parametre olarak alır.
    /// </summary>
    public event Action<int> OnWaveStarted;
    public event Action<int> OnWaveCompleted;
    public event Action<IEnemy> OnEnemySpawned;
    public event Action<IEnemy> OnEnemyReachedTarget;
    public event Action<IEnemy> OnEnemyDied;

    [Inject]
    public void Construct(DiContainer container, WaveConfiguration waveConfiguration, EnemyDatabase enemyDatabase, HealthBarManager healthBarManager)
    {
        _container = container;
        _waveConfig = waveConfiguration;
        _enemyDatabase = enemyDatabase;
        _healthBarManager = healthBarManager;
    }

    void Start()
    {
        StartCoroutine(WaveManager());
    }

    private IEnumerator WaveManager()
    {
        yield return new WaitForSeconds(_waveConfig.initialDelay);

        while (_currentWave < _waveConfig.waves.Length)
        {
            var currentWaveData = _waveConfig.waves[_currentWave];
            yield return StartWave(currentWaveData);
            yield return new WaitForSeconds(currentWaveData.waveInterval);
            _currentWave++;
        }
    }
    /// <summary>
    /// Wave'deki düşmanların oluşturulmasını ve yönetimini sağlar.
    /// </summary>
    /// <param name="waveData">Dalga konfigürasyon bilgileri</param>
    private IEnumerator StartWave(WaveData waveData)
    {
        _isWaveInProgress = true;
        _remainingEnemies = waveData.enemyCount;
        OnWaveStarted?.Invoke(_currentWave);

        for (int i = 0; i < waveData.enemyCount; i++)
        {
            SpawnEnemy(waveData.enemyTypes[i % waveData.enemyTypes.Length]);
            yield return new WaitForSeconds(waveData.spawnInterval);
        }

        while (_remainingEnemies > 0)
        {
            yield return null;
        }

        _isWaveInProgress = false;
        OnWaveCompleted?.Invoke(_currentWave);
    }
    /// <summary>
    /// Belirtilen türde düşman oluşturur ve başlangıç ayarlarını yapar.
    /// </summary>
    /// <param name="enemyType">Oluşturulacak düşman türü</param>
    private void SpawnEnemy(EnemyType enemyType)
    {
        Vector3 spawnPosition = GetRandomSpawnPosition();

        // Database'den enemy verilerini al
        var enemyData = _enemyDatabase.GetEnemyData(enemyType);
        if (enemyData == null) return;

        // Enemy'yi oluştur ve IEnemy interface'ini al
        var enemyObject = _container.InstantiatePrefabForComponent<IEnemy>(
            enemyData.prefab,
            spawnPosition,
            Quaternion.identity,
            transform
        );

        if (enemyObject != null)
        {
            // Stats'leri ayarla
            enemyObject.SetStats(enemyData.stats);

            // Enemy'yi başlat
            enemyObject.Initialize();
            enemyObject.SetTarget(targetPoint);
            _healthBarManager.CreateHealthBar(enemyObject, (enemyObject as MonoBehaviour).transform);


            // Event'leri bağla
            enemyObject.OnDeath += HandleEnemyDeath;
            enemyObject.OnDestinationReached += (pos) => HandleEnemyReachedTarget(enemyObject);

            // Listeye ekle ve event'i tetikle
            _activeEnemies.Add(enemyObject);
            OnEnemySpawned?.Invoke(enemyObject);
        }
        else
        {
            Debug.LogError($"Failed to spawn enemy of type {enemyType}");
            _remainingEnemies--; // Spawn başarısız olduğu için sayacı azalt
        }
    }

    private void HandleEnemyDeath(IDamageable damageable)
    {
        var enemy = damageable as Enemy;

        if (_activeEnemies.Contains(enemy))
        {
            _activeEnemies.Remove(enemy);
            _remainingEnemies--;
            OnEnemyDied?.Invoke(enemy);
        }
    }

    private void HandleEnemyReachedTarget(IEnemy enemy)
    {
        if (_activeEnemies.Contains(enemy))
        {
            _activeEnemies.Remove(enemy);
            _remainingEnemies--;
            OnEnemyReachedTarget?.Invoke(enemy);
            enemy.Deactivate();
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points assigned!");
            return Vector3.zero;
        }

        var randomSpawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
        Vector3 randomOffset = new Vector3(
            UnityEngine.Random.Range(-2f, 2f),
            0,
            UnityEngine.Random.Range(-2f, 2f)
        );

        return randomSpawnPoint.position + randomOffset;
    }

    // Public getters
    public bool IsWaveInProgress => _isWaveInProgress;
    public int CurrentWaveNumber => _currentWave;
    public int TotalWaves => _waveConfig.waves.Length;
    public IReadOnlyList<IEnemy> ActiveEnemies => _activeEnemies.AsReadOnly();

}