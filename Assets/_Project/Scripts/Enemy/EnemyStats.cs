using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStats", menuName = "Enemy/Enemy Stats", order = 1)]
public class EnemyStats : ScriptableObject, IEnemyStats {
    [SerializeField] private float damage = 5f;
    [SerializeField] private int health = 50;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float attackSpeed = 1.5f;

    public float Damage => damage;
    public int Health => health;
    public float MoveSpeed => moveSpeed;
    public float AttackSpeed => attackSpeed;
}