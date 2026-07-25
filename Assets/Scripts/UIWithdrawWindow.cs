using I2.Loc;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIWithdrawWindow : MonoBehaviour
{
    [SerializeField] private Text _descText;
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private GameObject _editButton;
    [SerializeField] private Button _acceptButton;
    [SerializeField] private TextMeshProUGUI _desclaimerText;
    [SerializeField] private Text _errorText;
    [SerializeField] private UnityEvent _onClose;
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        _inputField.onValueChanged.AddListener((value) =>
        {
            bool hasError = true;

            if (value.Contains(".fr") || value.Contains(".fde") || value.Contains(".it"))
            {
                hasError = true;
                _errorText.text = LocalizationManager.GetTranslation("PayoutWindow/Error1");
            }
            else if (value.Contains("@aol.com"))
            {
                hasError = true;
                _errorText.text = LocalizationManager.GetTranslation("PayoutWindow/Error2");
            }
            else if (value.Length > 27)
            {
                hasError = true;
                _errorText.text = LocalizationManager.GetTranslation("PayoutWindow/Error3");
            }
            else
            {
                hasError = false;
            }

            _acceptButton.GetComponent<CanvasGroup>().alpha = hasError ? 0.6f : 1f;
            _errorText.gameObject.SetActive(hasError);
            _acceptButton.interactable = !hasError;
        });
    }

    public void Show(BalanceHistoryData balanceHistoryData, PayoutSlotData slotData, System.Action<PayoutSlotData> onAccept)
    {
        var payoutType = (PayoutType)slotData.payout_type;
        PayoutHistoryData payout = null;
        bool anySuccesfullPayout = payout != null;
        bool isPaypal = payoutType == PayoutType.PayPal && !slotData.direct_paypal;
        var desclaimerTerm = isPaypal ? "PayoutWindow/DesclaimerPaypal" : "PayoutWindow/Desclaimer";

        if (balanceHistoryData != null)
        {
            payout = balanceHistoryData.PayoutHistoryData.FirstOrDefault(e => (PayoutStatus)e.Status == PayoutStatus.Processed && e.PayoutType == payoutType);
        }

        _inputField.text = anySuccesfullPayout ? payout.Wallet : string.Empty;
        _desclaimerText.gameObject.SetActive(isPaypal);
        _desclaimerText.text = LocalizationManager.GetTranslation(desclaimerTerm);
        _descText.gameObject.SetActive(anySuccesfullPayout);
        _inputField.interactable = !anySuccesfullPayout;
        //_editButton.SetActive(anySuccesfullPayout);

        gameObject.SetActive(true);

        _acceptButton.onClick.RemoveAllListeners();
        _acceptButton.onClick.AddListener(() => onAccept.Invoke(slotData));
        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(Hide);

        LayoutRebuilder.ForceRebuildLayoutImmediate(_desclaimerText.transform.parent as RectTransform);
    }

    public void Hide()
    {
        _onClose.Invoke();
    }
}
