using UnityEngine;

public class LaserBase : WeaponBase {
    [SerializeField] private LaserWeapon laserWeapon;
    public LaserData LaserTemplate => laserWeapon != null ? laserWeapon.TemplateData : null;

    public override void Initialize(WeaponRuntimeData data, bool isManual)
    {
        base.Initialize(data, isManual);
        laserWeapon.Initialize(data.Laser);
    }

    public override void Fire()
    {
    }

    public override void StartFire()
    {
        EnsureRuntimeData();
        laserWeapon.StartLaser();
    }

    public override void StopFire()
    {
        laserWeapon.StopLaser();
    }
}
