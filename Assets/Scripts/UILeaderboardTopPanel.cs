using I2.Loc;
using System;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;

public class UILeaderboardTopPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _usernameShortText;
    [SerializeField] private TextMeshProUGUI _usernameText;
    [SerializeField] private TextMeshProUGUI _earningText;
    [SerializeField] private TextMeshProUGUI _valueText;

    public void Init(string firstName, string lastName, float revenue)
    {
        var earningLocalization = LocalizationManager.GetTranslation("Leaderboard/Earning");
        var revenueText = revenue > 1f ? ((int)revenue).ToString() : Math.Round(revenue, 2).ToString("0.##", CultureInfo.InvariantCulture);

        _earningText.text = string.Format(earningLocalization, revenueText);
        _usernameText.text = $"{firstName} {lastName}";
        _usernameShortText.text = $"{firstName?.FirstOrDefault()}{lastName?.FirstOrDefault()}";
        _valueText.text = $"${revenueText}";
    }
}
