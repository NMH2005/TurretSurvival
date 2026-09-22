using System;
using System.Collections.Generic;
using UnityEngine;

public class FlamethrowerWeapon : MonoBehaviour {
    [SerializeField] private FlamethrowerData flameData;
    [SerializeField] private ParticleSystem flameParticle;

    private Collider2D flameCollider;
    private bool isFiring;
    private float tickTimer;

    //Scale-based range
    private Vector3 initialScale;
    private float baseRange;
    private float baseParticleSpeed;
    //Overheat
    private float currentHeat = 0f;
    private bool isOverheated = false;
    private float jamTimer = 0f;

    private List<Idamageable> enemiesInRange = new List<Idamageable>();
    private Dictionary<Idamageable, GameObject> enemyGameObjects = new Dictionary<Idamageable, GameObject>();

    public bool CanFire => !isOverheated;
    private void Awake()
    {
        flameCollider = GetComponent<Collider2D>();

        if (flameCollider != null)
        {
            flameCollider.isTrigger = true;
            flameCollider.enabled = false;
        }
        initialScale = transform.localScale;
        baseRange = flameData.flameRange;
        baseParticleSpeed = flameParticle.main.startSpeed.constant;

        UpdateRangeScale();
    }

    private void UpdateRangeScale()
    {
        float multiplier = flameData.flameRange / baseRange;
        transform.localScale = initialScale * multiplier;

        var main = flameParticle.main;
        main.startSpeed = baseParticleSpeed * multiplier;
    }

    public void StartFlame()
    {
        if (isOverheated) return;

        isFiring = true;
        tickTimer = 0f;

        UpdateRangeScale();

        if (flameCollider != null)
            flameCollider.enabled = true;

        if (flameParticle != null && !flameParticle.isPlaying)
            flameParticle.Play();
    }

    public void StopFlame()
    {
        isFiring = false;

        if (flameCollider != null)
            flameCollider.enabled = false;

        if (flameParticle != null && flameParticle.isPlaying)
            flameParticle.Stop();

        foreach (var enemyObj in enemyGameObjects.Values)
        {
            if (enemyObj != null && enemyObj.activeInHierarchy)
            {
                ApplyBurn(enemyObj);
            }
        }

        enemiesInRange.Clear();
        enemyGameObjects.Clear();
    }

    private void Update()
    {
        HandleHeatLogic();
        if (!isFiring) return;

        tickTimer -= Time.deltaTime;
        if (tickTimer <= 0f)
        {
            tickTimer = flameData.tickRate;
            DealDamageToEnemiesInRange();
        }
    }

    private void HandleHeatLogic()
    {
        if (isFiring)
        {
            currentHeat += (Time.deltaTime / flameData.timeToOverheat);
            if (currentHeat >= 1f)
            {
                currentHeat = 1f;
                isOverheated = true;
                jamTimer = flameData.overheatJamDuration;
                StopFlame();
            }
        }
        else
        {
            if (isOverheated)
            {
                if (jamTimer > 0f)
                {
                    jamTimer -= Time.deltaTime;
                }
                else
                {
                    currentHeat -= (Time.deltaTime / flameData.timeToOverheat);
                    if (currentHeat <= 0f)
                    {
                        currentHeat = 0f;
                        isOverheated = false;
                    }
                }
            } else
            {
                if(currentHeat > 0f)
                {
                    currentHeat -= (Time.deltaTime / flameData.timeToOverheat);
                    if (currentHeat < 0f) currentHeat = 0f;
                }

            }

        }
    }

    private void DealDamageToEnemiesInRange()
    {
        for (int i = enemiesInRange.Count - 1; i >= 0; i--)
        {
            var damageable = enemiesInRange[i];

            if (damageable == null || !enemyGameObjects.TryGetValue(damageable, out GameObject enemyObj) || enemyObj == null || !enemyObj.activeInHierarchy)
            {
                enemiesInRange.RemoveAt(i);
                continue;
            }

            damageable.TakeDamage(flameData.damagePerTick);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (((1 << collision.gameObject.layer) & flameData.enemyLayer) == 0) return;

        var damageable = collision.GetComponentInChildren<Idamageable>();
        if (damageable != null && !enemiesInRange.Contains(damageable))
        {
            enemiesInRange.Add(damageable);
            enemyGameObjects[damageable] = collision.gameObject;

            damageable.TakeDamage(flameData.damagePerTick);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var damageable = collision.GetComponentInChildren<Idamageable>();
        if (damageable != null)
        {
            enemiesInRange.Remove(damageable);
            if (enemyGameObjects.TryGetValue(damageable, out GameObject enemyObj))
            {
                if (enemyObj != null && enemyObj.activeInHierarchy)
                {
                    ApplyBurn(enemyObj);
                }
                enemyGameObjects.Remove(damageable);
            }
        }
    }

    private void ApplyBurn(GameObject enemy)
    {
        if (flameData.burnDuration <= 0f) return;

        BurnEffect burn = enemy.GetComponent<BurnEffect>();
        if (burn == null)
        {
            burn = enemy.AddComponent<BurnEffect>();
            burn.Apply(flameData.burnDamagePerTick, flameData.burnTickRate, flameData.burnDuration);
        }
        else
        {
            burn.Refresh(flameData.burnDamagePerTick, flameData.burnTickRate, flameData.burnDuration);
        }
    }
}
