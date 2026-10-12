using System;

public sealed class WeaponAmmo
{
    public int Capacity { get; }
    public int CurrentAmmo { get; private set; }
    public bool IsReloading => ReloadRemaining > 0f;
    public float ReloadRemaining { get; private set; }
    public float ReloadProgress => IsReloading ? 1f - ReloadRemaining / activeReloadDuration : 1f;
    private float activeReloadDuration;
    private float reloadDuration;

    public float ReloadDuration
    {
        get => reloadDuration;
        set
        {
            if (value <= 0f || float.IsNaN(value) || float.IsInfinity(value))
                throw new ArgumentOutOfRangeException(nameof(value), "Reload duration must be finite and positive.");
            reloadDuration = value;
        }
    }

    public WeaponAmmo(int capacity, float reloadDuration)
    {
        if (capacity <= 0) throw new ArgumentOutOfRangeException(nameof(capacity));
        Capacity = capacity;
        CurrentAmmo = capacity;
        ReloadDuration = reloadDuration;
    }

    public bool TryConsumeShot()
    {
        if (IsReloading || CurrentAmmo <= 0) return false;
        CurrentAmmo--;
        return true;
    }

    public bool TryReload()
    {
        if (IsReloading || CurrentAmmo == Capacity) return false;
        activeReloadDuration = ReloadDuration;
        ReloadRemaining = activeReloadDuration;
        return true;
    }

    public void Tick(float deltaTime)
    {
        if (deltaTime < 0f || float.IsNaN(deltaTime) || float.IsInfinity(deltaTime))
            throw new ArgumentOutOfRangeException(nameof(deltaTime));
        if (!IsReloading || deltaTime == 0f) return;
        ReloadRemaining = Math.Max(0f, ReloadRemaining - deltaTime);
        if (!IsReloading) CurrentAmmo = Capacity;
    }
}
