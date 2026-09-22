using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponChoiceCard : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Button button;
    public void Setup(WeaponData data, Action onSelected)
    {
        icon.sprite = data.icon;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onSelected());
    }
}
