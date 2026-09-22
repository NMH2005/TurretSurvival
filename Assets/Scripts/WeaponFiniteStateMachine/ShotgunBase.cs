using UnityEngine;

public class ShotgunBase : WeaponBase
{
    [SerializeField] private ShotgunData shotgunData;

    public override void Fire()
    {
        if (FirePoint == null && shotgunData == null) return;

        float startAngle = -shotgunData.spreadAngle / 2f;
        float step = shotgunData.pelletCount > 1 ? shotgunData.spreadAngle / (shotgunData.pelletCount - 1) : 0f;

        for (int i = 0; i < shotgunData.pelletCount; i++)
        {
            float angle = startAngle + step * i;
            Quaternion rot = FirePoint.rotation * Quaternion.Euler(0,0,angle);
            Instantiate(shotgunData.projectilePrefab, FirePoint.position, rot);
        }

        LastFireTime = Time.time;
    }
}
