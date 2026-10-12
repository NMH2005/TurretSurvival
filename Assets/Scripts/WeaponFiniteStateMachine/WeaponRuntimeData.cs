using System;
using UnityEngine;
using Object = UnityEngine.Object;

public sealed class WeaponRuntimeData : IDisposable
{
    public WeaponData Weapon { get; private set; }
    public ProjectileData Projectile { get; private set; }
    public ShotgunData Shotgun { get; private set; }
    public LaserData Laser { get; private set; }
    public FlamethrowerData Flame { get; private set; }
    public ChainLightningData Chain { get; private set; }
    private bool disposed;

    private WeaponRuntimeData() { }

    public static WeaponRuntimeData Create(WeaponBase source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (source.TemplateData == null) throw new ArgumentException("Weapon has no data definition.", nameof(source));

        var runtime = new WeaponRuntimeData();
        try
        {
            runtime.Weapon = Clone(source.TemplateData);
            if (source is ShotgunBase shotgun) runtime.Shotgun = Clone(shotgun.ShotgunTemplate);
            if (source is LaserBase laser) runtime.Laser = Clone(laser.LaserTemplate);
            if (source is FlamethrowerBase flame) runtime.Flame = Clone(flame.FlameTemplate);

            var projectile = runtime.Shotgun != null ? runtime.Shotgun.projectilePrefab : runtime.Weapon.projectilePrefab;
            if (projectile != null)
            {
                runtime.Projectile = Clone(projectile.TemplateData);
                if (projectile is ChainLightningProjectile chain) runtime.Chain = Clone(chain.ChainTemplate);
            }
            return runtime;
        }
        catch
        {
            runtime.Dispose();
            throw;
        }
    }

    private static T Clone<T>(T template) where T : ScriptableObject
    {
        if (template == null) return null;
        var clone = Object.Instantiate(template);
        clone.hideFlags = HideFlags.DontSave;
        return clone;
    }

    public void Dispose()
    {
        if (disposed) return;
        disposed = true;
        Release(Weapon);
        Release(Projectile);
        Release(Shotgun);
        Release(Laser);
        Release(Flame);
        Release(Chain);
    }

    private static void Release(Object data)
    {
        if (data == null) return;
        if (Application.isPlaying) Object.Destroy(data);
        else Object.DestroyImmediate(data);
    }
}
