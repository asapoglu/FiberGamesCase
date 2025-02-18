using UnityEngine;

public abstract class BaseTowerStats : ScriptableObject, ITowerStats
{
    [Header("Base Stats")]
    [SerializeField] protected float attackRange = 5f;
    [SerializeField] protected float attackSpeed = 1f;
    [SerializeField] protected float damage = 10f;
    [SerializeField] protected int health = 100;
    [SerializeField] protected TowerType towerType;
    [SerializeField] protected GameObject towerPrefab;

    public virtual float AttackRange => attackRange;
    public virtual float AttackSpeed => attackSpeed;
    public virtual float Damage => damage;
    public virtual int Health => health;
    public TowerType TowerType => towerType;
    public GameObject TowerPrefab => towerPrefab;
}