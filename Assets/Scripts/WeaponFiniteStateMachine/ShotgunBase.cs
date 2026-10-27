using UnityEngine;

public class ShotgunBase : WeaponBase
{
    [SerializeField] private ShotgunData shotgunData;
    public ShotgunData ShotgunTemplate => shotgunData;

    public override void Fire()
    {
        EnsureRuntimeData();
        var data = RuntimeData.Shotgun;
        if (FirePoint == null || data == null || data.projectilePrefab == null || data.pelletCount <= 0) return;
        if (!TryConsumeManualShot()) return;

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

    public void FireRadial(int projectileCount)
    {
        if(IsManual || Time.timeScale <= 0f) return;

        if(projectileCount <= 0) return;

        EnsureRuntimeData();
        
        var shotgun = RuntimeData.Shotgun;
        if(shotgun == null || shotgun.projectilePrefab == null) return;

        float angleStep = 360f / projectileCount;

        for (int i = 0; i < projectileCount; i++)
        {
            float angle = angleStep * i;

            Quaternion rotation = Quaternion.Euler(0f,0f,angle);

            SpawnProjectile(shotgun.projectilePrefab, transform.position, Quaternion.identity);
        }

        LastFireTime = Time.time;
    }
}
