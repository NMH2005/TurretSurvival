using System.Collections.Generic;
using UnityEngine;

public class LevelUpUI : MonoBehaviour {
    [SerializeField] private UpgradeSystem upgradeSystem;
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private UpgradeChoiceCard[] cards;

    private void Awake()
    {
        panelRoot.SetActive(false);
    }

    public void Show(IReadOnlyList<UpgradeData> choices)
    {
        for (int i = 0; i < cards.Length; i++)
        {
            bool hasChoice = i < choices.Count;

            cards[i].gameObject.SetActive(hasChoice);

            if (!hasChoice)
                continue;

            UpgradeData choice = choices[i];

            cards[i].Setup(choice, () =>
            {
                upgradeSystem.TryApply(choice);
            });
        }

        panelRoot.SetActive(true);
    }

    public void Hide()
    {
        panelRoot.SetActive(false);
    }
}