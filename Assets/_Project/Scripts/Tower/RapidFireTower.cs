using System.Collections;
using UnityEngine;

public class RapidFireTower : BaseTower
{
    private RapidFireTowerStats _rapidFireStats;
    private int _remainingBursts;
    private float _burstTimer;

    public override void Initialize()
    {
        base.Initialize();
        _rapidFireStats = _towerStats as RapidFireTowerStats;
        if (_rapidFireStats == null)
        {
            Debug.LogError("RapidFireTower initialized with wrong stats type!");
        }
    }

    public override void Attack(IDamageable target)
    {
        if (!CanAttack || !_isActive || target == null || target.IsDead) return;

        float distance = Vector3.Distance(transform.position, (target as MonoBehaviour).transform.position);
        if (distance <= AttackRange)
        {
            StartBurst(target);
            _attackCooldown = AttackRate;
        }
    }

    private void StartBurst(IDamageable target)
    {
        _remainingBursts = _rapidFireStats.BurstCount;
        StartCoroutine(PerformBurst(target));
    }

    private IEnumerator PerformBurst(IDamageable target)
    {
        while (_remainingBursts > 0 && target != null && !target.IsDead)
        {
            target.TakeDamage(Damage);
            RaiseOnAttackPerformed(target, Damage);
            _remainingBursts--;

            if (_remainingBursts > 0)
            {
                yield return new WaitForSeconds(_rapidFireStats.BurstInterval);
            }
        }
    }
}