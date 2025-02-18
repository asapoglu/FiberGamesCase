using UnityEngine;

[System.Serializable]
public class EnemyTypeData
{
    public EnemyType enemyType;
    public GameObject prefab;
    public EnemyStats stats;
}

[CreateAssetMenu(fileName = "EnemyDatabase", menuName = "FiberCase/Enemy/Enemy Database")]
public class EnemyDatabase : ScriptableObject
{
    public EnemyTypeData[] enemyTypes;

    public EnemyTypeData GetEnemyData(EnemyType type)
    {
        return System.Array.Find(enemyTypes, data => data.enemyType == type);
    }
}