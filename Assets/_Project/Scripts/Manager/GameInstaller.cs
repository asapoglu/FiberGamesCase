using Zenject;
using UnityEngine;

public class GameInstaller : MonoInstaller
{
    public GridManager gridManager;
    public TowerPlacementManager towerPlacementManager;
    public EnemyManager enemyManager;
    public HealthBarManager healthBarManager;
    public GameUIManager gameUIManager;
    [SerializeField] private TowerDatabase towerDatabase; // TowerDatabase ekleyelim
    public WaveConfiguration waveConfiguration;
    public EnemyDatabase enemyDatabase;

    public override void InstallBindings()
    {
        Container.Bind<HealthBarManager>().FromInstance(healthBarManager).AsSingle();
        Container.Bind<GridManager>().FromInstance(gridManager).AsSingle();
        Container.Bind<TowerPlacementManager>().FromInstance(towerPlacementManager).AsSingle();
        Container.Bind<EnemyManager>().FromInstance(enemyManager).AsSingle();
        Container.Bind<WaveConfiguration>().FromInstance(waveConfiguration).AsSingle();
        Container.Bind<EnemyDatabase>().FromInstance(enemyDatabase).AsSingle();
        Container.Bind<GameUIManager>().FromInstance(gameUIManager).AsSingle();
        Container.Bind<TowerDatabase>().FromInstance(towerDatabase).AsSingle();
    
    }
}