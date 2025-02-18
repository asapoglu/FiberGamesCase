using UnityEngine;
public interface IMoveable
{
    float MoveSpeed { get; }
    Vector3 CurrentPosition { get; }
    void MoveTo(Vector3 destination);
    bool HasReachedDestination { get; }
    event System.Action<Vector3> OnDestinationReached;
}
