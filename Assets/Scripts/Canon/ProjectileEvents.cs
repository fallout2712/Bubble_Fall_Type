using System;

public static class ProjectileEvents
{
    public static event Action<ProjectileBehavior> OnBallProjectileAttached;

    public static void RaiseBallProjectileAttached(ProjectileBehavior ballProjectile)
    {
        OnBallProjectileAttached?.Invoke(ballProjectile);
    }
}
