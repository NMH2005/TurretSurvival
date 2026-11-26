using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeChoiceCard : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text categoryText;
    [SerializeField] private Button button;

    public void Setup(UpgradeData data, Action onSelected)
    {
        titleText.text = data.displayName;
        descriptionText.text = data.description;
        if (categoryText != null)
        {
            categoryText.text = data.type switch
            {
                UpgradeType.MainDamage => "DAMAGE",
                UpgradeType.MainFireInterval => "FIRE RATE",
                UpgradeType.Recovery => "RECOVERY",
                UpgradeType.AcquireAutomatic => "AUTO WEAPON",
                UpgradeType.AutomaticDamage => "AUTO DAMAGE",
                UpgradeType.AutomaticCooldown => "AUTO COOLDOWN",
                _ => "REWARD"
            };
        }

        icon.sprite = data.icon;
        icon.enabled = data.icon != null;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onSelected?.Invoke());
    }
}
