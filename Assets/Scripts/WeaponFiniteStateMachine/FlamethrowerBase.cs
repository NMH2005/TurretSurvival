using UnityEngine;

public class FlamethrowerBase : WeaponBase {
    [SerializeField] private FlamethrowerWeapon flamethrowerWeapon;
    public FlamethrowerData FlameTemplate => flamethrowerWeapon != null ? flamethrowerWeapon.TemplateData : null;
    protected override bool UsesMagazine => false;
    public float HeatNormalized => flamethrowerWeapon.HeatNormalized;
    public bool IsOverheated => flamethrowerWeapon.IsOverheated;

    public override void Initialize(WeaponRuntimeData data, bool isManual)
    {
        base.Initialize(data, isManual);
        flamethrowerWeapon.Initialize(data.Flame);
        flamethrowerWeapon.SetHeatEnabled(isManual);
    }

    private bool isFlaming;

    public override void Fire()
    {
    }

    public override void StartFire()
    {
        if (Time.timeScale <= 0f) return;
        EnsureRuntimeData();
        if (!isFlaming && flamethrowerWeapon.CanFire)
        {
            isFlaming = true;
            flamethrowerWeapon.StartFlame();
        }
    }

    public override void StopFire()
    {
        if (isFlaming)
        {
            isFlaming = false;
            flamethrowerWeapon.StopFlame();
        }
    }

    protected override void Update()
    {
        base.Update();
        if (isFlaming && flamethrowerWeapon != null && !flamethrowerWeapon.CanFire)
        {
            isFlaming = false;
        }
    }
}
