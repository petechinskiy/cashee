using DG.Tweening;
using I2.Loc;
using Newtonsoft.Json.Bson;
using Singular;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TMPro;
using Ugi.PlayInstallReferrerPlugin;
using UI;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum InstallSource
{
    organic,
    ironsource,
    others
}

[Serializable]
public class WithdrawState
{
    public int Status;
    public string Info;
    public bool DirectPaypal;
}

[Serializable]
public class ApplicationState
{
    public bool IsFirstLaunch = true;
    public bool RatingShowed;
    public bool ReferrerTutorialIsShowed;
    public bool MoreGamesWindowShowed;
    public bool MoreGames2WindowShowed;
    public bool MoreGamesPromoWindowShowed;
    public int PrevOfferwallsCoins;
    public bool HasNotifications;
    public List<string> EventsCompleted = new List<string>();
}

[Serializable]
public class RevenueData
{
    public List<int> Revenues;
}

[Serializable]
public class LeaderboardData
{
    public int LeftSecondsToUpdate;
    public int MinBalance;
    public List<LeaderboardRankData> Ranks;
}

[Serializable]
public class LeaderboardRankData
{
    public string FirstName;
    public string LastName;
    public string CountryCode;
    public int Rank;
    public int Coins;
    public float Revenue;
    public bool IsOwner;
}

public class ApplicationController : Singleton<ApplicationController>, SingularDeviceAttributionCallbackHandler, SingularSdidAccessorHandler
{
    public GameObject registerPanel;
    public GameObject navigationBar;
    public GameObject lobbyPanel;
    public GameObject rateInfoPanel;
    public GameObject notEnoughCoinsInfoPanel;
    public InfoPanel withdrawCompleteInfoPanel;
    public UICommonWindow withdrawLinkCompleteWindow;
    public GameObject loadingPanel;
    public TMP_InputField withdrawInputField;
    public Image progressBarFill;
    public Text progressBarText;
    public TextMeshProUGUI moneyText;
    public NavigationButton[] allNavigationButtons;

    [SerializeField] private GameObject _signInPanel;
    [SerializeField] private GameObject _welcomePanel;
    [SerializeField] private GameObject _welcomeBonusWindow;
    [SerializeField] private Button _welcomeBonusButton;

    [SerializeField] private UIPayoutScreen _payoutScreen;
    [SerializeField] private UIBalanceHistoryScreen _balanceHistoryScreen;
    [SerializeField] private GameObject _paypalWithdrawError;
    [SerializeField] private GameObject _blockedFromServerWindow;
    [SerializeField] private UIWithdrawWindow _withdrawWindow;
    [SerializeField] private Button _earnButton;
    [SerializeField] private GameObject _moreGamesWindow;
    [SerializeField] private GameObject _moreGames2Window;
    [SerializeField] private Button _moreGamesButton;
    [SerializeField] private Button _moreGames2Button;
    [SerializeField] private Button _moreGamesPromoButton;
    [SerializeField] private GameObject _moreGamesPromoWindow;
    [SerializeField] private EarnTab _earnWindow;
    [SerializeField] private GameObject _notAvailableWindow;
    [SerializeField] private GameObject _notValidInputWindow;
    [SerializeField] private InfoPanel _accessRestrictedWindow;
    [SerializeField] private InfoPanel _infoWindow;
    [SerializeField] private InfoPanel _linkedDeviceNotValidWindow;
    [SerializeField] private UIAuthWindow _authWindow;
    [SerializeField] private Button _termsButton;
    [SerializeField] private GameObject _updatePanel;
    [SerializeField] private Button _updateButton;
    [SerializeField] private Button _loginButton;
    [SerializeField] private CanvasGroup _payoutNotifyPanel;
    [SerializeField] private Button _payoutNotifyButton;
    [SerializeField] private Text _payoutNotifyText;
    [SerializeField] private GameObject _instBonusNotifyPanel;
    [SerializeField] private Button _getInstBonusButton;
    [SerializeField] private GameObject _earnButtonClickImage;
    [SerializeField] private Button _receiveRewardInfoButton;
    [SerializeField] private InfoPanel _receiveRewardInfoWindow;
    [SerializeField] private RectTransform _navigationButtonsSelector;
    [SerializeField] private Button _supportButton;
    [SerializeField] private UISupportWindow _supportWindow;
    [SerializeField] private UIRegistrationScreen _registrationScreen;
    [SerializeField] private UIGiftsScreen _giftScreen;
    [SerializeField] private TextMeshProUGUI _balanceGiftScreenText;
    [SerializeField] private UIProfileScreen _profileScreen;
    [SerializeField] private Button _giftButton;
    [SerializeField] private Button _cashbackButton;
    [SerializeField] private UICommonWindow _rewardCallbackWindow;
    [SerializeField] private RewardCard cardPrefab;
    [SerializeField] private Transform cardParent;
    [SerializeField] private Transform endPointCard;
    [SerializeField] private bool testCard;
    [SerializeField] private Transform[] _rewardCallbackPanelPoints;
    [SerializeField] private Transform _balanceCoin;
    [SerializeField] private UILeaderboardScreen _leaderboardScreen;
    [SerializeField] private Button _leaderboardButton;
    [SerializeField] private Button _missionsButton;
    [SerializeField] private GameObject _missionAlertIcon;
    [SerializeField] private UIMissionsWindow _missionsWindow;
    [SerializeField] private Button _missionsHintButton;
    [SerializeField] private NavigationButton _payoutButton;
    [SerializeField] private GameObject _firstMissionCompletedPopup;
    [SerializeField] private GameObject _dailyStreakNotificationPopup;
    [SerializeField] private GameObject _payoutSlotLimit;

    private int moneyCount;
    private bool isWithdraw = false;
    private GameObject currentActivePanel;
    private GameObject prevPanel;

    [HideInInspector] public bool canChangePanel = true;

    private int needCoinsToWithdraw;
    private float currencyWithdraw;
    private Sequence _payoutNotifySeq;
    private PlayInstallReferrerDetails _installReferrerDetails;
    private AdjoeController _adjoeController;
    private PrimeOfferwallController _primeController;
    private Coroutine _changePanelsCoroutine;
    private Tween _navigationButtonsSelectorTween;

    private readonly string _getPayoutSlotsUri = "https://casheetrack.com/get_payout_slots_new.php";
    private readonly string _balanceHistoryUri = "https://casheetrack.com/balance_history_new.php";
    private readonly string _withdrawUri = "https://casheetrack.com/withdraw.php";
    private readonly string _settingsUri = "https://casheetrack.com/get_settings.php";
    private readonly string _checkPhoneUri = "https://casheetrack.com/check_phone.php";
    private readonly string _checkDeviceUri = "https://casheetrack.com/check_device.php";
    private readonly string _instagramBonusCallbackUri = "https://casheetrack.com/callback_inst.php";
    private readonly string _firebaseNotificationUri = "https://casheetrack.com/fcm.php";
    private readonly string _checkUserExistUri = "https://casheetrack.com/check_user_exist.php";
    private readonly string _userRegistrationUri = "https://casheetrack.com/user_registration.php";
    private readonly string _deleteAccountUri = "https://casheetrack.com/delete_account.php";
    private readonly string _updateProfileUri = "https://casheetrack.com/update_profile.php";
    private readonly string _adjoeButtonCallbackUri = "https://casheetrack.com/callback_adjoe_button.php";
    private readonly string _earnButtonCallbackUri = "https://casheetrack.com/callback_earn_button.php";
    private readonly string _getGiftSlotsUri = "https://casheetrack.com/get_gift_slots.php";
    private readonly string _getReferrersUri = "https://casheetrack.com/get_referrers.php";
    private readonly string _getRevenueDataUri = "https://casheetrack.com/get_revenue_data.php";
    private readonly string _getLeaderboardUri = "https://casheetrack.com/get_leaderboard.php";
    private readonly string _getWelcomeBonusUri = "https://casheetrack.com/callback_welcome_bonus.php";
    private readonly string _getMissionsDataUri = "https://casheetrack.com/get_missions.php";
    private readonly string _getDailyStreakUri = "https://casheetrack.com/get_daily_streak.php";
    private readonly string _dailyStreakRewardsUri = "https://casheetrack.com/daily_streak_rewards.php";
    private readonly string _checkCountryUri = "http://ip-api.com/json/";

    private const string FIRST_ADJOE_CLICK_TOKEN = "adjoe";
    private const string EARN_MONEY_CLICK_EVENT_TOKEN = "ymvi0x";
    private const string EVENT_400COINS_TOKEN = "400Coins";
    private const string EVENT_500COINS_TOKEN = "500Coins";
    private const string EVENT_1000COINS_TOKEN = "1000Coins";
    private const string EVENT_1500COINS_TOKEN = "1500Coins";
    private const string EVENT_2000COINS_TOKEN = "2000Coins";
    private const string EVENT_4000COINS_TOKEN = "4000Coins";
    private const string EVENT_6000COINS_TOKEN = "6000Coins";
    private const string EVENT_8000COINS_TOKEN = "8000Coins";
    private const string EVENT_10000COINS_TOKEN = "10000Coins";

    private readonly List<string> _targetEvents = new List<string>();
    private readonly string[] _rubCountries = new string[] { "ru", "by", "kz", "uz", "kg", "tj" };

    private UIProfilePanel[] _profilePanels;
    private GiftsData _giftsData;
    private float _leftSecondsToGiftUpdate;
    private float _leftSecondsToLeaderboardUpdate;
    private float _leftSecondsToDailyStreak;
    private bool _dailyStreakReceived;
    private bool _giftRequestIsSended;
    private bool _revenueRequestIsSended;
    private string _refCode;
    private string _attributionInfo;
    private string _networkName;
    private string _campaignName;
    private string _installSite;
    private string _creativeName;
    private static readonly Dictionary<string, Sprite> _countryFlags = new();

    public SettingsData SettingsData { get; private set; } = new SettingsData();
    public ServerLocationData ServerLocationData { get; private set; } = new ServerLocationData();
    public BalanceHistoryData BalanceHistoryData { get; private set; }
    public ApplicationState ApplicationState { get; private set; }
    public ReferrersData ReferrersData { get; private set; }
    public string SingularDeviceId { get; private set; }
    public string DeviceId
    {
        get
        {
#if UNITY_EDITOR || DEBUG_MODE
            return "c526fe1d643d67272b9a33e38e77568a";
#endif
            return SystemInfo.deviceUniqueIdentifier;
        }
    }
    public string GpsAdid { get; private set; }

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            if (BalanceHistoryData != null)
            {
                StartCoroutine(CheckRevenueData());
                StartCoroutine(GetMissions(false));
            }
        }
    }

    protected override void Awake()
    {
        if (ApkValidater.isRooted())
            Application.Quit();

        base.Awake();

        if (PlayerPrefs.HasKey("ApplicationState"))
        {
            var json = PlayerPrefs.GetString("ApplicationState");
            ApplicationState = JsonUtility.FromJson<ApplicationState>(json);
        }
        else
        {
            ApplicationState = new ApplicationState();

            SaveData();
        }

        _adjoeController = FindFirstObjectByType<AdjoeController>();
        _primeController = FindFirstObjectByType<PrimeOfferwallController>();
        _profilePanels = FindObjectsByType<UIProfilePanel>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        moneyCount = PlayerPrefs.GetInt("moneyCount", 0);
        currentActivePanel = registerPanel;

        GpsAdid = GetAndroidAdvertiserId();

        PlayInstallReferrer.GetInstallReferrerInfo((installReferrerDetails) =>
        {
            _installReferrerDetails = installReferrerDetails;

            Debug.Log("Install referrer details received!");

            // check for error
            if (installReferrerDetails.Error != null)
            {
                Debug.LogError("Error occurred!");
                if (installReferrerDetails.Error.Exception != null)
                {
                    Debug.LogError("Exception message: " + installReferrerDetails.Error.Exception.Message);
                }
                Debug.LogError("Response code: " + installReferrerDetails.Error.ResponseCode.ToString());
                return;
            }

            // print install referrer details
            if (installReferrerDetails.InstallReferrer != null)
            {
                Debug.Log("Install referrer: " + installReferrerDetails.InstallReferrer);

                var referrerQuery = WebUtility.UrlDecode(installReferrerDetails.InstallReferrer);
                var queryParams = ParseQueryString(referrerQuery);
                queryParams.TryGetValue("utm_term", out var term);

                _refCode = !string.IsNullOrEmpty(term) && term.Contains("refcode") ? term.Replace("refcode", "") : string.Empty;
            }
        });

        _earnButton.onClick.AddListener(() =>
        {
            // ������� � ������� ������
            var url = $"{_earnButtonCallbackUri}?device_id={DeviceId}";
            StartCoroutine(SendEventToServer_Coroutine(url, null));

            if (!ApplicationState.EventsCompleted.Contains(EARN_MONEY_CLICK_EVENT_TOKEN))
            {
                // TODO
                //AdjustEvent adjustEvent = new AdjustEvent(EARN_MONEY_CLICK_EVENT_TOKEN);
                //Adjust.trackEvent(adjustEvent);

                ApplicationState.EventsCompleted.Add(EARN_MONEY_CLICK_EVENT_TOKEN);
                SaveData();

                _earnButtonClickImage.SetActive(false);
            }

            OnClickEarnButton();
            SettingsData.AdjoeOpened = true;
        });

        _moreGamesButton.onClick.AddListener(() =>
        {
            OnClickMoreGamesButton();
        });

        _moreGames2Button.onClick.AddListener(() =>
        {
            if (SettingsData.MyChipsEnabled)
            {
                MCOfferwallObject.Instance.ShowOfferwall();
            }
        });

        _moreGamesPromoButton.onClick.AddListener(() =>
        {
            if (SettingsData.MyChipsEnabled && SettingsData.MychipsPromo)
            {
                MCOfferwallObject.Instance.ShowOfferwall();
            }
        });

        _termsButton.onClick.AddListener(() => Application.OpenURL("https://plus-games.com/cashee-terms"));
        _updateButton.onClick.AddListener(() => Application.OpenURL("https://play.google.com/store/apps/details?id=com.plusgames.cashee"));

        _loginButton.onClick.AddListener(() =>
        {
            _loginButton.interactable = false;
        });

        _payoutNotifyButton.onClick.AddListener(() =>
        {
            _payoutNotifySeq?.Kill();
            _payoutNotifyButton.interactable = false;

            _payoutNotifyPanel.DOFade(0f, 1f).OnComplete(() =>
            {
                _payoutNotifyPanel.gameObject.SetActive(false);
            });
        });

        _getInstBonusButton.onClick.AddListener(() =>
        {
            StartCoroutine(GetInstagramBonus_Coroutine(() => Application.OpenURL("https://instagram.com/plusgames_st")));
        });

        _receiveRewardInfoButton.onClick.AddListener(() => _receiveRewardInfoWindow.Show("How to receive my reward?\r\nYou will receive an email from �Your Digital Reward� in your mailbox. Simply click on �Claim your reward� at the bottom of the email and enter your account information."));

        _supportButton.onClick.AddListener(() => _supportWindow.gameObject.SetActive(true));

        StartCoroutine(CheckUserExist_Coroutine((isExist) =>
        {
            _registrationScreen.Show((userInfo) =>
            {
                StartCoroutine(UserRegisteration_Coroutine(userInfo, (success) => Login()));
            }, Login, !isExist, string.IsNullOrEmpty(_refCode));
        }));

        InitializeCountryFlags();

        _welcomeBonusButton.onClick.AddListener(() =>
        {
            _welcomeBonusButton.interactable = false;

            SendWelcomeBonusRequest();
        });

        void OpenMissionsWindow()
        {
            _missionsWindow.Show();

            _targetEvents.Add("missions-opened");
            CheckEvents();

            _missionsHintButton.gameObject.SetActive(false);
        }

        _missionsButton.onClick.AddListener(() =>
        {
            OpenMissionsWindow();
        });

        _missionsHintButton.onClick.AddListener(() =>
        {
            OpenMissionsWindow();
        });

        _missionsWindow.Init(() =>
        {
            _targetEvents.Add("daily-streak-opened");
            CheckEvents();
        });
    }

    private void Start()
    {
        if (testCard)
        {
            StartCoroutine(CreateTestCard_Coroutine());
        }

        SingularSDK.SetSingularDeviceAttributionCallbackHandler(this);
        SingularSDK.SetSingularSdidAccessorHandler(this);
    }
    
    private void Update()
    {
        if (_giftsData != null && _giftsData.WasPaid)
        {
            _leftSecondsToGiftUpdate -= Time.unscaledDeltaTime;

            _leftSecondsToGiftUpdate = _leftSecondsToGiftUpdate = Mathf.Max(0, _leftSecondsToGiftUpdate);
            _giftScreen.UpdateTimer(Mathf.RoundToInt(_leftSecondsToGiftUpdate));

            if (!_giftRequestIsSended && _leftSecondsToGiftUpdate <= 0)
            {
                StartCoroutine(GetGiftSlots_Coroutine());
            }
        }

        _leftSecondsToLeaderboardUpdate -= Time.unscaledDeltaTime;
        _leftSecondsToLeaderboardUpdate = Mathf.Max(0, _leftSecondsToLeaderboardUpdate);

        if (_leaderboardScreen.gameObject.activeSelf)
        {
            _leaderboardScreen.UpdateTimer((int)_leftSecondsToLeaderboardUpdate);
        }

        _leftSecondsToDailyStreak -= Time.unscaledDeltaTime;
        _leftSecondsToDailyStreak = Mathf.Max(0, _leftSecondsToDailyStreak);

        if (_missionsWindow.gameObject.activeSelf)
        {
            _missionsWindow.UpdateTimerDailyStreak((int)_leftSecondsToDailyStreak);
        }

        _earnButton.interactable = SettingsData != null && !SettingsData.AccessRestricted;

        moneyText.text = moneyCount.ToString();

        float percent = (float)moneyCount / 1000f;
        progressBarFill.fillAmount = Mathf.Clamp01(percent);
        progressBarText.text = $"{Mathf.Clamp((int)(percent * 100f), 0, 100)}%";

        if (BalanceHistoryData != null)
        {
            foreach (var profile in _profilePanels)
            {
                profile.UpdateView(BalanceHistoryData.CurrentBalance);
            }
        }
    }

    public void SaveData()
    {
        var json = JsonUtility.ToJson(ApplicationState);
        PlayerPrefs.SetString("ApplicationState", json);
    }

    private void Login()
    {
        StartCoroutine(GetSettings_Coroutine((success) =>
        {

        }));

        ShowLobby();
    }

    public void AddMoney(int count)
    {
        moneyCount += count;

        PlayerPrefs.SetInt("moneyCount", moneyCount);
    }

    public void CompleteTutorial()
    {
        _signInPanel.SetActive(false);
        _welcomePanel.SetActive(true);
        loadingPanel.SetActive(true);
        PlayerPrefs.SetInt("tutorial", 1);
        StartCoroutine(ChangePanels(lobbyPanel, 2.5f));
        Invoke(nameof(ActiveNavigationBar), 3f);
    }

    public void NavigationButtonSelect(NavigationButton _btn)
    {
        foreach (var btn in allNavigationButtons)
            btn.UnactiveButton();

        _navigationButtonsSelectorTween?.Kill();

        _navigationButtonsSelectorTween = _navigationButtonsSelector.DOMoveX(_btn.transform.position.x, 0.5f).OnComplete(() =>
        {

        });

        _btn.ActiveButton();
    }

    private void Withdraw(PayoutSlotData slotData)
    {
        if(moneyCount < slotData.coins_amount)
        {
            notEnoughCoinsInfoPanel.SetActive(true);
            return;
        }
        else if (slotData.usage_limited)
        {
            _payoutSlotLimit.SetActive(true);
            return;
        }

        needCoinsToWithdraw = slotData.coins_amount;

        _withdrawWindow.Show(BalanceHistoryData, slotData, (slot) => CompleteWithdraw(slot));
    }
    
    private void CompleteWithdraw(PayoutSlotData payoutSlot)
    {
        if (withdrawInputField.text.Length <= 0 || string.IsNullOrEmpty(withdrawInputField.text) || isWithdraw)
            return;

        if (withdrawInputField.text.ToLowerInvariant() == "paypal")
        {
            _paypalWithdrawError.SetActive(true);
            return;
        }

        if (ServerLocationData.countryCode.ToLowerInvariant() != "ru")
        {
            if (!withdrawInputField.text.Contains("@") || !withdrawInputField.text.Contains("."))
            {
                _notValidInputWindow.SetActive(true);
                return;
            }
        }

        _withdrawWindow.Hide();

        StartCoroutine(Withdraw_Coroutine(payoutSlot, string.Empty));
    }

    private void ActiveNavigationBar()
    {
        navigationBar.SetActive(true);
    }

    public void RateGame()
    {
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.plusgames.cashee");
    }

    public void ChangePanel(GameObject newPanel)
    {
        ChangePanel(newPanel, 0f);
    }

    public void ChangePanel(GameObject newPanel, float timeToShow)
    {
        if (newPanel == currentActivePanel || newPanel == null)
            return;

        if (_changePanelsCoroutine != null)
        {
            StopCoroutine(_changePanelsCoroutine);
        }

        _changePanelsCoroutine = StartCoroutine(ChangePanels(newPanel, timeToShow));
    }

    public void PrevPanel()
    {
        ChangePanel(prevPanel, 0f);
    }

    private void CheckEvents()
    {
        if (BalanceHistoryData == null)
        {
            return;
        }

        if (BalanceHistoryData.OfferwallEarnedCoins >= 400)
        {
            _targetEvents.Add(EVENT_400COINS_TOKEN);
        }
        if (BalanceHistoryData.OfferwallEarnedCoins >= 500)
        {
            _targetEvents.Add(EVENT_500COINS_TOKEN);
        }
        if (BalanceHistoryData.OfferwallEarnedCoins >= 1000)
        {
            _targetEvents.Add(EVENT_1000COINS_TOKEN);
        }
        if (BalanceHistoryData.OfferwallEarnedCoins >= 1500)
        {
            _targetEvents.Add(EVENT_1500COINS_TOKEN);
        }
        if (BalanceHistoryData.OfferwallEarnedCoins >= 2000)
        {
            _targetEvents.Add(EVENT_2000COINS_TOKEN);
        }
        if (BalanceHistoryData.OfferwallEarnedCoins >= 4000)
        {
            _targetEvents.Add(EVENT_4000COINS_TOKEN);
        }
        if (BalanceHistoryData.OfferwallEarnedCoins >= 6000)
        {
            _targetEvents.Add(EVENT_6000COINS_TOKEN);
        }
        if (BalanceHistoryData.OfferwallEarnedCoins >= 8000)
        {
            _targetEvents.Add(EVENT_8000COINS_TOKEN);
        }
        if (BalanceHistoryData.OfferwallEarnedCoins >= 10000)
        {
            _targetEvents.Add(EVENT_10000COINS_TOKEN);
        }

        if (ApplicationState.IsFirstLaunch)
        {
            _targetEvents.Add("first_launch");
        }

        foreach (var eventId in _targetEvents)
        {
            if (!ApplicationState.EventsCompleted.Contains(eventId))
            {
                SingularSDK.Event(eventId);

                ApplicationState.EventsCompleted.Add(eventId);
                SaveData();
            }
        }

        _targetEvents.Clear();
    }

    // �������� ������ � ������ �� ������� � �������� ������ � ������
    private IEnumerator CheckRevenueData()
    {
        if (_revenueRequestIsSended)
        {
            yield break;
        }

        _revenueRequestIsSended = true;

        var url = $"{_getRevenueDataUri}?device_id={DeviceId}";

        while (true)
        {
            SendEvent(url, (response) =>
            {
                if (float.TryParse(response, NumberStyles.Float, CultureInfo.InvariantCulture, out float revenue))
                {
                    revenue = ((int)(revenue * 100)) * 0.01f;

                    if (revenue >= 0.5f)
                    {
                        SingularSDK.Revenue("USD", revenue);
                        SingularSDK.Event("postback", "revenue", revenue);
                        //FirebaseManager.Instance.SendRevenueEvent(revenue);
                    }
                    else
                    {
                        SingularAdData data = new SingularAdData(
                            "Adjoe",
                            "USD",
                            revenue
                        );

                        SingularSDK.AdRevenue(data);
                    }
                }
            });

            yield return new WaitForSecondsRealtime(60f);
        }
    }

    private IEnumerator GetMissions(bool repeat)
    {
        if (BalanceHistoryData == null)
        {
            yield return null;
        }

        if (!SettingsData.CanShowMissions)
        {
            yield break;
        }

        var url = $"{_getMissionsDataUri}?device_id={DeviceId}";

        do
        {
            SendEvent(url, (response) =>
            {
            var data = JsonUtility.FromJson<MissionsData>(response);

                if (data != null && data.Missions.Count > 0)
                {
                    bool firstMissionCompleted = data.Missions.Any(e => e.Completed);

                    _missionsWindow.UpdateMissions(data);
                    _missionsButton.gameObject.SetActive(SettingsData.CanShowMissions);
                    _missionAlertIcon.SetActive(data.Missions.Any(e => !e.Completed));
                    _missionsHintButton.gameObject.SetActive(!ApplicationState.IsFirstLaunch && !ApplicationState.EventsCompleted.Contains("missions-opened"));

                    foreach (var mission in data.Missions)
                    {
                        if (mission.Completed && !mission.Notified)
                        {
                            ShowRewardCard(mission.Coins);

                            _targetEvents.Add($"mission-{mission.Id}");
                        }
                    }

                    if (firstMissionCompleted && !ApplicationState.EventsCompleted.Contains("missions-opened"))
                    {
                        _firstMissionCompletedPopup.SetActive(true);
                    }

                    CheckEvents();
                }
            });

            yield return new WaitForSecondsRealtime(60f);
        } while (repeat);
    }

    public void DailyStreakReward()
    {
        StartCoroutine(DailyStreakReward_Coroutine());
    }

    private IEnumerator GetDailyStreak()
    {
        if (BalanceHistoryData == null)
        {
            yield return null;
        }

        if (!SettingsData.CanShowMissions)
        {
            yield break;
        }

        var url = $"{_getDailyStreakUri}?device_id={DeviceId}&app_version={Application.version}";

        SendEvent(url, (response) =>
        {
            var data = JsonUtility.FromJson<DailyStreakData>(response);

            if (data != null && data.States.Count > 0)
            {
                _missionsWindow.UpdateDailyStreak(data);
                _leftSecondsToDailyStreak = data.LeftSecondsToEnd;

                CheckDailyStreakEvents(data.States);
            }

            _dailyStreakReceived = !data.States.Any(e => e == 1);
        });
    }

    private void CheckDailyStreakEvents(List<int> states)
    {
        for (int i = 1; i < states.Count; i++)
        {
            int state = states[i];
            int day = i + 1;

            if (state > 0)
            {
                var eventId = $"day{day}";
                _targetEvents.Add(eventId);
            }
        }

        CheckEvents();
    }

    private IEnumerator DailyStreakReward_Coroutine()
    {
        if (BalanceHistoryData == null)
        {
            yield return null;
        }

        if (!SettingsData.CanShowMissions || _dailyStreakReceived)
        {
            yield break;
        }

        var url = $"{_dailyStreakRewardsUri}?device_id={DeviceId}&app_version={Application.version}";

        SendEvent(url, (response) =>
        {
            var states = JsonUtility.FromJson<List<int>>(response);

            CheckDailyStreakEvents(states);
        });
    }

    public IEnumerator ChangePanels(GameObject newPanel, float timeToShow)
    {
        canChangePanel = false;
        prevPanel = currentActivePanel;
        newPanel.GetComponent<Canvas>().sortingOrder = 20;
        prevPanel.GetComponent<Canvas>().sortingOrder = 10;
        yield return new WaitForSeconds(timeToShow);

        currentActivePanel = newPanel;
        newPanel.SetActive(true);
        yield return new WaitForSeconds(0.35f);

        _earnButtonClickImage.SetActive(false);

        prevPanel.SetActive(false);

        canChangePanel = true;

        if (newPanel == lobbyPanel)
        {
            yield return new WaitForSeconds(5f);

            _earnButtonClickImage.SetActive(true);
        }
    }

    private IEnumerator GetSettings_Coroutine(Action<bool> onComplete)
    {
        // �������� ��� ��������� ����� ��� ������ �������
        {
            float waitTimer = 0f;

            while (ApplicationState.IsFirstLaunch && waitTimer < 2f)
            {
                waitTimer += Time.unscaledDeltaTime;
                yield return null;
            }
        }

        var referrerQuery = WebUtility.UrlDecode(_installReferrerDetails.InstallReferrer);
        var url = $"{_settingsUri}?device_id={DeviceId}&app_version={Application.version}";

        var request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        bool success = !request.isNetworkError && !request.isHttpError && request.responseCode == 200;

        if (success)
        {
            Debug.Log($"Settings loaded: {request.downloadHandler.text}");

            var json = request.downloadHandler.text;
            SettingsData = JsonUtility.FromJson<SettingsData>(json);

            if (!SettingsData.AccessRestricted && (SettingsData.ConnFromAnotherIP || SettingsData.ConnFromSameIP))
            {
                var errorText = SettingsData.ConnFromAnotherIP ? "You connected from unknown IP. Your account is temporarily restricted." : "You cannot connect to the application from the second device.";

                _blockedFromServerWindow.SetActive(true);
                _blockedFromServerWindow.GetComponentInChildren<Text>().text = errorText;
            }

            // �������� ������ � ����� ������ ������ ������������ 
            {
                var source = string.Empty;
                var channel = string.Empty;
                var subpublisher = string.Empty;

                if (!string.IsNullOrEmpty(_campaignName))
                {
                    source = _campaignName;
                    channel = "video-ads";
                    subpublisher = _installSite;
                }
                else
                {
                    source = "organics";
                    channel = "google-play";
                }

                Debug.Log($"utm_source: {source}, ua_channel: {channel}");

#if !UNITY_EDITOR
                _adjoeController.Init(DeviceId, source, channel, subpublisher, () =>
                {
                    if (!ApplicationState.EventsCompleted.Contains(FIRST_ADJOE_CLICK_TOKEN))
                    {
                        var url = $"{_adjoeButtonCallbackUri}?device_id={DeviceId}";
                        StartCoroutine(SendEventToServer_Coroutine(url, null));

                        SingularSDK.Event(FIRST_ADJOE_CLICK_TOKEN);
                        ApplicationState.EventsCompleted.Add(FIRST_ADJOE_CLICK_TOKEN);
                        SaveData();
                    }
                });

                //_appSamuraiController.Init(DeviceId, GpsAdid, channel, source, SettingsData.CountryCode);
                //_ayetController.Init(DeviceId);

                MCOfferwallObject.Instance.SetGAID(GpsAdid);
                MCOfferwallObject.Instance.SetAffSub1(source);
                MCOfferwallObject.Instance.SetAffSub2(channel);
                MCOfferwallObject.Instance.SetAffSub2(subpublisher);
                MCOfferwallObject.Instance.SetUserId(DeviceId);
#endif
            }

            if (SettingsData.CheckUpdate)
            {
                _updatePanel.SetActive(true);
                yield break;
            } else
            {
                yield return GetGiftSlots_Coroutine();
                yield return GetReferrers_Coroutine();
            }

            StartCoroutine(UpdateBalance_Coroutine(true));
            StartCoroutine(GetMissions(true));
            StartCoroutine(GetDailyStreak());

            while (BalanceHistoryData == null)
            {
                yield return null;
            }

            _profileScreen.Init(this, ReferrersData);

            yield return InitPayout_Coroutine();

            StartCoroutine(CheckRevenueData());
            StartCoroutine(GetLeaderboard_Coroutine());
        }
        else
        {
            var desc = $"Error loading settings: {request.error}";

            _accessRestrictedWindow.Show(desc);

            Debug.LogError(desc);
        }

        request.Dispose();

        ApplicationState.IsFirstLaunch = false;
        SaveData();

        onComplete?.Invoke(success);

        yield return new WaitForSeconds(0.1f);

        NavigationButtonSelect(allNavigationButtons[0]);

        if (SettingsData.HasWelcomeBonus)
        {
            _welcomeBonusWindow.SetActive(true);
        }

        while (BalanceHistoryData.CurrentBalance == 0)
        {
            yield return null;
        }

        TryShowingRewardCard();

        var payoutData = BalanceHistoryData.PayoutHistoryData.LastOrDefault(e => e.Status == 1);

        if (payoutData != null)
        {
            if (SettingsData.PayoutNotify)
            {
                _payoutNotifyPanel.gameObject.SetActive(true);
                _payoutNotifyButton.interactable = false;
                _payoutNotifyText.text = $"Your reward was sent to\r\n {payoutData.Wallet} \r\nPlease check your email";

                _payoutNotifySeq = DOTween.Sequence()
                    .Append(_payoutNotifyPanel.DOFade(1f, 1f))
                    .AppendCallback(() =>
                    {
                        _payoutNotifyButton.interactable = true;
                    })
                    .AppendInterval(5f)
                    .AppendCallback(() =>
                    {
                        _payoutNotifyButton.interactable = false;
                    })
                    .Append(_payoutNotifyPanel.DOFade(0f, 1f))
                    .AppendCallback(() =>
                    {
                        _payoutNotifyPanel.gameObject.SetActive(false);
                    });
            }

            if (string.IsNullOrEmpty(SettingsData.Email))
            {
                SettingsData.Email = payoutData.Wallet;
            }
        }
    }

    private void TryShowingRewardCard()
    {
        int coinsDiff = BalanceHistoryData.OfferwallEarnedCoins - ApplicationState.PrevOfferwallsCoins;

        if (coinsDiff > 0)
        {
            ApplicationState.PrevOfferwallsCoins = BalanceHistoryData.OfferwallEarnedCoins;
            SaveData();

            ShowRewardCard(coinsDiff);
        }
    }

    private void ShowRewardCard(int coins)
    {
        int randomIndex = Random.Range(0, _rewardCallbackPanelPoints.Length);
        Transform spawnPoint = _rewardCallbackPanelPoints[randomIndex];

        RewardCard card = Instantiate(cardPrefab, cardParent);
        RectTransform cardRect = card.GetComponent<RectTransform>();

        cardRect.localPosition = spawnPoint.localPosition;

        card.Show(coins, endPointCard, () =>
        {

        });
    }

    private IEnumerator UpdateBalance_Coroutine(bool repeat, Action onComplete = null)
    {
        var waitForSeconds = new WaitForSeconds(10f);

        do
        {
            while (_authWindow.gameObject.activeSelf)
            {
                yield return null;
            }

            var url = $"{_balanceHistoryUri}?device_id={DeviceId}&gift_index={_giftsData.BestGiftIndex}";
            var request = UnityWebRequest.Get(url);

            yield return request.SendWebRequest();

            bool success = !request.isNetworkError && !request.isHttpError && request.responseCode == 200;

            if (success)
            {
                Debug.Log($"Balance update request success: {request.downloadHandler.text}");

                var json = request.downloadHandler.text;
                BalanceHistoryData = JsonUtility.FromJson<BalanceHistoryData>(json);

                _balanceHistoryScreen.Show(BalanceHistoryData);

                moneyCount = BalanceHistoryData.CurrentBalance;
                PlayerPrefs.SetInt("moneyCount", moneyCount);

                CheckEvents();

                var blockedPayout = BalanceHistoryData.PayoutHistoryData.FirstOrDefault(e => e.Status == 3 || e.Status == 5 || e.Status == 7);
                bool withdrawIsBlocked = blockedPayout != null;

                if (SettingsData.AccessRestricted || withdrawIsBlocked)
                {
                    var errorText = "Access Restricted. Please send your ID to pyotr@plus-games.com";

                    if (withdrawIsBlocked)
                    {
                        switch (blockedPayout.Status)
                        {
                            case 3:
                            case 5:
                                errorText = "Access Restricted. According to our Terms of Service (https://plus-games.com/bucks-up-terms). It is not allowed to use several devices for coins earning.";
                                break;
                            case 7:
                                errorText = "Terms & Conditions of Use Violated.";
                                break;
                        }
                    }

                    _accessRestrictedWindow.Show(errorText);

                    yield break;
                }

                if (BalanceHistoryData.InstagramBonusNotify)
                {
                    _instBonusNotifyPanel.SetActive(true);
                }

                if (!ApplicationState.RatingShowed && BalanceHistoryData.PayoutHistoryData.Any(e => e.Status == 1))
                {
                    ApplicationState.RatingShowed = true;
                    SaveData();

                    rateInfoPanel.SetActive(true);
                }


                if (!ApplicationState.MoreGamesWindowShowed && SettingsData.AdjoeEnabled && SettingsData.AdjoeOpened)
                {
                    ApplicationState.MoreGamesWindowShowed = true;
                    SaveData();

                    _moreGamesWindow.SetActive(true);
                }

                if (!ApplicationState.MoreGames2WindowShowed && SettingsData.Status >= UserStatus.ApprenticeGameTester)
                {
                    ApplicationState.MoreGames2WindowShowed = true;
                    SaveData();

                    _moreGames2Window.SetActive(true);
                }

                bool showMoreGames = SettingsData.AdjoeOpened;
                bool showMoreGames2 = SettingsData.MyChipsEnabled && !SettingsData.AccessRestricted && SettingsData.Status >= UserStatus.ApprenticeGameTester;
                var moreGames2Button = SettingsData.MychipsPromo ? _moreGamesPromoButton : _moreGames2Button;

                if (showMoreGames2 && SettingsData.MychipsPromo && !ApplicationState.MoreGamesPromoWindowShowed)
                {
                    ApplicationState.MoreGamesPromoWindowShowed = true;
                    SaveData();

                    _moreGamesPromoWindow.SetActive(true);
                }

                _moreGamesButton.gameObject.SetActive(showMoreGames);
                moreGames2Button.gameObject.SetActive(showMoreGames2);

                //_giftButton.gameObject.SetActive(BalanceHistoryData.CurrentBalance > 0);
                _cashbackButton.gameObject.SetActive((SettingsData != null && !SettingsData.IsOrganic) || (BalanceHistoryData != null && BalanceHistoryData.OfferwallEarnedCoins > 0));
                _leaderboardButton.gameObject.SetActive(BalanceHistoryData.OfferwallEarnedCoins > 0);

                _payoutScreen.UpdateView();

                if (!ApplicationState.HasNotifications && HasNotificationPermision())
                {
                    NotificationSender.ScheduleNotifications();

                    ApplicationState.HasNotifications = true;
                    SaveData();
                }
            }
            else
            {
                Debug.LogError($"Balance update request error: {request.error}");
            }

            request.Dispose();

            onComplete?.Invoke();

            yield return waitForSeconds;
        } while (repeat);
    }

    private IEnumerator InitPayout_Coroutine()
    {
        yield return GetCountryByIP_Coroutine();

        var url = $"{_getPayoutSlotsUri}?device_id={DeviceId}&gift_percent_index={_giftsData.BestGiftPercentIndex}";
        var request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        bool success = !request.isNetworkError && !request.isHttpError && request.responseCode == 200;

        if (success)
        {
            Debug.Log($"Payout slots data request success: {request.downloadHandler.text}");

            var json = request.downloadHandler.text;
            var payoutData = JsonUtility.FromJson<PayoutData>(json);
            var countryCode = ServerLocationData.countryCode.ToLowerInvariant();
            bool showAdditionalSlots = countryCode == "usa" || countryCode == "us";

            // ������� ����� �����
            //showAdditionalSlots = true;

            payoutData.SlotsData = payoutData.SlotsData.OrderBy(e => e.coins_amount).ToList();

            _payoutScreen.Init(this, payoutData, countryCode, (slotData) =>
            {
                currencyWithdraw = (int)(slotData.currency_amount * 10) * 0.1f;
                Withdraw(slotData);
            },
            showAdditionalSlots);
        }
        else
        {
            Debug.LogError($"Payout slots data request error: {request.error}");
        }

        request.Dispose();
    }

    private IEnumerator GetCountryByIP_Coroutine()
    {
        var url = _checkCountryUri;
        var request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        bool success = !request.isNetworkError && !request.isHttpError && request.responseCode == 200;

        if (success)
        {
            Debug.Log($"Location data request success: {request.downloadHandler.text}");

            var json = request.downloadHandler.text;
            ServerLocationData = JsonUtility.FromJson<ServerLocationData>(json);
        }
        else
        {
            Debug.LogError($"Location data request error: {request.error}");
        }

        request.Dispose();
    }

    private IEnumerator Withdraw_Coroutine(PayoutSlotData slotData, string phoneNumber)
    {
        if (isWithdraw)
        {
            yield break;
        }

        var deviceId = DeviceId;

        isWithdraw = true;

        //#if UNITY_EDITOR
        //        userId = "220b8adbbad18245a52b628bfde3cf5f";
        //#endif

        //if (ApplicationState.InstallSource != InstallSource.organic || FirebaseManager.Instance.IsVerificationCompleted)
        {
            int a = UnityEngine.Random.Range(1, 10);
            int b = 10 - a;
            deviceId = deviceId.Insert(3, a.ToString());
            deviceId = deviceId.Insert(10, b.ToString());
        }

        //var phoneNumber = ""; //ApplicationState.InstallSource != InstallSource.organic ? "" : FirebaseManager.Instance.User.PhoneNumber;
        var wallet = withdrawInputField.text;
        var payoutId = slotData.id;
        var url = $"{_withdrawUri}?payout_slot_id={payoutId}&wallet={wallet}&app_version={Application.version}&device_id={deviceId}&phone={phoneNumber}&gift_index={_giftsData.BestGiftIndex}&gift_percent_index={_giftsData.BestGiftPercentIndex}";
        var request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        bool success = !request.isNetworkError && !request.isHttpError && request.responseCode == 200;

        if (success)
        {
            var withdrawState = JsonUtility.FromJson<WithdrawState>(request.downloadHandler.text);

            switch (withdrawState.Status)
            {
                case 1:
                    {
                        moneyCount -= needCoinsToWithdraw;
                        PlayerPrefs.SetInt("moneyCount", moneyCount);

                        // ����� �������� �� ������� ����� �������� � ������� ������
                        //if (ApplicationState.InstallSource == InstallSource.organic)
                        //{
                        //    SettingsData.PhoneNumber = FirebaseManager.Instance.User.PhoneNumber;
                        //}

                        // ��������� �������
                        StartCoroutine(GetGiftSlots_Coroutine());

                        // ��������� �������� ������
                        StartCoroutine(InitPayout_Coroutine());

                        bool isPaypal = slotData.payout_type == (int)PayoutType.PayPal;
                        var term = "Popup/PayoutSuccess";

                        if (isPaypal)
                        {
                            term = withdrawState.DirectPaypal ? "Popup/PayoutSuccess/Paypal" : "Popup/PayoutSuccess/TremendoesPaypal";
                        }

                        var localization = LocalizationManager.GetTranslation(term);

                        withdrawCompleteInfoPanel.Show(localization);
                    }
                    break;
                case 2:
                case 3:
                case 5:
                case 6:
                    var erroText = LocalizationManager.GetTranslation($"Popup/PayoutError{withdrawState.Status}");
                    _infoWindow.Show(erroText);
                    break;
                default:
                    _infoWindow.Show("Withdraw request failed.");
                    break;
            }
        }
        else
        {
            _infoWindow.Show($"Withdraw request error: {request.error}");
        }

        request.Dispose();

        _withdrawWindow.Hide();

        isWithdraw = false;
    }

    public Dictionary<string, string> ParseQueryString(string requestQueryString)
    {
        Dictionary<string, string> rc = new Dictionary<string, string>();
        string[] ar1 = requestQueryString.Split(new char[] { '&', '?' });
        foreach (string row in ar1)
        {
            if (string.IsNullOrEmpty(row)) continue;
            int index = row.IndexOf('=');
            if (index < 0) continue;
            rc[Uri.UnescapeDataString(row.Substring(0, index))] = Uri.UnescapeDataString(row.Substring(index + 1)); // use Unescape only parts          
        }
        return rc;
    }

    private void ShowLobby()
    {
        if (PlayerPrefs.GetInt("tutorial") == 0)
        {
            CompleteTutorial();
        }
        else
        {
            _signInPanel.SetActive(false);
            registerPanel.SetActive(false);
            navigationBar.SetActive(true);
            lobbyPanel.SetActive(true);
            lobbyPanel.GetComponent<Animator>().SetTrigger("Idle");
        }

        //StartCoroutine(CheckDeviceDuplicate_Coroutine((success) =>
        //{
        //    if (success)
        //    {
        //        StartCoroutine(GetSettings_Coroutine());
        //    }
        //    else
        //    {
        //        _linkedDeviceNotValidWindow.Show($"Google ID:\n {FirebaseManager.Instance.UserId} \n\nDevice ID:\n {SystemInfo.deviceUniqueIdentifier}");
        //    }
        //}));
    }

    private IEnumerator GetInstagramBonus_Coroutine(Action onComplete)
    {
        var url = $"{_instagramBonusCallbackUri}?device_id={DeviceId}&device_model={SystemInfo.deviceModel}&app_version={Application.version}";
        var request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        bool success = !request.isNetworkError && !request.isHttpError && request.responseCode == 200;

        if (success)
        {
            Debug.Log($"Instagram bonus callback request success: {request.downloadHandler.text}");
        }
        else
        {
            Debug.LogError($"Instagram bonus callback request error: {request.error}");
        }

        request.Dispose();

        onComplete.Invoke();
    }

    public void SendEvent(string url, Action<string> onComplete)
    {
        StartCoroutine(SendEventToServer_Coroutine(url, onComplete));
    }

    private IEnumerator SendEventToServer_Coroutine(string url, Action<string> onComplete)
    {
        var request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        bool success = !request.isNetworkError && !request.isHttpError && request.responseCode == 200;

        if (success)
        {
            Debug.Log($"Sending event to {url} is success: {request.downloadHandler.text}");
        }
        else
        {
            Debug.LogError($"Sending event to {url} is failed: {request.error}");
        }

        onComplete?.Invoke(request.downloadHandler.text);

        request.Dispose();
    }

    public void UpdateProfile(string name, string surname, string email, string phone)
    {
        var validPhone = ClearPhoneNumber(phone);

        StartCoroutine(UpdateProfile_Coroutine(name, surname, email, validPhone));
    }

    private IEnumerator UpdateProfile_Coroutine(string name, string surname, string email, string phone)
    {
        var url = $"{_updateProfileUri}?device_id={DeviceId}&name={name}&surname={surname}&email={email}&phone={phone}";
        var request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        bool success = !request.isNetworkError && !request.isHttpError && request.responseCode == 200;

        if (success)
        {
            Debug.Log($"Updating profile request success.");

            SettingsData.Name = name;
            SettingsData.Surname = surname;
            SettingsData.Email = email;
            SettingsData.PhoneNumber = phone;
        }
        else
        {
            Debug.LogError($"Updating profile request error: {request.error}");
        }

        request.Dispose();
    }

    private IEnumerator GetReferrers_Coroutine()
    {
        var url = $"{_getReferrersUri}?device_id={DeviceId}";
        var request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        bool success = !request.isNetworkError && !request.isHttpError && request.responseCode == 200;

        if (success)
        {
            Debug.Log($"Referrers data request success: {request.downloadHandler.text}");

            var json = request.downloadHandler.text;
            ReferrersData = JsonUtility.FromJson<ReferrersData>(json);
        }
        else
        {
            Debug.LogError($"Referrers data request error: {request.error}");
        }

        request.Dispose();
    }

    public IEnumerator CheckUserExist_Coroutine(Action<bool> onComplete)
    {
        var url = $"{_checkUserExistUri}?device_id={DeviceId}&gps_adid={GpsAdid}";
        var request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        bool success = !request.isNetworkError && !request.isHttpError && request.responseCode == 200;

        if (success)
        {
            int.TryParse(request.downloadHandler.text, out int status);
            bool isExist = status == 1 && !ApplicationState.IsFirstLaunch;

            if (status == 1)
            {
                Debug.Log($"User is exist.");
            }

            onComplete.Invoke(isExist);
        }
        else
        {
            Debug.LogError($"Checking user exist request error: {request.error}");

            _accessRestrictedWindow.Show("No Internet");
        }

        request.Dispose();
    }

    public IEnumerator UserRegisteration_Coroutine(UserRegistrationInfo userInfo, Action<bool> onComplete)
    {
        var referrerQuery = WebUtility.UrlDecode(_installReferrerDetails.InstallReferrer);
        var refCode = string.IsNullOrEmpty(_refCode) ? userInfo.ReferrerCode : _refCode;
        var url = $"{_userRegistrationUri}?device_id={DeviceId}&is_male={userInfo.IsMale}&age={userInfo.Age}&gps_adid={GpsAdid}&{referrerQuery}&referrer_code={refCode}&singular_device_id={SingularDeviceId}&network_name={_networkName}&campaign_name={_campaignName}&install_site={_installSite}&creative_name={_creativeName}";
        var request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        bool success = !request.isNetworkError && !request.isHttpError && request.responseCode == 200;

        if (success)
        {
            int.TryParse(request.downloadHandler.text, out int status);

            if (status == 1)
            {
                Debug.Log($"User registration completed.");
            }

            onComplete.Invoke(status == 1);
        }
        else
        {
            Debug.Log($"User registration failed. Response text: {request.downloadHandler.text}");
        }

        request.Dispose();
    }

    public void DeleteAccount()
    {
        StartCoroutine(DeleteAccount_Coroutine());
    }

    private IEnumerator DeleteAccount_Coroutine()
    {
        var url = $"{_deleteAccountUri}?device_id={DeviceId}";
        var request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        bool success = !request.isNetworkError && !request.isHttpError && request.responseCode == 200;

        if (success)
        {
            Debug.Log($"User acoount was deleted.");
            SceneManager.LoadScene(0);
        }
        else
        {
            Debug.Log($"Account deletion failed. Response text: {request.downloadHandler.text}");
        }

        request.Dispose();
    }

    public void UpdateGiftSlots()
    {
        StartCoroutine(GetGiftSlots_Coroutine());
    }

    private IEnumerator GetGiftSlots_Coroutine()
    {
        var url = $"{_getGiftSlotsUri}?device_id={DeviceId}";
        var request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        bool success = !request.isNetworkError && !request.isHttpError && request.responseCode == 200;

        if (success)
        {
            Debug.Log($"Gift slots data request success: {request.downloadHandler.text}");

            var json = request.downloadHandler.text;
            _giftsData = JsonUtility.FromJson<GiftsData>(json);
            _leftSecondsToGiftUpdate = _giftsData.LeftSecondsToUpdate;
            _giftScreen.UpdateSlots(_giftsData);
        }
        else
        {
            Debug.LogError($"Gift slots data request error: {request.error}");
        }

        request.Dispose();

        _giftRequestIsSended = false;
    }

    private IEnumerator GetLeaderboard_Coroutine()
    {
        var waitForSeconds = new WaitForSecondsRealtime(30f);

        while (true)
        {
            if (BalanceHistoryData.OfferwallEarnedCoins > 0)
            {
                var url = $"{_getLeaderboardUri}?device_id={DeviceId}&app_version={Application.version}";
                var request = UnityWebRequest.Get(url);

                yield return request.SendWebRequest();

                bool success = !request.isNetworkError && !request.isHttpError && request.responseCode == 200;

                if (success)
                {
                    Debug.Log($"Leaderboard data request success: {request.downloadHandler.text}");

                    var json = request.downloadHandler.text;
                    var leaderboardData = JsonUtility.FromJson<LeaderboardData>(json);

                    if (leaderboardData != null)
                    {
                        _leftSecondsToLeaderboardUpdate = leaderboardData.LeftSecondsToUpdate;
                    }

                    _leaderboardScreen.Init(leaderboardData);
                }
                else
                {
                    Debug.LogError($"Leaderboard data request error: {request.error}");
                }

                request.Dispose();
            }

            yield return waitForSeconds;
        }
    }

    public void UpdatePayoutSlots()
    {
        StartCoroutine(InitPayout_Coroutine());
    }

    public static string ClearPhoneNumber(string phoneNumber)
    {
        string phone = string.Empty;

        foreach (char c in phoneNumber)
        {
            if (c < '0' || c > '9')
            {
                continue;
            }

            phone += c;
        }

        return phone;
    }

    public bool IsRubCountries()
    {
        var countryCode = ServerLocationData.countryCode.ToLowerInvariant();
        return _rubCountries.Contains(countryCode);
    }

    public static string GetAndroidAdvertiserId()
    {
        string advertisingID = "";
        try
        {
            AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = up.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaClass client = new AndroidJavaClass("com.google.android.gms.ads.identifier.AdvertisingIdClient");
            AndroidJavaObject adInfo = client.CallStatic<AndroidJavaObject>("getAdvertisingIdInfo", currentActivity);

            advertisingID = adInfo.Call<string>("getId").ToString();
        }
        catch (Exception)
        {
        }
        return advertisingID;
    }

    public static void CopyToClickboard(string text)
    {
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject clipboard = jo.Call<AndroidJavaObject>("getSystemService", "clipboard");
        AndroidJavaClass clipdata = new AndroidJavaClass("android.content.ClipData");
        AndroidJavaObject clipObject = clipdata.CallStatic<AndroidJavaObject>("newPlainText", "label", text);
        clipboard.Call("setPrimaryClip", clipObject);
    }
    
    private IEnumerator CreateTestCard_Coroutine()
    {
        yield return new WaitForSeconds(5f);

        int randomIndex = Random.Range(0, _rewardCallbackPanelPoints.Length);
        Transform spawnPoint = _rewardCallbackPanelPoints[randomIndex];

        Debug.Log($"[TestCard] Карточка появилась на позиции {randomIndex} ({spawnPoint.name})");
        RewardCard card = Instantiate(cardPrefab, cardParent);
        RectTransform cardRect = card.GetComponent<RectTransform>();
        
        cardRect.localPosition = spawnPoint.localPosition;
        
        if (!card.gameObject.activeSelf)
            card.gameObject.SetActive(true);

        card.Show(999, endPointCard);
    }

    public void DidSetSdid(string result)
    {
        SingularDeviceId = result;

        Debug.Log($"singular_device_id:{SingularDeviceId}");
    }

    public void SdidReceived(string result)
    {
        SingularDeviceId = result;

        Debug.Log($"singular_device_id:{SingularDeviceId}");
    }

    public void OnSingularDeviceAttributionCallback(Dictionary<string, object> attributionInfo)
    {
        foreach (var kvp in attributionInfo)
        {
            Debug.Log($"OnSingularDeviceAttributionCallback Key: {kvp.Key}, Value: {kvp.Value}");
        }

        if (attributionInfo.ContainsKey("network"))
        {
            attributionInfo.TryGetValue("network", out object networkName);
            attributionInfo.TryGetValue("campaign", out object campaignName);
            attributionInfo.TryGetValue("install_site", out object installSite);
            attributionInfo.TryGetValue("install_creative", out object creativeName);

            _networkName = networkName.ToString().ToLowerInvariant();
            _campaignName = campaignName.ToString().ToLowerInvariant();
            _installSite = installSite.ToString().ToLowerInvariant();
            _creativeName = creativeName.ToString().ToLowerInvariant();
        }

        Debug.Log($"network_name={_networkName}, campaign_name={_campaignName}, install_site={_installSite}, install_creative={_creativeName}");
    }

    private void SendWelcomeBonusRequest()
    {
        var url = $"{_getWelcomeBonusUri}?device_id={DeviceId}";

        SendEvent(url, (response) =>
        {
            StartCoroutine(UpdateBalance_Coroutine(false, () =>
            {
                _welcomeBonusWindow.SetActive(false);
                ShowPaypalPayout();
                //TryShowingRewardCard();
            }));
        });
    }

    private void InitializeCountryFlags()
    {
        if (_countryFlags.Any())
        {
            return;
        }

        var flags = Resources.LoadAll<Sprite>("CountryFlags");

        foreach (var flag in flags)
        {
            var name = flag.name.ToLowerInvariant();

            if (!_countryFlags.ContainsKey(name))
            {
                _countryFlags.Add(name, flag);
            }
        }
    }

    public static Sprite GetCountryFlag(string countryCode)
    {
        _countryFlags.TryGetValue(countryCode.ToLowerInvariant(), out Sprite flag);

        return flag;
    }

    public async void ShowPaypalPayout()
    {
        _payoutButton.OnClick();
        ChangePanel(_payoutScreen.gameObject);

        await Task.Delay(500);

        _payoutScreen.ScrollToType(PayoutType.PayPal, true);
    }

    public void OnClickEarnButton()
    {
        if (SettingsData.AdjoeForEarnButton)
        {
            if (SettingsData.AdjoeEnabled)
            {
                _adjoeController.ShowOfferwall();
                SettingsData.AdjoeOpened = true;
            }
        }
        else
        {
            if (SettingsData.PrimeEnabled)
            {
                _primeController.ShowOfferwall();
                SettingsData.AdjoeOpened = true;
            }
        }
    }

    public void OnClickMoreGamesButton()
    {
        if (!SettingsData.AdjoeForEarnButton)
        {
            if (SettingsData.AdjoeEnabled)
            {
                _adjoeController.ShowOfferwall();
            }
        }
        else
        {
            if (SettingsData.PrimeEnabled)
            {
                _primeController.ShowOfferwall();
            }
        }
    }

    public static bool HasNotificationPermision()
    {
        return NotificationPermissionService.HasPermission() || NotificationPermissionService.AreNotificationsEnabledInSystem();
    }

    public void RequestNotificationPermission()
    {
        if (!SettingsData.CanShowMissions || HasNotificationPermision())
        {
            return;
        }

        StartCoroutine(NotificationPermissionService.RequestPermission(granted =>
        {
            Debug.Log("Notification permission granted: " + granted);

            if (!granted)
            {
                NotificationPermissionService.OpenNotificationSettings();
            } else
            {
                _targetEvents.Add("push-accepted");
                CheckEvents();
            }
        }));
    }

    public void TryShowDailyStreakNotificationPopup()
    {
        if (SettingsData.CanShowMissions && !ApplicationState.HasNotifications)
        {
            _dailyStreakNotificationPopup.SetActive(true);
        }
    }
}