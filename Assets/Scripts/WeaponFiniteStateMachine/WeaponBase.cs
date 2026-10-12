using UnityEngine;

public class WeaponBase : MonoBehaviour {

    [SerializeField] private WeaponData weaponData;
    [SerializeField] private Transform firePoint;
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

    public Transform Target { get; set; }

    public bool IsManual { get; private set; } = true;
    public void SetControlMode(bool isManual)
    {
        IsManual = isManual;
    }
    public virtual void Initialize(WeaponRuntimeData data, bool isManual)
    {
        if (data == null) throw new System.ArgumentNullException(nameof(data));
        if (RuntimeData != null) throw new System.InvalidOperationException("Weapon is already initialized.");
        RuntimeData = data;
        SetControlMode(isManual);
    }

    protected void EnsureRuntimeData()
    {
        if (RuntimeData == null) Initialize(WeaponRuntimeData.Create(this), IsManual);
    }

    private void Start()
    {
        EnsureRuntimeData();
        pivotHandle = GetComponentInParent<PlayerPivotHandle>();
        InputHandler = GetComponentInParent<PlayerInputHandler>();

        StateMachine = new WeaponStateMachine();
        AimState = new WeaponAimState(this, StateMachine, Data);
        FireState = new WeaponFireState(this, StateMachine, Data);
        SearchState = new WeaponSearchState(this, StateMachine, Data);
        AttackState = new WeaponAttackState(this, StateMachine, Data);

        StateMachine.Initialize(IsManual ? (WeaponState)AimState : SearchState);
    }
    protected virtual void Update()
    {
        StateMachine.CurrentState.LogicUpdate();
    }

    private void FixedUpdate()
    {
        StateMachine.CurrentState.PhysicsUpdate();
    }


    public virtual void Fire()
    {
        EnsureRuntimeData();
        if (firePoint == null || Data.projectilePrefab == null) return;
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
        EnsureRuntimeData();
        if (Time.time >= LastFireTime + Data.fireRate)
        {
            Fire();
        }
    }

    public virtual void StopFire()
    {
    }

    protected virtual void OnDestroy()
    {
        RuntimeData?.Dispose();
    }
}
