using DG.Tweening;
using UnityEngine;

namespace UI
{
    public class PayPalButtonsController : MonoBehaviour
    {
        [SerializeField] private PayPalButtonView[] _buttons;
        [SerializeField] private bool _autoCollectFromChildren = true;
        [SerializeField] private UIPayoutScreen _payoutScreen;

        [SerializeField] private float _fadeDuration = 0.2f;
        [SerializeField] private Ease _fadeEase = Ease.OutQuad;
        [SerializeField] private Color _selectedColor;
        [SerializeField] private Color _defaultColor;

        private int _selectedIndex = -1;
        public PayoutType SelectedType { get; private set; }
        private bool _suppressScrollRequest;

        private void Awake()
        {
            if (_autoCollectFromChildren && (_buttons == null || _buttons.Length == 0))
                _buttons = GetComponentsInChildren<PayPalButtonView>(true);

            if (_buttons == null || _buttons.Length != 4)
                Debug.LogWarning($"PayPalButtonsController: expected 4 buttons, got {(_buttons == null ? 0 : _buttons.Length)}", this);

            for (int i = 0; i < _buttons.Length; i++)
            {
                int idx = i;
                if (_buttons[i] != null && _buttons[i].Button != null)
                {
                    _buttons[i].Button.onClick.AddListener(() => Select(idx, true));
                }
                else
                {
                    Debug.LogWarning($"PayPalButtonsController: button view [{i}] is not set properly", this);
                }
            }
        }

        private void OnEnable()
        {
            if (_autoCollectFromChildren && (_buttons == null || _buttons.Length == 0))
                _buttons = GetComponentsInChildren<PayPalButtonView>(true);

            if (_buttons == null || _buttons.Length == 0)
                return;

            for (int i = 0; i < _buttons.Length; i++)
            {
                SetButtonVisual(i, isSelected: false);
            }

            Select(0, true, force: true);
        }

        private void Select(int index, bool animate, bool force = false)
        {
            if (_buttons == null || _buttons.Length == 0)
                return;

            if (index < 0 || index >= _buttons.Length)
                return;

            if (!force && _selectedIndex == index)
                return;

            int prev = _selectedIndex;
            _selectedIndex = index;
            SelectedType = _buttons[index].PayoutType;

            if (prev >= 0)
                SetButtonVisual(prev, isSelected: false);

            SetButtonVisual(index, isSelected: true);

            if (!_suppressScrollRequest && _payoutScreen != null)
                _payoutScreen.ScrollToType(SelectedType, animated: true);
        }

        public void Select(PayoutType type, bool animate = true, bool force = false)
        {
            if (_buttons == null || _buttons.Length == 0)
                return;

            for (int i = 0; i < _buttons.Length; i++)
            {
                if (_buttons[i] != null && _buttons[i].PayoutType == type)
                {
                    Select(i, animate, force);
                    return;
                }
            }
        }

        public void SelectFromScroll(PayoutType type, bool animate = true)
        {
            _suppressScrollRequest = true;
            try
            {
                Select(type, animate, force: false);
            }
            finally
            {
                _suppressScrollRequest = false;
            }
        }

        private void SetButtonVisual(int index, bool isSelected)
        {
            var v = _buttons[index];
            if (v == null)
                return;

            if (v.Button != null)
                v.Button.interactable = !isSelected;

            var textColor = isSelected ? _selectedColor : _defaultColor;
            if (v.Text != null) v.Text.color = textColor;
            if (v.Image != null) v.Image.gameObject.SetActive(isSelected);
        }
    }
}
