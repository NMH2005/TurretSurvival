using System.Collections.Generic;
using UnityEngine;

public class WeaponSystem : CoreComponent
{
    [SerializeField] private Transform weaponPivot;
    [SerializeField] private Transform automaticWeaponRoot;
    [SerializeField] private int maxAutomaticSlots = 3;

    private readonly List<WeaponBase> equippedWeapons = new();
    private readonly List<WeaponBase> automaticWeapons = new();

    public IReadOnlyList<WeaponBase> EquippedWeapon => equippedWeapons;
    public IReadOnlyList<WeaponBase> AutomaticWeapons => automaticWeapons;

    public WeaponBase ManualWeapon => equippedWeapons.Find(weapon => weapon != null && weapon.IsManual);
    public bool HasFreeSlot => automaticWeapons.Count < maxAutomaticSlots;

    public bool TryEquipManual(WeaponData data)
    {
        if (ManualWeapon != null) return false;

        WeaponBase weapon = CreateWeapon(data, weaponPivot, true);

        if(weapon == null) return false;

        equippedWeapons.Add(weapon);

        return true;
        
    }

    public bool TryEquipAutomatic(WeaponData data)
    {
        if(data == null) return false;

        if(data.automaticAttackPattern == AutomaticAttackPattern.None) return false;

        if(automaticWeapons.Count >= maxAutomaticSlots) return false;

        WeaponBase weapon = CreateWeapon(data, automaticWeaponRoot, false);
        if(weapon == null) return false;

        AutomaticAttackController controller = weapon.GetComponent<AutomaticAttackController>();

        if (controller == null)
        {
            controller =
                weapon.gameObject.AddComponent<AutomaticAttackController>();
        }

        controller.Initialize(weapon);

        automaticWeapons.Add(weapon);
        equippedWeapons .Add(weapon);

        return true;
    }

    private WeaponBase CreateWeapon(WeaponData data, Transform parent, bool isManual)
    {
        if (data == null || data.weaponPrefab == null || parent == null)
            return null;

        GameObject instance = Instantiate(data.weaponPrefab, parent, false);
        instance.transform.localPosition = Vector3.zero;
        instance.transform.localRotation = Quaternion.identity;

        WeaponBase weapon = instance.GetComponent<WeaponBase>();

        weapon.Initialize(WeaponRuntimeData.Create(weapon), isManual);

        return weapon;
    }

    public WeaponBase EquipWeapon(WeaponData data)
    {
         bool success = ManualWeapon == null ? TryEquipManual(data) : TryEquipAutomatic(data);

        if(!success) return null;

        return equippedWeapons[equippedWeapons.Count - 1];
    }
}
