using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPayoutSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _coinsText;
    [SerializeField] private TextMeshProUGUI _currencyText;
    [SerializeField] private Image _payoutMethodImage;
    [SerializeField] private Button _acceptButton;
    [SerializeField] private Color _activeColor;
    [SerializeField] private Color _inactiveColor;
    [SerializeField] private Sprite _activeCoinSprite;
    [SerializeField] private Sprite _inactiveCoinSprite;
    [SerializeField] private CanvasGroup _statusCanvasGroup;
     
    private PayoutSlotData _slotData;

    public void Show(PayoutSlotData slotData, PayoutMethod payoutMethod, Action<PayoutSlotData> onClick)
    {
        _slotData = slotData;

        var countryCode = ApplicationController.Instance.ServerLocationData.countryCode.ToLowerInvariant();
        var currencySymbol = "$";

        switch (countryCode)
        {
            case "de":
            case "fr":
            case "ie":
            case "it":
            case "es":
            case "se": // швеция
            case "dk": // дания
            case "cz": // чехия
            case "be": // бельгия
            case "at": // австрия
            case "nl": // нидерланды
                currencySymbol = "€";
                break;
            case "gb":
                currencySymbol = "£";
                break;
            case "ch": // швейцария
                currencySymbol = "₣";
                break;
            case "kr": // южная корея
                currencySymbol = "₩";
                break;
            case "jp": // япония
                currencySymbol = "¥";
                break;
        }

        _coinsText.text = $"{slotData.coins_amount} <sprite name=\"coin\">";
        _currencyText.text = Math.Round(slotData.currency_amount, 2).ToString("0.##", CultureInfo.InvariantCulture) + currencySymbol;
        _payoutMethodImage.sprite = payoutMethod.Icon;

        UpdateView();

        //var iconRectSize = payoutMethod.Icon.rect.size;
        //var targetSize = new Vector2(Mathf.Min(iconRectSize.x, 227f), Mathf.Min(iconRectSize.y, 227f));
        //_payoutMethodImage.GetComponent<RectTransform>().sizeDelta = targetSize;

        _acceptButton.interactable = slotData.is_active;
        _acceptButton.onClick.RemoveAllListeners();
        _acceptButton.onClick.AddListener(() =>
        {
            onClick.Invoke(slotData);
        });

        gameObject.SetActive(true);

        GetComponent<CanvasGroup>().alpha = slotData.usage_limited && slotData.is_active ? 0.6f : 1f;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void UpdateView()
    {
        if (_slotData == null)
        {
            return;
        }

        bool isActive = _slotData.is_active && ApplicationController.Instance.BalanceHistoryData.CurrentBalance >= _slotData.coins_amount;

        _coinsText.color = isActive ? _activeColor : _inactiveColor;
        _statusCanvasGroup.alpha = isActive ? 1f : 0.6f;
    }
}
