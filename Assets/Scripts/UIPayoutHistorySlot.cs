using I2.Loc;
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPayoutHistorySlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _dateText;
    [SerializeField] private TextMeshProUGUI _usdAmountText;
    [SerializeField] private Image _statusImage;
    [SerializeField] private TextMeshProUGUI _statusText;
    [SerializeField] private Button _claimButton;
    [SerializeField] private TextMeshProUGUI _requestText;

    public void Show(PayoutHistoryData slotData, PayoutState payoutState, Action onClaimButtonClick)
    {
        bool showClaimButton = !string.IsNullOrEmpty(slotData.NeocurrencyCode) && slotData.NeocurrencyCode != "0";
        string currencySymbol = Regex.Unescape(slotData.CurrencySymbol);

        _dateText.text = slotData.Date;
        _usdAmountText.text = $"{currencySymbol}{Math.Round(slotData.PayoutUsd, 2).ToString("0.##", CultureInfo.InvariantCulture)}";
        var statusTerm = string.Empty;

        switch(payoutState.Status)
        {
            case PayoutStatus.Pending:
                statusTerm = "Pending";
                break;
            case PayoutStatus.Processed:
                statusTerm = slotData.PayoutType == PayoutType.PayPal && slotData.DirectPaypal ? "Processed/Paypal" : "Processed";
                break;
            case PayoutStatus.Declined:
                statusTerm = slotData.Status == 6 ? "Declined/Limit" : "Declined";
                break;
        }
        
        _statusText.text = LocalizationManager.GetTranslation($"PayoutStatus/{statusTerm}");
        //_statusImage.color = new Color(payoutState.Color.r, payoutState.Color.g, payoutState.Color.b, 0.5f);
        _statusText.color = payoutState.Color;

        if (showClaimButton)
        {
            _claimButton.onClick.RemoveAllListeners();
            _claimButton.onClick.AddListener(() =>
            {
                onClaimButtonClick.Invoke();
            });
        }

        _claimButton.gameObject.SetActive(showClaimButton);
        _requestText.enabled = !showClaimButton;
    }
}
