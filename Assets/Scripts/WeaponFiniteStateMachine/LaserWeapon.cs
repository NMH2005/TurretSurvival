using System.Collections.Generic;
using UnityEngine;

public class LaserWeapon : MonoBehaviour
{
    [SerializeField] private LaserData laserData;
    [SerializeField] private Transform firePoint;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private GameObject startVFX;
    [SerializeField] private GameObject endVFX;

    private bool isFiring;
    private float tickTimer;

    private static RaycastHit2D[] hitBuffer = new RaycastHit2D[10];
    private readonly List<ParticleSystem> startParticles = new List<ParticleSystem>();
    private readonly List<ParticleSystem> endParticles = new List<ParticleSystem>();

    private void Awake()
    {
        FillList(startVFX, startParticles);
        FillList(endVFX, endParticles);

        Debug.Log($"startVFX null: {startVFX == null}, startParticles count: {startParticles.Count}");
        Debug.Log($"endVFX null: {endVFX == null}, endParticles count: {endParticles.Count}");
    }

    private void FillList(GameObject vfxRoot, List<ParticleSystem> list)
    {
        if (vfxRoot == null) return;

        for (int i = 0; i < vfxRoot.transform.childCount; i++)
        {
            var ps = vfxRoot.transform.GetChild(i).GetComponent<ParticleSystem>();
            if (ps != null) list.Add(ps);
        }
    }

    public void StartLaser()
    {
        Debug.Log("START LASER");

        isFiring = true;
        tickTimer = 0f;
        lineRenderer.enabled = true;
        PlayAll(startParticles);
        PlayAll(endParticles);
    }

    public void StopLaser()
    {
        isFiring = false;
        lineRenderer.enabled = false;
        StopAll(startParticles);
        StopAll(endParticles);
    }

    private void PlayAll(List<ParticleSystem> list)
    {
        for (int i = 0; i < list.Count; i++) list[i].Play();
    }

    private void StopAll(List<ParticleSystem> list)
    {
        for (int i = 0; i < list.Count; i++) list[i].Stop();
    }

    private void Update()
    {
        if (!isFiring) return;

        Vector2 origin = firePoint.position;
        Vector2 direction = firePoint.up;

        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(laserData.enemyLayer);
        filter.useTriggers = true;

        int count = Physics2D.Raycast(origin, direction, filter, hitBuffer, laserData.range);

        float hitDistance = laserData.range;

        tickTimer -= Time.deltaTime;
        bool shouldTick = tickTimer <= 0f;
        if (shouldTick) tickTimer = laserData.tickRate;

        if (count > 0)
        {
            hitDistance = hitBuffer[0].distance;

            if (shouldTick)
            {
                int hitsToApply = laserData.pierceThrough ? count : 1;
                for (int i = 0; i < hitsToApply; i++)
                {
                    var damageable = hitBuffer[i].collider.GetComponentInChildren<Idamageable>();
                    damageable?.TakeDamage(laserData.damagePerTick);
                }
            }
        }

        Vector2 endPoint = origin + direction * hitDistance;

        lineRenderer.SetPosition(0, origin);
        lineRenderer.SetPosition(1, endPoint);

        if (endVFX != null)
        {
            endVFX.transform.position = endPoint;
        }
    }
}
