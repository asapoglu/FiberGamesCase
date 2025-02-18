using UnityEngine;

[CreateAssetMenu(fileName = "RapidFireTowerStats", menuName = "FiberCase/Tower/RapidFire Tower Stats")]
public class RapidFireTowerStats : BaseTowerStats
{
    [Header("RapidFire Specific")]
    [SerializeField] private int burstCount = 3;
    [SerializeField] private float burstInterval = 0.1f;

    public int BurstCount => burstCount;
    public float BurstInterval => burstInterval;

    private void OnEnable()
    {
        towerType = TowerType.RapidFire;
    }
}
