using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "TowerDatabase", menuName = "FiberCase/Tower/Tower Database")]
public class TowerDatabase : ScriptableObject
{
    [SerializeField] private BaseTowerStats[] towerStats;
    private Dictionary<TowerType, BaseTowerStats> _statsMap;

    private void OnEnable()
    {
        InitializeStatsMap();
    }

    private void InitializeStatsMap()
    {
        _statsMap = new Dictionary<TowerType, BaseTowerStats>();
        foreach (var stats in towerStats)
        {
            if (stats != null)
            {
                _statsMap[stats.TowerType] = stats;
            }
        }
    }

    public BaseTowerStats GetTowerStats(TowerType type)
    {
        if (_statsMap == null)
        {
            InitializeStatsMap();
        }

        return _statsMap.TryGetValue(type, out var stats) ? stats : null;
    }
}