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

        for (int i = 0; i < cards.Length; i++)
        {
            bool hasWeapon =
                i < availableWeapons.Length &&
                availableWeapons[i] != null;

            cards[i].gameObject.SetActive(hasWeapon);

            if (!hasWeapon)
                continue;

            WeaponData choice = availableWeapons[i];

            cards[i].Setup(
                choice,
                () => SelectWeapon(choice));
        }
    }

    private void SelectWeapon(WeaponData weaponData)
    {
        bool success =
            player.Core.WeaponSystem.TryEquipManual(weaponData);

        if (success)
        {
            player.InputHandler.ClearFireInput();
            panelRoot.SetActive(false);
        }
    }
}
