using Zenject;
using UnityEngine;

public class GameInstaller : MonoInstaller
{
    public GridManager gridManager;
    public TowerPlacementManager towerPlacementManager;
    public EnemyManager enemyManager; // EnemyManager referansı
    public TowerStats towerStats;  // Tower için ScriptableObject
    public EnemyStats enemyStats;  // Enemy için ScriptableObject

    public override void InstallBindings()
    {
        Container.Bind<IGridManager>().FromInstance(gridManager).AsSingle();
        Container.Bind<ITowerPlacementManager>().FromInstance(towerPlacementManager).AsSingle();
        Container.Bind<ITowerStats>().FromInstance(towerStats).AsSingle();
        Container.Bind<IEnemyStats>().FromInstance(enemyStats).AsSingle();
        Container.Bind<EnemyManager>().FromInstance(enemyManager).AsSingle();
    }
}