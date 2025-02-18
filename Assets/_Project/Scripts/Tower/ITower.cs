using UnityEngine;

public interface ITower : ICombatEntity, IAttacker 
{
    Transform TurretHead { get; }
    Transform Transform { get; }

    float TurretRotationSpeed { get; }
    void SetStats(ITowerStats stats);
    void SetGridCell(GridCell cell);
}
