/// <summary>
/// Saldırı yapabilen varlıklar için arayüz.
/// </summary>
public interface IAttacker
{
    /// <summary>
    /// Varlığın verdiği hasar miktarı.
    /// </summary>
    float Damage { get; }

    /// <summary>
    /// Saldırı menzili. Bu menzil içindeki hedeflere saldırabilir.
    /// </summary>
    float AttackRange { get; }

    /// <summary>
    /// Saldırı hızı (saniye cinsinden). İki saldırı arasındaki bekleme süresi.
    /// </summary>
    float AttackRate { get; }

    /// <summary>
    /// Varlığın şu anda saldırı yapıp yapamayacağını belirtir.
    /// Cooldown süresi ve diğer faktörlere bağlıdır.
    /// </summary>
    bool CanAttack { get; }

    /// <summary>
    /// Belirtilen hedefe saldırı gerçekleştirir.
    /// </summary>
    /// <param name="target">Saldırı yapılacak hedef</param>
    void Attack(IDamageable target);

    /// <summary>
    /// Saldırı gerçekleştiğinde tetiklenen event.
    /// Verilen hasar ve hedef bilgisini içerir.
    /// </summary>
    event System.Action<IDamageable, float> OnAttackPerformed;
}