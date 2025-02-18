public interface IDamageable
{
    int CurrentHealth { get; }
    int MaxHealth { get; }
    void TakeDamage(float damage);
    bool IsDead { get; }
    event System.Action<float> OnDamageTaken;
    event System.Action<IDamageable> OnDeath; // OnDeath event'i IDamageable parametresi alacak
}
