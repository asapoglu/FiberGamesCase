using UnityEngine;

[CreateAssetMenu(fileName = "StandardTowerStats", menuName = "FiberCase/Tower/Standard Tower Stats")]
public class StandardTowerStats : BaseTowerStats
{
    private void OnEnable()
    {
        towerType = TowerType.Standard;
    }
}