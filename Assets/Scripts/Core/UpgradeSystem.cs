using System.Collections.Generic;
using UnityEngine;

public class UpgradeSystem : MonoBehaviour {
    [SerializeField] private WeaponSystem weaponSystem;
    [SerializeField] private Health health;
    [SerializeField] private LevelUpUI levelUpUI;

    [SerializeField]
    private UpgradeData[] availableUpgrades =
        System.Array.Empty<UpgradeData>();

    [SerializeField] private UpgradeData recoveryReward;

    private readonly Dictionary<UpgradeData, int> ranks = new();
    private readonly List<UpgradeData> activeChoices = new();

    private int pendingLevels;
    private bool isChoosing;
    private bool ownsPause;
    private bool playerDead;

    private float previousTimeScale;

    private void Awake()
    {
        if (weaponSystem == null ||
            health == null ||
            levelUpUI == null ||
            recoveryReward == null ||
            recoveryReward.type != UpgradeType.Recovery)
        {
            enabled = false;
        }
    }

    private void OnEnable()
    {
        EventsManager.OnLevelUp += QueueLevelUp;
        EventsManager.OnPlayerDeath += HandlePlayerDeath;
    }

    private void OnDisable()
    {
        EventsManager.OnLevelUp -= QueueLevelUp;
        EventsManager.OnPlayerDeath -= HandlePlayerDeath;

        CancelChoices();
    }

    public void QueueLevelUp(int level)
    {
        if (playerDead)
            return;

        pendingLevels++;
    }

    private void LateUpdate()
    {
        if (playerDead ||
            isChoosing ||
            pendingLevels <= 0 ||
            weaponSystem.ManualWeapon == null)
        {
            return;
        }

        ShowNextChoice();
    }

    private void ShowNextChoice()
    {
        if (!ownsPause)
        {
            previousTimeScale = Time.timeScale;
            ownsPause = true;

            weaponSystem.ManualWeapon.StopFire();
            Time.timeScale = 0f;
        }

        activeChoices.Clear();
        activeChoices.AddRange(GetChoices(3));

        isChoosing = true;
        levelUpUI.Show(activeChoices);
    }

    public IReadOnlyList<UpgradeData> GetChoices(int count)
    {
        var pool = new List<UpgradeData>();
        var result = new List<UpgradeData>();

        if (count <= 0)
            return result;

        foreach (UpgradeData upgrade in availableUpgrades)
        {
            if (upgrade == null ||
                upgrade.type == UpgradeType.Recovery ||
                pool.Contains(upgrade) ||
                !CanApply(upgrade))
            {
                continue;
            }

            pool.Add(upgrade);
        }

        while (pool.Count > 0 && result.Count < count)
        {
            int index = Random.Range(0, pool.Count);

            result.Add(pool[index]);
            pool.RemoveAt(index);
        }

        if (result.Count == 0)
            result.Add(recoveryReward);

        return result;
    }

    private int GetRank(UpgradeData upgrade)
    {
        return ranks.TryGetValue(upgrade, out int rank)
            ? rank
            : 0;
    }

    private bool CanApply(UpgradeData upgrade)
    {
        WeaponBase main = weaponSystem.ManualWeapon;

        if (upgrade == null ||
            main == null ||
            main.RuntimeData == null ||
            health.IsDead)
        {
            return false;
        }

        if (upgrade.type == UpgradeType.Recovery)
            return true;

        if (GetRank(upgrade) >= upgrade.maxRank)
            return false;

        if (upgrade.value <= 0f ||
            float.IsNaN(upgrade.value) ||
            float.IsInfinity(upgrade.value))
        {
            return false;
        }

        switch (upgrade.type)
        {
            case UpgradeType.MainDamage:
                return true;

            case UpgradeType.MainFireInterval:
                return main.RuntimeData.Projectile != null &&
                       main.Data.fireRate > 0.05f &&
                       upgrade.value < 1f;

            default:
                return false;
        }
    }

    public bool TryApply(UpgradeData choice)
    {
        if (playerDead ||
            !isChoosing ||
            pendingLevels <= 0 ||
            !activeChoices.Contains(choice) ||
            !CanApply(choice))
        {
            return false;
        }

        isChoosing = false;
        activeChoices.Clear();

        ApplyReward(choice);

        if (choice.type != UpgradeType.Recovery)
            ranks[choice] = GetRank(choice) + 1;

        pendingLevels--;

        Debug.Assert(pendingLevels >= 0);

        levelUpUI.Hide();

        if (pendingLevels == 0)
            RestoreTimeScale();

        return true;
    }

    private void ApplyReward(UpgradeData choice)
    {
        WeaponBase main = weaponSystem.ManualWeapon;

        switch (choice.type)
        {
            case UpgradeType.MainDamage:
                ScaleDamage(main, 1f + choice.value);
                break;

            case UpgradeType.MainFireInterval:
                main.Data.fireRate = Mathf.Max(
                    0.05f,
                    main.Data.fireRate * (1f - choice.value));
                break;

            case UpgradeType.Recovery:
                if (health.CurrentHealth < health.MaxHealth)
                {
                    int amount = Mathf.Max(
                        1,
                        Mathf.CeilToInt(health.MaxHealth * 0.1f));

                    health.Heal(amount);
                }
                else
                {
                    foreach (WeaponBase weapon
                             in weaponSystem.EquippedWeapon)
                    {
                        if (weapon != null)
                            ScaleDamage(weapon, 1.05f);
                    }
                }

                break;
        }
    }

    private static void ScaleDamage(
        WeaponBase weapon,
        float multiplier)
    {
        WeaponRuntimeData runtime = weapon.RuntimeData;

        if (runtime == null)
            return;

        int Scale(int value)
        {
            return value > 0
                ? Mathf.CeilToInt(value * multiplier)
                : value;
        }

        if (runtime.Projectile != null)
            runtime.Projectile.damage =
                Scale(runtime.Projectile.damage);

        if (runtime.Laser != null)
            runtime.Laser.damagePerTick =
                Scale(runtime.Laser.damagePerTick);

        if (runtime.Flame != null)
        {
            runtime.Flame.damagePerTick =
                Scale(runtime.Flame.damagePerTick);

            runtime.Flame.burnDamagePerTick =
                Scale(runtime.Flame.burnDamagePerTick);
        }

        if (runtime.Chain != null)
            runtime.Chain.damagePerJump =
                Scale(runtime.Chain.damagePerJump);
    }

    private void HandlePlayerDeath()
    {
        playerDead = true;
        CancelChoices();
    }

    private void CancelChoices()
    {
        pendingLevels = 0;
        isChoosing = false;
        activeChoices.Clear();

        if (levelUpUI != null)
            levelUpUI.Hide();

        RestoreTimeScale();
    }

    private void RestoreTimeScale()
    {
        if (!ownsPause)
            return;

        Time.timeScale = previousTimeScale;
        ownsPause = false;
    }
}