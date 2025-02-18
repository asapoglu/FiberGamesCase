using UnityEngine;
using Zenject;
using System;
/// <summary>
/// Tüm kule tiplerinin temel sınıfı.
/// Temel kule davranışlarını ve özellikleri içerir.
/// </summary>
public abstract class BaseTower : MonoBehaviour, ITower
{
    public Transform Transform => transform;
    protected ITowerStats _towerStats;
    protected TowerPlacementManager _towerPlacementManager;
    protected int _currentHealth;
    protected float _attackCooldown;
    protected string _entityId;
    protected bool _isActive = false;
    protected IDamageable _currentTarget;
    protected GridCell _currentCell; // Grid hücre referansını tut

    [SerializeField] protected LayerMask enemyLayerMask;
    [SerializeField] protected Transform turretHead;
    [SerializeField] protected float turretRotationSpeed = 10f;

    public event Action<float> OnDamageTaken;
    public event Action<IDamageable> OnDeath;
    public event Action<Vector3> OnDestinationReached;
    public event Action<IDamageable, float> OnAttackPerformed;

    public Transform TurretHead => turretHead;
    public float TurretRotationSpeed => turretRotationSpeed;

    // ICombatEntity implementation
    public int CurrentHealth => _currentHealth;
    public int MaxHealth => _towerStats?.Health ?? 0;
    public bool IsDead => _currentHealth <= 0;
    public float MoveSpeed => 0f;
    public Vector3 CurrentPosition => transform.position;
    public string EntityId => _entityId;
    public EntityType Type => EntityType.Tower;
    public bool IsActive => _isActive;
    public bool HasReachedDestination => true;

    // IAttacker implementation
    public float Damage => _towerStats?.Damage ?? 0;
    public float AttackRange => _towerStats?.AttackRange ?? 0;
    public float AttackRate => _towerStats?.AttackSpeed ?? 0;
    public bool CanAttack => _attackCooldown <= 0f;

    [Inject]
    public void Construct(TowerPlacementManager towerPlacementManager)
    {
        _towerPlacementManager = towerPlacementManager;
        _entityId = $"Tower_{Guid.NewGuid().ToString("N")}";
    }

    public virtual void Initialize()
    {
        _currentHealth = _towerStats.Health;
        _attackCooldown = 0f;
        _isActive = true;
    }

    public virtual void TakeDamage(float damage)
    {
        if (!_isActive || IsDead) return;

        _currentHealth -= (int)damage;
        OnDamageTaken?.Invoke(damage);

        if (IsDead)
        {
            Die();
        }
    }

    public virtual void Attack(IDamageable target)
    {
        if (!CanAttack || !_isActive || target == null || target.IsDead) return;

        float distance = Vector3.Distance(transform.position, (target as MonoBehaviour).transform.position);
        if (distance <= AttackRange)
        {
            target.TakeDamage(Damage);
            _attackCooldown = AttackRate;
            OnAttackPerformed?.Invoke(target, Damage);
        }
    }



    protected virtual void Die()
    {
        if (_currentCell != null)
        {
            _currentCell.isAvailable = true; // Hücreyi serbest bırak
        }
        _isActive = false;
        OnDeath?.Invoke(this);
        Destroy(gameObject);
    }

    protected virtual void RaiseOnAttackPerformed(IDamageable target, float damage)
    {
        OnAttackPerformed?.Invoke(target, damage);
    }

    public virtual void Deactivate()
    {
        _isActive = false;
    }

    public void SetStats(ITowerStats stats)
    {
        _towerStats = stats;
    }

    protected virtual void Update()
    {
        if (!_isActive) return;

        if (_attackCooldown > 0)
        {
            _attackCooldown -= Time.deltaTime;
        }

        SearchAndAttack();
    }
    /// <summary>
    /// Düşman tespiti ve saldırı yönetimini gerçekleştirir.
    /// </summary>
    protected virtual void SearchAndAttack()
    {
        if (_currentTarget != null)
        {
            var targetMono = _currentTarget as MonoBehaviour;
            if (targetMono != null && !_currentTarget.IsDead)
            {
                float distance = Vector3.Distance(transform.position, targetMono.transform.position);
                if (distance <= AttackRange)
                {
                    RotateTurretHead(targetMono.transform.position);
                    Attack(_currentTarget);
                    return;
                }
            }
            _currentTarget = null;
        }

        Collider[] hits = Physics.OverlapSphere(transform.position, AttackRange, enemyLayerMask);
        foreach (var hit in hits)
        {
            var targetEntity = hit.GetComponent<IDamageable>();
            if (targetEntity != null && !targetEntity.IsDead)
            {
                _currentTarget = targetEntity;
                var targetTransform = hit.transform;
                RotateTurretHead(targetTransform.position);
                Attack(targetEntity);
                break;
            }
        }
    }
    /// <summary>
    /// Kule başlığını hedefe doğru döndürür.
    /// </summary>
    /// <param name="targetPosition">Hedef pozisyonu</param>
    protected virtual void RotateTurretHead(Vector3 targetPosition)
    {
        if (turretHead == null) return;

        Vector3 direction = targetPosition - turretHead.position;
        direction.y = 0;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        turretHead.rotation = Quaternion.Slerp(
            turretHead.rotation,
            targetRotation,
            Time.deltaTime * turretRotationSpeed
        );
    }
    public void MoveTo(Vector3 destination)
    {
    }

    public void SetGridCell(GridCell cell)
    {
        _currentCell = cell;
    }
}