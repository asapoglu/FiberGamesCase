using System;
using UnityEngine;

[Serializable]
public class WaveData
{
    public int waveNumber;
    public int enemyCount;
    public float spawnInterval;
    public float waveInterval;
    public EnemyType[] enemyTypes;
}

[CreateAssetMenu(fileName = "WaveConfig", menuName = "FiberCase/Enemy Manager/Wave Configuration")]
public class WaveConfiguration : ScriptableObject
{
    public WaveData[] waves;
    public float initialDelay = 3f;
}

public enum EnemyType
{
    Basic,
    Fast,
    Tank
}