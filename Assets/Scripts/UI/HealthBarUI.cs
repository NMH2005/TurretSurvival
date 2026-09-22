using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Image hpFill;
    [SerializeField] private TextMeshProUGUI heartText;

    private PlayerController player;
    private Health health;
    private void Start()
    {
        player = FindAnyObjectByType<PlayerController>();
        health = player.Core.Health;
        HandleHealthChanged(health.CurrentHealth, health.CurrentHeart);
    }

    private void OnEnable()
    {
        EventsManager.OnHealthChanged += HandleHealthChanged;
    }

    private void OnDisable()
    {
        EventsManager.OnHealthChanged -= HandleHealthChanged;

    }

    private void HandleHealthChanged(int currentHealth, int currentHeart)
    {
        hpFill.fillAmount = (float) currentHealth / health.MaxHealth;
        heartText.text = currentHeart.ToString();
    }
}
