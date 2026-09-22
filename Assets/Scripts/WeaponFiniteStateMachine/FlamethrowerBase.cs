using UnityEngine;

public class FlamethrowerBase : WeaponBase {
    [SerializeField] private FlamethrowerWeapon flamethrowerWeapon;

    private bool isFlaming;

    public override void Fire()
    {
    }

    public override void StartFire()
    {
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
