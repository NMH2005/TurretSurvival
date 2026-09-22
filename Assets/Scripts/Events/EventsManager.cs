using System;
using UnityEngine;

public static class EventsManager
{
    public static event Action<int, int, int> OnExpChanged;
    public static event Action<int> OnLevelUp;
    public static event Action<int, int> OnHealthChanged;
    public static event Action OnPlayerDeath;
    public static void RaiseExpChanged(int currentExp, int expToNextLevel, int currentLevel)
    {
        OnExpChanged?.Invoke(currentExp, expToNextLevel, currentLevel);
    }

    public static void RaiseLevelUp(int currentLevel)
    {
        OnLevelUp?.Invoke(currentLevel);
    }

    public static void RaiseHealthChanged(int currentHealth, int currentHeart)
    {
        OnHealthChanged?.Invoke(currentHealth, currentHeart);
    }

    public static void RaisePlayerDeath()
    {
        OnPlayerDeath?.Invoke();
    }
}
