using UnityEngine;
using UnityEngine.AI;
using Zenject;
using System;
/// <summary>
/// Düşman davranışlarını kontrol eden sınıf.
/// Hareket, saldırı ve hedef takibi mantığını içerir.
/// </summary>
public class Enemy : MonoBehaviour, IEnemy
{
    private IEnemyStats _enemyStats;
    private NavMeshAgent _agent;
    private Transform _mainTarget;
    private ICombatEntity _currentTarget;
    private int _currentHealth;
    private string _entityId;
    private bool _isActive = true;
    private bool _isAttacking;
    private float _attackCooldown;
    private float _nextDetectionTime;
    private Vector3 _lastTargetPosition;
    private bool _isMovingToAttackPosition;

    [SerializeField] private LayerMask towerLayer;
    [SerializeField] private float detectionInterval = 0.5f;

    public event Action<float> OnDamageTaken;
    public event Action<IDamageable> OnDeath;
    public event Action<Vector3> OnDestinationReached;

    public int CurrentHealth => _currentHealth;
    public int MaxHealth => _enemyStats.Health;
    public bool IsDead => _currentHealth <= 0;
    public float MoveSpeed => _enemyStats.MoveSpeed;
    public Vector3 CurrentPosition => transform.position;
    public string EntityId => _entityId;
    public EntityType Type => EntityType.Enemy;
    public bool IsActive => _isActive;
    public float AttackRange => _enemyStats.AttackRange;
    public bool IsAttacking => _isAttacking;
    public bool HasReachedDestination
    {
        get
        {
            if (_agent == null) return false;

            if (_isMovingToAttackPosition)
            {
                return Vector3.Distance(transform.position, _agent.destination) <= _enemyStats.AttackRange;
            }
            else
            {
                return Vector3.Distance(transform.position, _mainTarget.position) <= _enemyStats.ArrivalDistance;
            }
        }
    }

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _entityId = $"Enemy_{Guid.NewGuid().ToString("N")}";
    }

    public void Initialize()
    {
        if (_enemyStats == null)
        {
            Debug.LogError("Enemy stats not set!");
            return;
        }

        _currentHealth = _enemyStats.Health;
        if (_agent != null)
        {
            _agent.speed = _enemyStats.MoveSpeed;
            _agent.stoppingDistance = _enemyStats.AttackRange * 0.9f;
        }
        _isActive = true;
        _isAttacking = false;
        _isMovingToAttackPosition = false;
        _attackCooldown = 0f;
    }

    void Update()
    {
        if (!_isActive || IsDead) return;

        // Tower'a angaje değilsek ve hedefimize gidiyorsak tower ara
        if (_currentTarget == null && !_isAttacking)
        {
            if (Time.time >= _nextDetectionTime)
            {
                DetectAndEngageTowers();
                _nextDetectionTime = Time.time + detectionInterval;
            }
        }

        UpdateMovement();
        HandleCombat();
    }
    /// <summary>
    /// Menzil içindeki kuleleri tespit eder ve en yakın kuleye saldırı başlatır.
    /// </summary>
    private void DetectAndEngageTowers()
    {
        Collider[] towers = Physics.OverlapSphere(transform.position, _enemyStats.DetectionRange, towerLayer);

        foreach (var tower in towers)
        {
            var combatEntity = tower.GetComponent<ICombatEntity>();
            if (combatEntity != null && !combatEntity.IsDead)
            {
                EngageTower(combatEntity);
                return; // İlk bulunan tower'a yönel ve çık
            }
        }
    }

    private void EngageTower(ICombatEntity tower)
    {
        if (tower == null) return;

        var targetMono = tower as MonoBehaviour;
        if (targetMono == null || targetMono.gameObject == null)
        {
            return;
        }

        _currentTarget = tower;
        _isMovingToAttackPosition = true;
        Vector3 targetPosition = targetMono.transform.position;

        if (_agent != null)
        {
            _agent.stoppingDistance = _enemyStats.AttackRange * 0.9f;
            _agent.SetDestination(targetPosition);
            _lastTargetPosition = targetPosition;
        }
    }
    /// <summary>
    /// Düşmanın hareket durumunu günceller.
    /// Hedef takibi ve pozisyon güncellemesini yönetir.
    /// </summary>    
    private void UpdateMovement()
    {
        if (_currentTarget != null)
        {
            var targetMono = _currentTarget as MonoBehaviour;
            if (targetMono == null || targetMono.gameObject == null)
            {
                ReturnToMainTarget();
                return;
            }

            float distanceToTarget = Vector3.Distance(transform.position, targetMono.transform.position);

            if (distanceToTarget <= _enemyStats.AttackRange)
            {
                StopMoving();
                _isMovingToAttackPosition = false;
                _isAttacking = true;
            }
            else if (_isMovingToAttackPosition)
            {
                // Tower pozisyonunu güncelle
                if (Vector3.Distance(targetMono.transform.position, _lastTargetPosition) > 0.1f)
                {
                    _agent.SetDestination(targetMono.transform.position);
                    _lastTargetPosition = targetMono.transform.position;
                }
            }
        }
        else if (!_isAttacking && _mainTarget != null)
        {
            // Ana hedefe ilerleme kontrolü
            float distanceToMain = Vector3.Distance(transform.position, _mainTarget.position);
            if (distanceToMain <= _enemyStats.ArrivalDistance)
            {
                OnDestinationReached?.Invoke(CurrentPosition);
            }
        }
    }
    /// <summary>
    /// Saldırı durumunu ve cooldown süresini yönetir.
    /// </summary>    
    private void HandleCombat()
    {
        if (!_isAttacking || _currentTarget == null) return;

        // Tower'ı kontrol et
        var targetMono = _currentTarget as MonoBehaviour;
        if (targetMono == null || targetMono.gameObject == null || _currentTarget.IsDead)
        {
            // Tower yok olmuşsa ana hedefe dön
            ReturnToMainTarget();
            return;
        }

        float distanceToTarget = Vector3.Distance(transform.position, targetMono.transform.position);

        // Eğer menzil dışına çıktıysa tekrar yaklaş
        if (distanceToTarget > _enemyStats.AttackRange)
        {
            EngageTower(_currentTarget);
            return;
        }

        // Saldırı yap
        if (_attackCooldown <= 0)
        {
            PerformAttack();
            _attackCooldown = _enemyStats.AttackSpeed;
        }
        else
        {
            _attackCooldown -= Time.deltaTime;
        }
    }


    private void PerformAttack()
    {
        if (_currentTarget != null && !_currentTarget.IsDead)
        {
            _currentTarget.TakeDamage(_enemyStats.Damage);
        }
    }

    public void ReturnToMainTarget()
    {
        _currentTarget = null;
        _isAttacking = false;
        _isMovingToAttackPosition = false;

        if (_mainTarget != null && _agent != null)
        {
            _agent.stoppingDistance = _enemyStats.ArrivalDistance;
            _agent.SetDestination(_mainTarget.position);
            StartMoving();
        }
    }

    public void SetTarget(Transform target)
    {
        _mainTarget = target;
        if (_mainTarget != null && _agent != null && _isActive)
        {
            _agent.SetDestination(_mainTarget.position);
        }
    }

    public void SetStats(IEnemyStats stats)
    {
        _enemyStats = stats;
    }

    public void StartMoving()
    {
        if (_agent != null)
        {
            _agent.isStopped = false;
        }
    }

    public void StopMoving()
    {
        if (_agent != null)
        {
            _agent.velocity = Vector3.zero;
            _agent.isStopped = true;
        }
    }

    public void TakeDamage(float damage)
    {
        if (!_isActive || IsDead) return;

        _currentHealth -= (int)damage;
        OnDamageTaken?.Invoke(damage);

        if (IsDead)
        {
            Die();
        }
    }

    private void Die()
    {
        _isActive = false;
        OnDeath?.Invoke(this);
        Destroy(gameObject);
    }

    public void Deactivate()
    {
        _isActive = false;
        if (_agent != null)
        {
            _agent.isStopped = true;
        }
    }
    public void MoveTo(Vector3 destination)
    {
        throw new NotImplementedException();
    }
}