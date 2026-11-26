using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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

        if (choices.Count > 0 && cards.Length > 0 && EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(cards[0].gameObject);
    }

    public void Hide()
    {
        panelRoot.SetActive(false);
    }
}
