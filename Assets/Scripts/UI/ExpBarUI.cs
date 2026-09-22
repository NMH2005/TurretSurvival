using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExpBarUI : MonoBehaviour
{
    [SerializeField] private Image expFill;
    [SerializeField] private TextMeshProUGUI levelText;

    private LevelSystem levelSystem;

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

    private void HandleExpChanged(int currentExp, int expToNext, int currentLevel)
    {
        expFill.fillAmount =(float) currentExp/expToNext;
        levelText.text = currentLevel.ToString();
    }
}
