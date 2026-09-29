using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Image hpFill;
    [SerializeField] private TextMeshProUGUI heartText;
    [SerializeField] private float fillSpeed = 2f;

    private PlayerController player;
    private Health health;
    private float targetFillAmount;
    private bool isAnimating;

    private void Start()
    {
        player = FindAnyObjectByType<PlayerController>();
        health = player.Core.Health;
        targetFillAmount = (float)health.CurrentHealth / health.MaxHealth;

        hpFill.fillAmount = targetFillAmount;
        heartText.text = health.CurrentHeart.ToString();
    }

    private void OnEnable()
    {
        EventsManager.OnHealthChanged += HandleHealthChanged;
    }

    private void OnDisable()
    {
        EventsManager.OnHealthChanged -= HandleHealthChanged;

    }

    private void Update()
    {
        if (!isAnimating)
            return;

        hpFill.fillAmount = Mathf.MoveTowards(
            hpFill.fillAmount,
            targetFillAmount,
            fillSpeed * Time.deltaTime
        );

        if (Mathf.Approximately(hpFill.fillAmount, targetFillAmount))
        {
            hpFill.fillAmount = targetFillAmount;
            isAnimating = false;
        }
    }

    private void HandleHealthChanged(int currentHealth, int currentHeart)
    {
        targetFillAmount = (float) currentHealth / health.MaxHealth;
        heartText.text = currentHeart.ToString();
        isAnimating = true;
    }
}
