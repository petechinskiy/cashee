using DG.Tweening;
using I2.Loc;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class MissionsData
{
    public List<MissionState> Missions;
}

[Serializable]
public class MissionState
{
    public int Id;
    public float Reward;
    public int Coins;
    public bool Completed;
    public List<MissionProgress> Progresses;
    public bool Notified;
}

[Serializable]
public class MissionProgress
{
    public MissionProgressType Type;
    public int Current;
    public int Total;
}

public enum MissionProgressType
{
    Games, Coins
}

public class UIMissionsWindow : UIWindowBase
{
    [SerializeField] private Button _closeButton;
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _descText;
    [SerializeField] private TabsController _tabsController;
    [SerializeField] private Transform _tabsSelector;
    [SerializeField] private UIMissionsPanel _missionsPanel;
    [SerializeField] private UIDailyStreakPanel _dailyStreakPanel;

    private TabView _currentTab;
    private Action _onDailyStreakOpened;

    protected override void Awake()
    {
        base.Awake();

        _closeButton.onClick.AddListener(() => Hide());

        _tabsController.Init((tab) =>
        {
            SelectTab(tab);

            if (tab.Type == TabType.DailyStreak)
            {
                _onDailyStreakOpened.Invoke();
            }
        });

        _dailyStreakPanel.GetComponent<CanvasGroup>().alpha = 0f;
        _dailyStreakPanel.gameObject.SetActive(false);

        var missionsTab = _tabsController.GetTab(TabType.Missions);

        _dailyStreakPanel.Init(() => SelectTab(missionsTab));
        _currentTab = missionsTab;
    }

    public void Init(Action onDailySteakOpened)
    {
        _onDailyStreakOpened = onDailySteakOpened;
    }

    public void UpdateMissions(MissionsData missions)
    {
        _missionsPanel.UpdateView(missions);
    }

    public void UpdateDailyStreak(DailyStreakData dailyStreak)
    {
        _dailyStreakPanel.UpdateView(dailyStreak);
    }

    private void SelectTab(TabView tab)
    {
        if (_currentTab == tab)
        {
            return;
        }

        bool isMissions = tab.Type == TabType.Missions;
        var panel = tab.Content.transform;
        var oldPanel = _currentTab.Content.transform;
        var pos = oldPanel.position;
        var targetPos = pos;

        pos.x += isMissions ? -0.5f : 0.5f;
        panel.position = pos;
        panel.gameObject.SetActive(true);

        DOTween.Sequence()
            .Append(oldPanel.GetComponent<CanvasGroup>().DOFade(0f, 0.25f))
            .Join(_tabsSelector.DOMove(tab.Toggle.transform.position, 0.5f))
            .Append(panel.GetComponent<CanvasGroup>().DOFade(1f, 0.25f))
            .Join(panel.DOMove(targetPos, 0.25f))
            .OnComplete(() =>
            {
                oldPanel.gameObject.SetActive(false);
                _currentTab = tab;
                
            });

        var titleTerm = isMissions ? "MissionsWindow/Title" : "DailyStreak/Title";
        var descTerm = isMissions ? "MissionsWindow/Desc" : "DailyStreak/Desc1";

        _titleText.text = LocalizationManager.GetTranslation(titleTerm);
        _descText.text = LocalizationManager.GetTranslation(descTerm);
    }

    public void UpdateTimerDailyStreak(int seconds)
    {
        _dailyStreakPanel.UpdateTimer(seconds);
    }
}
