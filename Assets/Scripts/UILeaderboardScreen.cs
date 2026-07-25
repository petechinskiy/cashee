using DG.Tweening;
using I2.Loc;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UILeaderboardScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private List<UILeaderboardSlot> _slots;
    [SerializeField] private GameObject _tableParent;
    [SerializeField] private GameObject _placeholder;
    [SerializeField] private TextMeshProUGUI _placeholderText;
    [SerializeField] private Transform _lockIconTransform;
    [SerializeField] private Button _lockAnimationButton;
    [SerializeField] private UILeaderboardTopPanel[] _topSlots;

    private void Awake()
    {
        _lockAnimationButton.onClick.AddListener(() =>
        {
            DOTween.Sequence()
            //.Append(_lockIconTransform.DOPunchScale(Vector3.one * 1.3f, 1f, 0, 0))
            .Join(_lockIconTransform.DORotate(-Vector3.up * 360f, 1f, RotateMode.FastBeyond360));
        });
    }

    public void Init(LeaderboardData leaderboardData)
    {
        bool showData = leaderboardData != null && leaderboardData.Ranks.Count > 0;

        _placeholder.SetActive(!showData);
        _tableParent.SetActive(showData);

        if (showData)
        {
            leaderboardData.Ranks = leaderboardData.Ranks.OrderBy(e => e.Rank).ToList();
            _slots.ForEach(e => e.gameObject.SetActive(false));

            for (int i = 0; i < leaderboardData.Ranks.Count; i++)
            {
                var view = _slots[i];
                var data = leaderboardData.Ranks[i];
                var flagSprite = ApplicationController.GetCountryFlag(data.CountryCode);

                view.Init(data.Rank, flagSprite, $"{data.FirstName} {data.LastName}", data.Coins, data.IsOwner);

                if (i < 3)
                {
                    var topSlot = _topSlots[i];
                    topSlot.Init(data.FirstName, data.LastName, data.Revenue);
                }
            }
        } else
        {
            var desc = LocalizationManager.GetTranslation("Leaderboard/Desc");
            _placeholderText.text = string.Format(desc, leaderboardData.MinBalance);
        }
    }

    public void UpdateTimer(int seconds)
    {
        var text = LocalizationManager.GetTranslation("Leaderboard/Timer") + " ";
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
