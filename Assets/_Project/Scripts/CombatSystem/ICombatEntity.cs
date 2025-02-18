/// <summary>
/// Temel savaş varlığı arayüzü. Oyundaki tüm savaşabilen birimler (kuleler ve düşmanlar) 
/// bu arayüzü implement eder.
/// </summary>
public interface ICombatEntity : IDamageable, IMoveable
{
    /// <summary>
    /// Varlığın benzersiz tanımlayıcısı.
    /// </summary>
    string EntityId { get; }

    /// <summary>
    /// Varlığın türü (Kule veya Düşman).
    /// </summary>
    EntityType Type { get; }

    /// <summary>
    /// Varlığın aktif olup olmadığını belirtir.
    /// Deaktif varlıklar Update işlemlerini yapmaz.
    /// </summary>
    bool IsActive { get; }

    /// <summary>
    /// Varlığı başlatır. Stats, health ve diğer başlangıç değerlerini ayarlar.
    /// </summary>
    void Initialize();

    /// <summary>
    /// Varlığı deaktif eder. Ölüm veya hedef noktaya ulaşma durumlarında çağrılır.
    /// </summary>
    void Deactivate();
}