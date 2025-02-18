# Tower Defense Game - Technical Documentation

## 1. Framework Seçimi: Zenject

### Neden Zenject?
- Unity ile sorunsuz entegrasyon
- Scene-based bağımlılık yönetimi
- Constructor ve method injection desteği
- Yüksek performans ve az overhead
- Aktif topluluk desteği

### Zenject Kullanımı
```csharp
// Bağımlılıkların merkezi yönetimi
public class GameInstaller : MonoInstaller
{
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
```
```csharp
// Constructor injection örneği
public class TowerPlacementManager
{
    [Inject]
    public void Construct(GridManager gridManager, DiContainer container)
    {
        _gridManager = gridManager;
        _container = container;
    }
}
```
```csharp
// Prefab Instantiation
var enemy = _container.InstantiatePrefabForComponent<IEnemy>(
    enemyPrefab,
    spawnPosition,
    Quaternion.identity
);
```

## 2. Sınıf Yapıları ve İşlevleri

### Manager Sınıfları

#### EnemyManager
```csharp
public class EnemyManager : MonoBehaviour
{
    // İşlevler:
    // - Düşman dalgalarının yönetimi
    // - Düşman spawn işlemleri
    // - Aktif düşmanların takibi
    // - Wave başlangıç/bitiş eventleri
}
```

#### TowerPlacementManager
```csharp
public class TowerPlacementManager : MonoBehaviour
{
    // İşlevler:
    // - Kule yerleştirme mekanizması
    // - Grid sistemi ile etkileşim
    // - Kule preview sistemi
    // - Kule yaratma ve yok etme işlemleri
}
```

#### GridManager
```csharp
public class GridManager : MonoBehaviour
{
    // İşlevler:
    // - Grid sisteminin oluşturulması
    // - Hücre durumlarının yönetimi
    // - Dünya koordinatı - grid dönüşümleri
}
```

#### GameUIManager
```csharp
public class GameUIManager : MonoBehaviour
{
    // İşlevler:
    // - UI güncellemeleri
    // - Wave bilgisi gösterimi
    // - Düşman ve kule sayaçları
}
```

### Combat Sınıfları

#### BaseTower
```csharp
public abstract class BaseTower : MonoBehaviour
{
    // İşlevler:
    // - Temel kule davranışları
    // - Hedef tespiti ve takibi
    // - Saldırı mekanizması
    // - Hasar alma sistemi
}
```

#### Enemy
```csharp
public class Enemy : MonoBehaviour
{
    // İşlevler:
    // - Düşman hareketi ve navigasyonu
    // - Kule tespiti ve saldırı
    // - Can ve hasar sistemi
    // - Hedef noktaya ulaşma kontrolü
}
```

### UI Sınıfları



#### HealthBarUI
```csharp
public class HealthBarUI : MonoBehaviour
{
    // İşlevler:
    // - Can barı görselleştirmesi
    // - Fade in/out efektleri
    // - Pozisyon takibi
}
```

## 3. Interface Yapısı ve Entity Mimarisi

### Core Interfaces

#### ICombatEntity
```csharp
public interface ICombatEntity : IDamageable, IMoveable
{
    string EntityId { get; }
    EntityType Type { get; }
    bool IsActive { get; }
    void Initialize();
    void Deactivate();
}
```
- Tüm savaş birimlerinin (Tower ve Enemy) temel arayüzü
- IDamageable ve IMoveable'ı birleştirir
- Entity yönetimi için temel fonksiyonları sağlar

#### IDamageable
```csharp
public interface IDamageable
{
    int CurrentHealth { get; }
    int MaxHealth { get; }
    bool IsDead { get; }
    void TakeDamage(float damage);
    event Action<float> OnDamageTaken;
    event Action<IDamageable> OnDeath;
}
```
- Can sistemi ve hasar alma mekanizmalarını tanımlar
- Health bar sistemi bu interface üzerinden çalışır

#### IMoveable
```csharp
public interface IMoveable
{
    float MoveSpeed { get; }
    Vector3 CurrentPosition { get; }
    void MoveTo(Vector3 destination);
    bool HasReachedDestination { get; }
    event Action<Vector3> OnDestinationReached;
}
```
- Hareket sistemi için gerekli özellikleri tanımlar
- NavMesh ile entegre çalışır

#### IAttacker
```csharp
public interface IAttacker
{
    float Damage { get; }
    float AttackRange { get; }
    float AttackRate { get; }
    bool CanAttack { get; }
    void Attack(IDamageable target);
    event Action<IDamageable, float> OnAttackPerformed;
}
```
- Saldırı mekanizmalarını tanımlar
- Menzil ve cooldown sistemini yönetir

### Enemy Yapısı

#### IEnemy Interface
```csharp
public interface IEnemy : ICombatEntity
{
    void SetTarget(Transform target);
    void SetStats(IEnemyStats stats);
    void StartMoving();
    void StopMoving();
    void ReturnToMainTarget();
    float AttackRange { get; }
    bool IsAttacking { get; }
}
```

#### Enemy Class Hierarchy
```
MonoBehaviour
    └── Enemy (implements IEnemy)
            ├── Movement System
            │   ├── NavMeshAgent entegrasyonu
            │   ├── Hedef takibi
            │   └── Yol bulma
            │
            ├── Combat System
            │   ├── Tower tespiti
            │   ├── Saldırı mekanizması
            │   └── Hasar sistemi
            │
            └── State Management
                ├── Active/Inactive durumları
                ├── Hareket/Saldırı durumları
                └── Health yönetimi
```

#### IEnemyStats
```csharp
public interface IEnemyStats
{
    float Damage { get; }
    int Health { get; }
    float MoveSpeed { get; }
    float AttackSpeed { get; }
    float DetectionRange { get; }
    float AttackRange { get; }
    float ArrivalDistance { get; }
}
```
- Düşman birimlerinin özelliklerini tanımlar
- ScriptableObject üzerinden yapılandırılır

### Tower Yapısı

#### ITower Interface
```csharp
public interface ITower : ICombatEntity, IAttacker
{
    Transform TurretHead { get; }
    Transform Transform { get; }
    float TurretRotationSpeed { get; }
    void SetStats(ITowerStats stats);
}
```

#### Tower Class Hierarchy
```
MonoBehaviour
    └── BaseTower (abstract, implements ITower)
            ├── StandardTower
            │   └── Temel kule davranışları
            │
            └── RapidFireTower
                └── Özel burst-attack mekanizması

BaseTower özellikleri:
- Hedef tespiti ve takibi
- Turret rotasyonu
- Saldırı mekanizması
- Hasar alma sistemi
```

#### ITowerStats
```csharp
public interface ITowerStats
{
    float AttackRange { get; }
    float AttackSpeed { get; }
    float Damage { get; }
    int Health { get; }
}
```

#### Tower Tipleri ve Özellikleri

1. StandardTower
```csharp
public class StandardTower : BaseTower
{
    // Temel kule davranışları
    // - Tekli hedef
    // - Sabit hasar
    // - Normal atak hızı
}
```

2. RapidFireTower
```csharp
public class RapidFireTower : BaseTower
{
    // Özel yetenekler:
    // - Burst saldırı sistemi
    // - Yüksek atak hızı
    // - Burst count ve interval konfigürasyonu
}
```

### Stats Sistemi
```
ScriptableObject
    ├── BaseTowerStats (abstract)
    │   ├── StandardTowerStats
    │   └── RapidFireTowerStats
    │
    └── EnemyStats
        ├── BasicEnemyStats
        ├── FastEnemyStats
        └── TankEnemyStats
```
- Scriptable Object tabanlı veri yönetimi
- Runtime'da değiştirilebilir özellikler
- Kolay genişletilebilir yapı

## 4. Event Sistemi

### Düşman Eventleri
```csharp
public event Action<IEnemy> OnEnemySpawned;
public event Action<IEnemy> OnEnemyDied;
public event Action<int> OnWaveStarted;
public event Action<int> OnWaveCompleted;
```

### Kule Eventleri
```csharp
public event Action<ITower> OnTowerPlaced;
public event Action<ITower> OnTowerDestroyed;
public event Action<IDamageable, float> OnAttackPerformed;
```

## 5. Scriptable Objects

### EnemyStats
- Düşman özellikleri ve istatistikleri
- Farklı düşman tipleri için ayarlar

### TowerStats
- Kule istatistikleri ve özellikleri
- Farklı kule tipleri için ayarlar

### WaveConfiguration
- Dalga yapılandırmaları
- Düşman spawn zamanlamaları

## 6. Component İletişimi

### Observer Pattern
- Event sistemi ile loose coupling
- Manager sınıfları arası iletişim

### Interface-Based Communication
- Bağımlılıkların interface'ler üzerinden yönetimi
- Modüler ve değiştirilebilir yapı

## 7. Asset Yönetimi

### Prefab Sistemi
- Düşman ve kule prefabları
- UI elementleri

### Scriptable Object Database
- Düşman veritabanı
- Kule veritabanı
- Wave konfigürasyonları