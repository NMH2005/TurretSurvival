using System;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSelectUI : MonoBehaviour
{
    [SerializeField] private WeaponData[] availableWeapons;
    [SerializeField] private WeaponChoiceCard[] cards;
    [SerializeField] private GameObject panelRoot;

    private PlayerController player;

    private void Start()
    {
        player = FindAnyObjectByType<PlayerController>();

        for(int i = 0; i<cards.Length; i++)
        {
            int index = i;
            cards[index].Setup(availableWeapons[index], () => SelectWeapon(availableWeapons[index]) );
        }
    }

    private void SelectWeapon(WeaponData weaponData)
    {
        player.Core.WeaponSystem.EquipWeapon(weaponData);
        panelRoot.SetActive(false);
    }
}
