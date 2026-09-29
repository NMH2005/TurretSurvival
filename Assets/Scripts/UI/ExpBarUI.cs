using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExpBarUI : MonoBehaviour {
    [SerializeField] private Image expFill;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private float fillSpeed = 1.5f;

    private LevelSystem levelSystem;
    private float targetFillAmount;
    private bool isAnimating;
    private int lastLevel = -1;

    private void Start()
    {
        PlayerController player = FindAnyObjectByType<PlayerController>();
        if (player == null) return;
        levelSystem = player.Core.LevelSystem;
        HandleExpChanged(levelSystem.CurrentExp, levelSystem.ExpToNextLevel, levelSystem.CurrentLevel);
    }

    private void OnEnable()
    {
        EventsManager.OnExpChanged += HandleExpChanged;
    }

    private void OnDisable()
    {
        EventsManager.OnExpChanged -= HandleExpChanged;
    }

    private void Update()
    {
        if (!isAnimating) return;

        expFill.fillAmount = Mathf.MoveTowards(
            expFill.fillAmount,
            targetFillAmount,
            fillSpeed * Time.deltaTime
        );

        if (Mathf.Approximately(expFill.fillAmount, targetFillAmount))
        {
            expFill.fillAmount = targetFillAmount;
            isAnimating = false;
        }
    }

    private void HandleExpChanged(int currentExp, int expToNext, int currentLevel)
    {
        float newFillAmount = (float)currentExp / expToNext;
        levelText.text = currentLevel.ToString();

        if (currentLevel != lastLevel)
        {
            expFill.fillAmount = newFillAmount;
            targetFillAmount = newFillAmount;

            isAnimating = false;
            lastLevel = currentLevel;

            return;
        }

        targetFillAmount = newFillAmount;
        isAnimating = true;

        lastLevel = currentLevel;
    }
}
