using UnityEngine;

public class ShotgunBase : WeaponBase
{
    [SerializeField] private ShotgunData shotgunData;
    public ShotgunData ShotgunTemplate => shotgunData;

    public override void Fire()
    {
        EnsureRuntimeData();
        var data = RuntimeData.Shotgun;
        if (FirePoint == null || data == null || data.projectilePrefab == null) return;

        float startAngle = -data.spreadAngle / 2f;
        float step = data.pelletCount > 1 ? data.spreadAngle / (data.pelletCount - 1) : 0f;

        for (int i = 0; i < data.pelletCount; i++)
        {
            float angle = startAngle + step * i;
            Quaternion rot = FirePoint.rotation * Quaternion.Euler(0,0,angle);
            SpawnProjectile(data.projectilePrefab, FirePoint.position, rot);
        }

        LastFireTime = Time.time;
    }
}
