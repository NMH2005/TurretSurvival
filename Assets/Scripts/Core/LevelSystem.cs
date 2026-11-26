using UnityEngine;

public class LevelSystem : CoreComponent {
    [SerializeField] private LevelData levelData;

    public int CurrentLevel { get; private set; } = 1;
    public int CurrentExp { get; private set; }
    public int ExpToNextLevel { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        ExpToNextLevel = levelData.GetExpRequired(CurrentLevel);
    }

    private void Start()
    {
        EventsManager.RaiseExpChanged(CurrentExp, ExpToNextLevel, CurrentLevel);
    }

    public void AddExp(int amount)
    {
        if (amount <= 0) return;

        long remainingExp = (long)CurrentExp + amount;
        while (remainingExp >= ExpToNextLevel)
        {
            EventsManager.RaiseExpChanged(ExpToNextLevel, ExpToNextLevel, CurrentLevel);

            remainingExp -= ExpToNextLevel;
            CurrentExp = (int)System.Math.Min(remainingExp, int.MaxValue);
            CurrentLevel++;
            ExpToNextLevel = levelData.GetExpRequired(CurrentLevel);
            EventsManager.RaiseLevelUp(CurrentLevel);
        }

        CurrentExp = (int)remainingExp;
        EventsManager.RaiseExpChanged(CurrentExp, ExpToNextLevel, CurrentLevel);
    }
}
