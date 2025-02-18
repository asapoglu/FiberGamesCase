using UnityEngine;

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