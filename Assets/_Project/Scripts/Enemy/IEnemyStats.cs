public interface IEnemyStats 
{
    float Damage { get; }
    int Health { get; }
    float MoveSpeed { get; }
    float AttackSpeed { get; }
    float DetectionRange { get; }  // Düşmanın kuleyi tespit mesafesi
    float AttackRange { get; }     // Düşmanın saldırı mesafesi
    float ArrivalDistance { get; } // Hedef noktaya varış mesafesi
}