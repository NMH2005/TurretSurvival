using UnityEngine;

public class LaserBase : WeaponBase {
    [SerializeField] private LaserWeapon laserWeapon;

    public override void Fire()
    {
    }

    public override void StartFire()
    {
        laserWeapon.StartLaser();
    }

    public override void StopFire()
    {
        laserWeapon.StopLaser();
    }
}
