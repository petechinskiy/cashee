using DG.Tweening;
using I2.Loc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class DailyStreakData
{
    public List<int> States;
    public int LeftSecondsToEnd;
    public float StreakRevenue;
    public float DailyRevenue;
}

public enum DailyStreakSlotType
{
    None, Current, Completed
}

[Serializable]
public class DailyStreakSlotView
{
    public DailyStreakSlotType SlotType;
    public Sprite Sprite;
}

public class UIDailyStreakPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _daysText;
    [SerializeField] private TextMeshProUGUI _dailyRewardText;
    [SerializeField] private TextMeshProUGUI _dailyRewardDescText;
    [SerializeField] private TextMeshProUGUI _streakRewardText;
    [SerializeField] private TextMeshProUGUI _leftDaysText;
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private GameObject _rewardPanel;
    [SerializeField] private GameObject _endPanel;
    [SerializeField] private Button _telegramButton;
    [SerializeField] private Button _moreOffersButton;
    [SerializeField] private TextMeshProUGUI _maximumStreakText;
    [SerializeField] private DailyStreakSlotView[] _slotViews;
    [SerializeField] private UIDailyStreakSlot[] _slots;

    private Sequence _daySlotsSequence;
    private Sequence _daysStreakSequence;

    private void OnEnable()
    {
        _daySlotsSequence?.Restart();
        _daysStreakSequence?.Restart();
    }

    public void Init(Action onMoreOffersClick)
    {
        _moreOffersButton.onClick.AddListener(() => onMoreOffersClick.Invoke());
    }

    private void Awake()
    {
        _telegramButton.onClick.AddListener(() => Application.OpenURL("https://t.me/casheesupport"));
    }

    public void UpdateView(DailyStreakData data)
    {
        var defaultSlot = _slotViews.FirstOrDefault();
        var daysStreackLocalization = LocalizationManager.GetTranslation("DailyStreak/CurrentStreak");
        var leftDaysLocalization = LocalizationManager.GetTranslation("DailyStreak/LeftDays");
        var maximumStreakLocalization = LocalizationManager.GetTranslation("DailyStreak/MaximumStreak");
        int daysRewarded = data.States.Count(e => e > 0);
        int daysDone = data.States.Count(e => e != 0);
        int leftDays = 7 - daysDone;
        bool currentDayDetected = false;
        bool allDone = leftDays == 0;

        _dailyRewardText.text = "+ $" + data.DailyRevenue.ToString("0.##", CultureInfo.InvariantCulture);
        _streakRewardText.text = "+ $" + data.StreakRevenue.ToString("0.##", CultureInfo.InvariantCulture);
        _leftDaysText.text = string.Format(leftDaysLocalization, leftDays);
        _maximumStreakText.text = string.Format(maximumStreakLocalization, daysRewarded);

        _rewardPanel.SetActive(!allDone);
        _endPanel.SetActive(allDone);

        _daySlotsSequence = DOTween.Sequence().Pause();
        _daysStreakSequence = DOTween.Sequence().Pause();

        _daysStreakSequence
            .AppendInterval(1f)
            .Append(_daysText.transform.DOPunchScale(Vector3.one * 1.05f, 0.5f, 0))
            .Append(DOTween.To((x) =>
            {
                _daysText.text = string.Format(daysStreackLocalization, (int)x);
            }, 0, daysRewarded, 0.5f));

        foreach (var slot in _slots)
        {
            slot.UpdateView(defaultSlot.Sprite);
        }

        for (int i = 0; i < _slots.Length; i++)
        {
            var slot = _slots[i];
            var slotView = _slotViews.FirstOrDefault();
            var state = data.States[i];

            if (state > 0)
            {
                slotView = _slotViews.FirstOrDefault(e => e.SlotType == DailyStreakSlotType.Completed);
            }
            else if (!currentDayDetected && state == 0)
            {
                currentDayDetected = true;
                slotView = _slotViews.FirstOrDefault(e => e.SlotType == DailyStreakSlotType.Current);
            }

            if (slotView.SlotType != DailyStreakSlotType.None)
            {
                var slotImage = slot.GetComponentInChildren<Image>();
                var slotColor = slotImage.color;

                slotColor.a = state > 0 ? 1f : 0.3f;
                slotImage.color = slotColor;

                _daySlotsSequence
                        .Append(slot.transform.DOPunchScale(Vector3.one * 1.025f, 0.3f, 0))
                        .PrependInterval(0.15f)
                        .AppendCallback(() =>
                        {
                            slot.UpdateView(slotView.Sprite);
                        });

                if (slotView.SlotType == DailyStreakSlotType.Current)
                {
                    _daySlotsSequence.Append(slotImage.DOFade(1f, 2f));
                }
            }
        }
    }

    public void UpdateTimer(int seconds)
    {
        var text = LocalizationManager.GetTranslation("DailyStreak/Timer") + " ";
        var t = TimeSpan.FromSeconds(seconds);

        if (t.Days > 0)
        {
            text += $"{t.Days}d {t.Hours}h {t.Minutes}m {t.Seconds}s";
        }
        else if (t.Hours > 0)
        {
            text += $"{t.Hours}h {t.Minutes}m {t.Seconds}s";
        }
        else
        {
            text += $"{t.Minutes}m {t.Seconds}s";
        }

        _timerText.text = text;
    }
}
