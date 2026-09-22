using System.Collections.Generic;
using UnityEngine;

public class WeaponSystem : CoreComponent
{
    [SerializeField] private Transform weaponPivot;
    [SerializeField] private int maxFreeSlot = 6;    
    private readonly List<WeaponBase> equippedWeapons = new List<WeaponBase>();
  
    public IReadOnlyList<WeaponBase> EquippedWeapon => equippedWeapons;
    public bool HasFreeSlot => equippedWeapons.Count < maxFreeSlot;
    public WeaponBase EquipWeapon(WeaponData weaponData)
    {
        if(!HasFreeSlot) return null;
        GameObject instance = Instantiate(weaponData.weaponPrefab, weaponPivot.position, Quaternion.identity,weaponPivot);
        WeaponBase weapon = instance.GetComponent<WeaponBase>();
        bool isManual = equippedWeapons.Count == 0;
        weapon.SetControlMode(isManual);
        equippedWeapons.Add(weapon);
        return weapon; 
    }

}
