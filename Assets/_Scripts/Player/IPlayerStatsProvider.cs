using UnityEngine;

public interface IPlayerStatsProvider
{
    float MoveSpeed { get; }
    float JumpForce { get; }
    float GroundAcceleration { get; }
    float Mass { get; }
    float GravityScale { get; }
    LayerMask GroundLayer { get; }
    bool CanPushObjects { get; }
}