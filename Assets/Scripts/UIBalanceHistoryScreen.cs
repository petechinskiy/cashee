using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

[System.Serializable]
public class BalanceHistoryData
{
    public int CurrentBalance;
    public int TodayEarnedCoins;
    public int LastWeekEarnedCoins;
    public int LastMonthEarnedCoins;
    public bool InstagramBonusNotify;
    public int OfferwallEarnedCoins;
    public List<PayoutHistoryData> PayoutHistoryData;
}

[System.Serializable]
public class PayoutHistoryData
{
    public string Date;
    public float PayoutUsd;
    public PayoutType PayoutType;
    public string Wallet;
    public int Status;
    public string CurrencySymbol;
    public string NeocurrencyCode;
    public bool DirectPaypal;
}

[System.Serializable]
public class PayoutState
{
    public PayoutStatus Status;
    public Color Color;
}

public enum PayoutStatus
{
    Pending,
    Processed,
    Declined
}

public class UIBalanceHistoryScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _lastDayCoinsText;
    [SerializeField] private TextMeshProUGUI _lastWeekCoinsText;
    [SerializeField] private TextMeshProUGUI _lastMonthCoinsText;
    [SerializeField] private GameObject _placeholder;
    [SerializeField] private UIPayoutHistorySlot _payoutHistorySlotPrefab;
    [SerializeField] private RectTransform _payoutHistorySlotsParent;
    [SerializeField] private Button _dontReceiveButton;
    [SerializeField] private GameObject _dontReceiveWindow;
    [SerializeField] private UICommonWindow _commonWindow;
    [SerializeField] private PayoutState[] _payoutStates;
    
    private readonly List<UIPayoutHistorySlot> _historySlots = new();

    public void Show(BalanceHistoryData balanceData)
    {
        _lastDayCoinsText.text = balanceData.TodayEarnedCoins.ToString();
        _lastWeekCoinsText.text = balanceData.LastWeekEarnedCoins.ToString();
        _lastMonthCoinsText.text = balanceData.LastMonthEarnedCoins.ToString();

        _placeholder.SetActive(!balanceData.PayoutHistoryData.Any());
        _dontReceiveButton.gameObject.SetActive(balanceData.PayoutHistoryData.Any());

        for (int i = 0; i < balanceData.PayoutHistoryData.Count; i++)
        {
            var slotData = balanceData.PayoutHistoryData[i];
            UIPayoutHistorySlot slot;

            if (i < _historySlots.Count)
            {
                slot = _historySlots[i];
            }
            else
            {
                slot = Instantiate(_payoutHistorySlotPrefab, _payoutHistorySlotsParent);
                _historySlots.Add(slot);
            }

            var status = PayoutStatus.Pending;

            if (slotData.Status == 1)
            {
                status = PayoutStatus.Processed;
            }
            else if (slotData.Status > 1 && slotData.Status != 9)
            {
                status = PayoutStatus.Declined;
            }

            var payoutState = _payoutStates.FirstOrDefault(e => e.Status == status);

            slot.Show(slotData, payoutState, () => _commonWindow.Show(() => Application.OpenURL($"https://redeem.yourdigitalreward.com/activate-code/{slotData.NeocurrencyCode}"), null, "Your payout were sent successfully"));
            slot.transform.SetAsLastSibling();
        }

        _dontReceiveButton.onClick.AddListener(() =>
        {
            _dontReceiveWindow.SetActive(true);
        });
    }
}
