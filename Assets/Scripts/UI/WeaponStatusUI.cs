using TMPro;
using UnityEngine;

public class WeaponStatusUI : MonoBehaviour
{
    [SerializeField] private WeaponSystem weaponSystem;
    [SerializeField] private TMP_Text statusText;

    private void Start()
    {
        if (weaponSystem == null) weaponSystem = FindAnyObjectByType<WeaponSystem>();
    }

    private void Update()
    {
        if (statusText == null || weaponSystem == null) return;
        var weapon = weaponSystem.ManualWeapon;
        if (weapon == null)
        {
            statusText.text = string.Empty;
            return;
        }

        if (weapon is FlamethrowerBase flame)
            statusText.text = $"Heat {Mathf.RoundToInt(flame.HeatNormalized * 100f)}%" + (flame.IsOverheated ? " | Overheated" : "");
        else if (weapon is LaserBase laser)
            statusText.text = laser.IsReloading ? $"Reloading {laser.ReloadRemaining:0.0}s" : $"Battery {Mathf.RoundToInt(laser.BatteryNormalized * 100f)}%";
        else if (weapon.Ammo != null)
            statusText.text = weapon.Ammo.IsReloading ? $"Reloading {weapon.Ammo.ReloadRemaining:0.0}s" : $"Ammo {weapon.Ammo.CurrentAmmo} / {weapon.Ammo.Capacity}";
    }
}
