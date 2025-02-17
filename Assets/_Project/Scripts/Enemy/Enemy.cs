using UnityEngine;
using UnityEngine.AI;
using Zenject;

public class Enemy : MonoBehaviour
{
    private IEnemyStats _enemyStats;
    private NavMeshAgent _agent;
    private Transform _target; 
    private float _attackCooldown;
    private int _currentHealth;

    [Inject]
    public void Construct(IEnemyStats enemyStats)
    {
        Debug.Log("Enemy Constructed");
        _enemyStats = enemyStats;
    }

    void Awake() 
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        Debug.Log("Enemy Started");
        _currentHealth = _enemyStats.Health;
        _agent.speed = _enemyStats.MoveSpeed;
    }

    public void SetTarget(Transform target)
    {
        _target = target;
        if(_agent != null && _target != null)
        {
            _agent.SetDestination(_target.position);
        }
    }

    public void TakeDamage(float damage)
    {
        _currentHealth -= (int)damage;
        if(_currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Örneğin ölme animasyonu, puan ekleme vs. eklenebilir.
        Destroy(gameObject);
    }
    
    void Update()
    {
        // Hedefe ulaştığında veya belirli bir mesafedeyken saldırı mantığı eklenebilir.
        // Şu an sadece hedefe doğru hareket sağlanıyor.
    }
}