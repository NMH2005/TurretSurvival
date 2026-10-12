using UnityEngine;

public class LaserBase : WeaponBase {
    [SerializeField] private LaserWeapon laserWeapon;
    public LaserData LaserTemplate => laserWeapon != null ? laserWeapon.TemplateData : null;
    protected override bool UsesMagazine => false;
    public float BatteryRemaining { get; private set; }
    public float BatteryNormalized => Mathf.Clamp01(BatteryRemaining / Data.batteryDuration);
    public bool IsReloading => ReloadRemaining > 0f;
    public float ReloadRemaining { get; private set; }

    public override void Initialize(WeaponRuntimeData data, bool isManual)
    {
        base.Initialize(data, isManual);
        laserWeapon.Initialize(data.Laser);
        BatteryRemaining = Mathf.Max(0.05f, Data.batteryDuration);
    }

    public override void Fire()
    {
    }

    public override void StartFire()
    {
        if (Time.timeScale <= 0f) return;
        EnsureRuntimeData();
        if (IsManual && (IsReloading || BatteryRemaining <= 0f)) return;
        laserWeapon.StartLaser();
    }

    public override void StopFire()
    {
        if (laserWeapon != null) laserWeapon.StopLaser();
    }

    public override bool TryReload()
    {
        if (!IsManual || Time.timeScale <= 0f) return false;
        EnsureRuntimeData();
        if (IsReloading || BatteryRemaining >= Data.batteryDuration) return false;
        StopFire();
        ReloadRemaining = Mathf.Max(0.05f, Data.reloadDuration);
        return true;
    }

    protected override void Update()
    {
        if (Time.timeScale <= 0f || Time.deltaTime <= 0f) return;
        if (IsManual)
        {
            if (IsReloading)
            {
                ReloadRemaining = Mathf.Max(0f, ReloadRemaining - Time.deltaTime);
                if (!IsReloading) BatteryRemaining = Mathf.Max(0.05f, Data.batteryDuration);
            }
            else if (laserWeapon.IsFiring)
            {
                BatteryRemaining = Mathf.Max(0f, BatteryRemaining - Time.deltaTime);
                if (BatteryRemaining <= 0f) TryReload();
            }
        }
        base.Update();
    }
}
