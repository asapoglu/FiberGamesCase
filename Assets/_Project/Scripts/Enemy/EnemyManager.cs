using UnityEngine;
using Zenject;
using System.Collections;

public class EnemyManager : MonoBehaviour
{
    public GameObject enemyPrefab; // Inspector'da enemy prefab'ını atayın.
    public Transform spawnPoint;    // Enemylerin spawn olacağı nokta.
    public Transform targetPoint;   // Enemylerin ulaşmaya çalışacağı hedef (örneğin, savunma noktası).

    public float spawnInterval = 2f; // Dalga içindeki enemy spawn aralığı.
    public int enemiesPerWave = 5;   // Her dalgada oluşturulacak enemy sayısı.
    public float waveInterval = 10f; // Dalga arası bekleme süresi.

    [Inject]
    private DiContainer _container;  // Zenject container injection ile sağlanır.

    void Start()
    {
        StartCoroutine(SpawnWaves());
    }

    IEnumerator SpawnWaves()
    {
        while(true)
        {
            for(int i = 0; i < enemiesPerWave; i++)
            {
                SpawnEnemy();
                yield return new WaitForSeconds(spawnInterval);
            }
            yield return new WaitForSeconds(waveInterval);
        }
    }

    void SpawnEnemy()
    {
        Vector3 spawnPosition = GetRandomSpawnPosition();
        Enemy enemy = _container.InstantiatePrefabForComponent<Enemy>(enemyPrefab, spawnPosition, Quaternion.identity, transform);
        if(enemy != null)
        {
            enemy.SetTarget(targetPoint);
        }
    }

    Vector3 GetRandomSpawnPosition()
    {
        return spawnPoint.position + new Vector3(Random.Range(-10f, 10f), 0, 0);
    }
}