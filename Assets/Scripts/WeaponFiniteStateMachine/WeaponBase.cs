using UnityEngine;

public class WeaponBase : MonoBehaviour {

    [SerializeField] private WeaponData weaponData;
    [SerializeField] private Transform firePoint;
    [SerializeField] private SpriteRenderer[] gunbodyRenderers = System.Array.Empty<SpriteRenderer>();
    public PlayerPivotHandle pivotHandle { get; private set; }
    public PlayerInputHandler InputHandler { get; private set; }
    public WeaponStateMachine StateMachine { get; private set; }
    public WeaponAimState AimState { get; private set; }
    public WeaponFireState FireState { get; private set; }
    public WeaponSearchState SearchState { get; private set; }
    public WeaponAttackState AttackState { get; private set; }
    public float LastFireTime { get; set; }
    public Transform FirePoint => firePoint;
    public WeaponData TemplateData => weaponData;
    public WeaponData Data => RuntimeData != null ? RuntimeData.Weapon : weaponData;
    public WeaponRuntimeData RuntimeData { get; private set; }
    public WeaponAmmo Ammo { get; private set; }
    protected virtual bool UsesMagazine => true;

    public Transform Target { get; set; }

    public bool IsManual { get; private set; } = true;
    public void SetControlMode(bool isManual)
    {
        IsManual = isManual;

        foreach(SpriteRenderer renderer in gunbodyRenderers)
        {
            if(renderer != null)
            {
                renderer.enabled = isManual;
            }
        }
    }
    public virtual void Initialize(WeaponRuntimeData data, bool isManual)
    {
        if (data == null) throw new System.ArgumentNullException(nameof(data));
        if (RuntimeData != null) throw new System.InvalidOperationException("Weapon is already initialized.");
        RuntimeData = data;
        SetControlMode(isManual);
        if (isManual && UsesMagazine)
            Ammo = new WeaponAmmo(Data.magazineCapacity, Data.reloadDuration);
    }

    protected void EnsureRuntimeData()
    {
        if (RuntimeData == null) Initialize(WeaponRuntimeData.Create(this), IsManual);
    }

    private void Start()
    {
        EnsureRuntimeData();

        if (!IsManual) return;

        pivotHandle = GetComponentInParent<PlayerPivotHandle>();
        InputHandler = GetComponentInParent<PlayerInputHandler>();

        StateMachine = new WeaponStateMachine();
        AimState = new WeaponAimState(this, StateMachine, Data);
        FireState = new WeaponFireState(this, StateMachine, Data);

        StateMachine.Initialize(AimState);
    }
    protected virtual void Update()
    {
        if (Time.timeScale <= 0f || Time.deltaTime <= 0f) return;

        if (!IsManual) return;

        Ammo?.Tick(Time.deltaTime);
        StateMachine.CurrentState.LogicUpdate();
    }

    private void FixedUpdate()
    {
        if(!IsManual) return;

        if (Time.timeScale <= 0f || Time.deltaTime <= 0f) return;

        StateMachine.CurrentState.PhysicsUpdate();
    }


    public virtual void Fire()
    {
        EnsureRuntimeData();
        if (firePoint == null || Data.projectilePrefab == null) return;
        if (!TryConsumeManualShot()) return;
        SpawnProjectile(Data.projectilePrefab, firePoint.position, firePoint.rotation);
        LastFireTime = Time.time;
    }

    protected void SpawnProjectile(Projectile prefab, Vector3 position, Quaternion rotation)
    {
        var projectile = Instantiate(prefab, position, rotation);
        if (projectile is ChainLightningProjectile chain)
            chain.Initialize(RuntimeData.Projectile, RuntimeData.Chain);
        else
            projectile.Initialize(RuntimeData.Projectile);
    }

    public virtual void StartFire()
    {
        if (Time.timeScale <= 0f) return;
        EnsureRuntimeData();
        if (Time.time >= LastFireTime + Data.fireRate)
        {
            Fire();
        }
    }

    public virtual void StopFire()
    {
    }

    protected bool TryConsumeManualShot()
    {
        if (Time.timeScale <= 0f) return false;
        if (!IsManual || Ammo == null) return true;
        Ammo.ReloadDuration = Data.reloadDuration;
        if (!Ammo.TryConsumeShot())
        {
            if (Ammo.CurrentAmmo == 0) Ammo.TryReload();
            return false;
        }
        if (Ammo.CurrentAmmo == 0) Ammo.TryReload();
        return true;
    }

    public virtual bool TryReload()
    {
        if (!IsManual || !UsesMagazine || Time.timeScale <= 0f) return false;
        EnsureRuntimeData();
        Ammo.ReloadDuration = Data.reloadDuration;
        return Ammo.TryReload();
    }

    protected virtual void OnDisable()
    {
        StopFire();
    }

    protected virtual void OnDestroy()
    {
        RuntimeData?.Dispose();
    }
}
