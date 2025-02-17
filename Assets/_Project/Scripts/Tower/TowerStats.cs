using UnityEngine;

[CreateAssetMenu(fileName = "TowerStats", menuName = "Tower/Tower Stats", order = 1)]
public class TowerStats : ScriptableObject, ITowerStats {
    [SerializeField] private float attackRange = 5f;
    [SerializeField] private float attackSpeed = 1f; // Saldırılar arasındaki bekleme süresi
    [SerializeField] private float damage = 10f;
    [SerializeField] private int health = 100;

    public float AttackRange => attackRange;
    public float AttackRate => attackSpeed;
    public float Damage => damage;
    public int Health => health;
}