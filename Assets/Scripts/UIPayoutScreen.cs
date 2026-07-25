using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using DG.Tweening;
using UI;

public enum PayoutType
{
    PayPal,
    Amazon,
    Adidas,
    AirBNB,
    Apple,
    BurgerKing,
    Dominos,
    GAP,
    GooglePlay,
    Netflix,
    Nike,
    Spotify,
    Uber,
    UberEats,
    NaverPay,
    LotteMart,
    BaskinRobbins,
    JawsTopokki,
    Starbucks,
    QuoPay,
    CU,
    HappyMoney,
    JCBPremo,
}

[System.Serializable]
public class PayoutMethod
{
    public PayoutType Type;
    public Sprite Icon;
    public Transform Parent;
    public bool IsAdditional = false;
}

[System.Serializable]
public class PayoutData
{
    public List<PayoutSlotData> SlotsData;
}

[System.Serializable]
public class PayoutSlotData
{
    public int coins_amount;
    public float currency_amount;
    public int payout_type;
    public bool is_active;
    public int id;
    public bool direct_paypal;
    public bool usage_limited;
}

public class UIPayoutScreen : MonoBehaviour
{
    [SerializeField] private UIPayoutSlot _payoutSlotPrefab;
    [SerializeField] private RectTransform _payoutSlotsParent;
    [SerializeField] private GameObject _placeholder;
    [SerializeField] private GameObject _dataParent;
    [SerializeField] private PayoutMethod[] _payoutMethods;

    [Header("Scroll")]
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private RectTransform _viewport;
    [SerializeField] private RectTransform _content;
    [SerializeField] private float _scrollDuration = 0.25f;
    [SerializeField] private Ease _scrollEase = Ease.OutQuad;
    [SerializeField] private PayPalButtonsController _tabsController;

    private readonly List<UIPayoutSlot> _slots = new List<UIPayoutSlot>();
    private Tween _scrollTween;
    private bool _suppressTabSync;

    private void OnValidate()
    {
        AutoWireScroll();
    }

    private void Awake()
    {
        AutoWireScroll();
    }

    private void OnEnable()
    {
        AutoWireScroll();
        if (_scrollRect != null)
            _scrollRect.onValueChanged.AddListener(OnScrollChanged);
    }

    private void OnDisable()
    {
        if (_scrollRect != null)
            _scrollRect.onValueChanged.RemoveListener(OnScrollChanged);
    }

    private void AutoWireScroll()
    {
        if (_scrollRect == null)
            _scrollRect = GetComponentInChildren<ScrollRect>(true);
        if (_scrollRect != null)
        {
            if (_viewport == null)
                _viewport = _scrollRect.viewport;
            if (_content == null)
                _content = _scrollRect.content;
        }
    }

    public void Init(ApplicationController appController, PayoutData payoutData, string countryCode, System.Action<PayoutSlotData> onClick, bool showAdditionalSlots)
    {
        if (_slots.Any())
        {
            Show(payoutData, onClick, showAdditionalSlots);

            return;
        }


        foreach (var slotData in payoutData.SlotsData)
        {
            var payoutType = (PayoutType)slotData.payout_type;
            var payoutMethod =_payoutMethods.FirstOrDefault(e => e.Type == payoutType);

            if (payoutMethod == null || (payoutMethod.IsAdditional && !showAdditionalSlots))
            {
                continue;
            }

            var slot = Instantiate(_payoutSlotPrefab, payoutMethod.Parent);
            slot.Show(slotData, payoutMethod, onClick);

            _slots.Add(slot);
        }

        // если нет карточек, то скрываем раздел
        foreach (var method in _payoutMethods)
        {
            int payoutTypeInt = (int)method.Type;
            bool show = payoutData.SlotsData.Any(e => e.payout_type == payoutTypeInt) && (!method.IsAdditional || showAdditionalSlots);

            method.Parent.parent.gameObject.SetActive(show);
        }

        _placeholder.SetActive(false);
        _dataParent.SetActive(true);

        LayoutRebuilder.ForceRebuildLayoutImmediate(_payoutSlotsParent);
    }

    public void ScrollToType(PayoutType type, bool animated = true)
    {
        AutoWireScroll();
        if (_scrollRect == null || _content == null || _viewport == null)
            return;

        var method = _payoutMethods != null ? _payoutMethods.FirstOrDefault(m => m.Type == type) : null;
        if (method == null || method.Parent == null)
            return;

        var target = method.Parent.parent as RectTransform;
        if (target == null)
            target = method.Parent as RectTransform;

        Canvas.ForceUpdateCanvases();

        var contentRT = _content;
        var viewportRT = _viewport;

        Vector3[] targetCorners = new Vector3[4];
        Vector3[] viewportCorners = new Vector3[4];
        target.GetWorldCorners(targetCorners);
        viewportRT.GetWorldCorners(viewportCorners);

        float targetTopY = targetCorners[1].y; 
        float viewportTopY = viewportCorners[1].y;
        float deltaWorldY = viewportTopY - targetTopY;
        float deltaY = viewportRT.InverseTransformVector(new Vector3(0f, deltaWorldY, 0f)).y;

        Vector2 pos = contentRT.anchoredPosition;
        pos.y += deltaY;

        float contentH = contentRT.rect.height;
        float viewportH = viewportRT.rect.height;
        float maxY = Mathf.Max(0f, contentH - viewportH);
        pos.y = Mathf.Clamp(pos.y, 0f, maxY);

        _suppressTabSync = true;

        if (animated && _scrollDuration > 0f)
        {
            if (_scrollTween != null && _scrollTween.IsActive())
                _scrollTween.Kill(false);

            contentRT.DOKill();
            _scrollTween = contentRT
                .DOAnchorPos(pos, _scrollDuration)
                .SetEase(_scrollEase)
                .OnKill(() => _suppressTabSync = false)
                .OnComplete(() => _suppressTabSync = false);
        }
        else
        {
            contentRT.anchoredPosition = pos;
            _suppressTabSync = false;
        }
    }

    private void OnScrollChanged(Vector2 _)
    {
        if (_suppressTabSync || _tabsController == null || _viewport == null)
            return;

        var type = GetTypeAtViewportTop();
        if (type.HasValue && _tabsController.SelectedType != type.Value)
            _tabsController.SelectFromScroll(type.Value, animate: true);
    }

    private PayoutType? GetTypeAtViewportTop()
    {
        if (_payoutMethods == null || _payoutMethods.Length == 0 || _viewport == null)
            return null;

        float viewportTop = _viewport.rect.yMax;
        const float eps = 0.1f;

        bool hasAbove = false;
        float bestAbove = float.PositiveInfinity;
        PayoutType bestAboveType = default;

        bool hasBelow = false;
        float bestBelow = float.NegativeInfinity;
        PayoutType bestBelowType = default;

        Vector3[] corners = new Vector3[4];

        for (int i = 0; i < _payoutMethods.Length; i++)
        {
            var m = _payoutMethods[i];
            if (m == null || m.Parent == null) continue;

            var group = m.Parent.parent as RectTransform;
            if (group == null) group = m.Parent as RectTransform;
            if (group == null || !group.gameObject.activeInHierarchy) continue;

            group.GetWorldCorners(corners);
            float topLocalY = _viewport.InverseTransformPoint(corners[1]).y;

            if (topLocalY >= viewportTop - eps)
            {
                if (!hasAbove || topLocalY < bestAbove)
                {
                    hasAbove = true;
                    bestAbove = topLocalY;
                    bestAboveType = m.Type;
                }
            }
            else
            {
                if (!hasBelow || topLocalY > bestBelow)
                {
                    hasBelow = true;
                    bestBelow = topLocalY;
                    bestBelowType = m.Type;
                }
            }
        }

        if (hasAbove) return bestAboveType;
        if (hasBelow) return bestBelowType;
        return null;
    }

    private void Show(PayoutData payoutData, System.Action<PayoutSlotData> onClick, bool showAdditionalSlots)
    {
        _slots.ForEach(e => e.Hide());

        for (int i = 0; i < payoutData.SlotsData.Count; i++)
        {
            var slotData = payoutData.SlotsData[i];
            var payoutType = (PayoutType)slotData.payout_type;
            var payoutMethod = _payoutMethods.FirstOrDefault(e => e.Type == payoutType);

            if (payoutMethod.IsAdditional && !showAdditionalSlots)
            {
                continue;
            }

            UIPayoutSlot view; 

            if (i >= _slots.Count)
            {
                view = Instantiate(_payoutSlotPrefab, payoutMethod.Parent);
                _slots.Add(view);
            }
            else
            {
                view = _slots[i];
            }

            view.Show(slotData, payoutMethod, onClick);
        }
    }

    public void UpdateView()
    {
        foreach (var slot in _slots)
        {
            slot.UpdateView();
        }
    }
}
