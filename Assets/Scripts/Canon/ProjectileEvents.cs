using System;

public static class ProjectileEvents
{
    public static event Action<Projectile> OnProjectileAttached;

    public static void RaiseProjectileAttached(Projectile projectile)
    {
        OnProjectileAttached?.Invoke(projectile);
    }
}
