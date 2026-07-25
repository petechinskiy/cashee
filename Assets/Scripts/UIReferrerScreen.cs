using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReferrersData
{
    public int FirstLevelMembers;
    public int FirstLevelCoins;
    public int SecondLevelMembers;
    public int SecondLevelCoins;

    public int TotalMembers => FirstLevelMembers + SecondLevelMembers;
    public int TotalCoins => FirstLevelCoins + SecondLevelCoins;
}

public class UIReferrerScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _balanceText;
    [SerializeField] private TextMeshProUGUI _totalMembersText;
    [SerializeField] private TextMeshProUGUI _totalCoinsText;
    [SerializeField] private TextMeshProUGUI _firstLevelMembersText;
    [SerializeField] private TextMeshProUGUI _firstLevelCoinsText;
    [SerializeField] private TextMeshProUGUI _secondLevelMembersText;
    [SerializeField] private TextMeshProUGUI _secondLevelCoinsText;
    [SerializeField] private Button _inviteButton;
    [SerializeField] private Button _trasnferButton;
    [SerializeField] private UIInviteFriendScreen _inviteFriendScreen;
    [SerializeField] private GameObject _tutorial;
    [SerializeField] private GameObject _transferInfoWindow;

    private const string TRANSFER_REFERRER_REWARD_URI = "https://casheetrack.com/transfer_referrer_reward.php";

    private ApplicationController _applicationController;

    private void Awake()
    {
        _inviteButton.onClick.AddListener(() =>
        {
            _inviteFriendScreen.gameObject.SetActive(true);
        });

        _trasnferButton.onClick.AddListener(() =>
        {
            if (_applicationController.ReferrersData.TotalCoins > 0)
            {
                var url = $"{TRANSFER_REFERRER_REWARD_URI}?device_id={_applicationController.DeviceId}";

                _applicationController.SendEvent(url, (response) =>
                {
                    bool success = int.Parse(response) == 1;

                    if (success)
                    {
                        Show(_applicationController, new ReferrersData());
                    }
                });
            }
            else
            {
                _transferInfoWindow.SetActive(true);
            }
        });
    }

    private void Update()
    {
        _balanceText.text = _applicationController.BalanceHistoryData.CurrentBalance.ToString();
    }

    private void OnEnable()
    {
        bool showTutorial = !_applicationController.ApplicationState.ReferrerTutorialIsShowed;

        _balanceText.text = _applicationController.BalanceHistoryData.CurrentBalance.ToString();
        _tutorial.SetActive(showTutorial);

        _applicationController.ApplicationState.ReferrerTutorialIsShowed = true;
        _applicationController.SaveData();
    }

    public void Show(ApplicationController applicationController, ReferrersData data)
    {
        _applicationController = applicationController;

        int totalMembers = data.FirstLevelMembers + data.SecondLevelMembers;
        int totalCoins = data.FirstLevelCoins + data.SecondLevelCoins;

        _totalMembersText.text = totalMembers.ToString();
        _totalCoinsText.text = totalCoins.ToString();

        _firstLevelMembersText.text = data.FirstLevelMembers.ToString();
        _firstLevelCoinsText.text = data.FirstLevelCoins.ToString();

        _secondLevelMembersText.text = data.SecondLevelMembers.ToString();
        _secondLevelCoinsText.text = data.SecondLevelCoins.ToString();
    }
}
