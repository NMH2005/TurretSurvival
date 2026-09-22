using UnityEngine;

public class LevelSystem : CoreComponent
{
    [SerializeField] private LevelData levelData;

    public int CurrentLevel { get; private set; }
    public int CurrentExp { get; private set; }
    public int ExpToNextLevel { get; private set;}

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
        CurrentExp += amount;
        while(CurrentExp >= ExpToNextLevel)
        {
            CurrentExp -= ExpToNextLevel;
            CurrentLevel++;
            ExpToNextLevel = levelData.GetExpRequired(CurrentLevel);
            EventsManager.RaiseLevelUp(CurrentLevel);
        }

        EventsManager.RaiseExpChanged(CurrentExp, ExpToNextLevel, CurrentLevel);
    }
}
