using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UI
{
    public class PayPalButtonView : MonoBehaviour
    {
        [SerializeField] private PayoutType _payoutType;
        [SerializeField] private Button _button;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private GameObject _image;

        public PayoutType PayoutType => _payoutType;
        public Button Button => _button;
        public TextMeshProUGUI Text => _text;
        public GameObject Image => _image;

        private void Reset()
        {
            _button = GetComponent<Button>();
        }

        private void OnValidate()
        {
            if (_button == null)
                _button = GetComponent<Button>();
        }
    }
}
