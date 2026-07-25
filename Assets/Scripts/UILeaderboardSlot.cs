using I2.Loc;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UILeaderboardSlot : MonoBehaviour
{
    [SerializeField] private bool _isTopRank;
    [SerializeField] private Image _rankImage;
    [SerializeField] private Sprite _defaultRankSprite;
    [SerializeField] private Sprite _userRankSprite;
    [SerializeField] private TextMeshProUGUI _rankText;
    [SerializeField] private Color _defaultRankColor;
    [SerializeField] private Color _userRankColor;
    [SerializeField] private Image _flagImage;
    [SerializeField] private TextMeshProUGUI _usernameText;
    [SerializeField] private TextMeshProUGUI _coinsText;

    public void Init(int rank, Sprite flag, string username, int coins, bool isOwner)
    {
        if (!_isTopRank)
        {
            _rankImage.sprite = isOwner ? _userRankSprite : _defaultRankSprite;
            _rankText.color = isOwner ? _userRankColor : _defaultRankColor;
            _rankText.text = rank.ToString();
        }

        if (isOwner)
        {
            var ownetText = LocalizationManager.GetTranslation("Leaderboard/You");

            username = $"{username} [{ownetText}]";
        }

        _flagImage.sprite = flag;
        _coinsText.text = coins.ToString();
        _usernameText.text = username;

        gameObject.SetActive(true);
    }
}
