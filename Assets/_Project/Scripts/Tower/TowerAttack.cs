using System;
using UnityEngine;
using Zenject;

public class TowerAttack : MonoBehaviour
{
    private enum TowerState { Search, Attack };
    private ITowerStats _towerStats;
    private Enemy _currentEnemy;
    private float _attackCooldown = 0f;
    private TowerState _state = TowerState.Search;

    [SerializeField] private LayerMask enemyLayerMask; // Düşmanların bulunduğu layer
    [SerializeField] private Transform TurretHead; // Kule başlığı


    [Inject]
    public void Construct(ITowerStats towerStats)
    {
        _towerStats = towerStats;
    }

    void Update()
    {
        switch (_state)
        {
            case TowerState.Search:
                {
                    SearchForEnemies();
                    break;
                }
            case TowerState.Attack:
                {
                    RotateTurretHead();
                    _attackCooldown -= Time.deltaTime;
                    if (_attackCooldown <= 0f)
                    {
                        Attack();
                    }
                    break;
                }
        }
    }

    private void Attack()
    {
        float distance = Vector3.Distance(transform.position, _currentEnemy.transform.position);
        if (distance! > _towerStats.AttackRange)
        {
            Enemy enemy = _currentEnemy.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(_towerStats.Damage);
                _attackCooldown = _towerStats.AttackRate;
            }
        }
        {
            _state = TowerState.Search;
            return;
        }
    }


    void SearchForEnemies()
    {
        // Belirlenen menzil içinde düşmanları arar.
        Collider[] hits = Physics.OverlapSphere(transform.position, _towerStats.AttackRange, enemyLayerMask);
        if (hits.Length > 0)
        {
            _currentEnemy = hits[0].transform.GetComponent<Enemy>();
            _state = TowerState.Attack;
        }
    }



    void RotateTurretHead()
    {
        Vector3 direction = _currentEnemy.transform.position - TurretHead.position;

        TurretHead.forward = direction.normalized;
        // Quaternion lookRotation = Quaternion.LookRotation(direction);
        // Vector3 rotation = Quaternion.Lerp(TurretHead.localRotation, lookRotation, Time.deltaTime * 10f).eulerAngles;
        // TurretHead.localRotation = Quaternion.Euler(0f, rotation.y, 0f);
    }

}