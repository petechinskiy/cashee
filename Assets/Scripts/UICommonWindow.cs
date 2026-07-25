using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UICommonWindow : MonoBehaviour
{
    [SerializeField] private Button _acceptBtn;
    [SerializeField] private Button _declineBtn;
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private TextMeshProUGUI _description;
    [SerializeField] private TextMeshProUGUI _acceptButtonText;
    [SerializeField] private TextMeshProUGUI _declineButtonText;

    private Action _onAccept;
    private Action _onDecline;

    private void Awake()
    {
        _acceptBtn?.onClick.AddListener(() =>
        {
            _onAccept?.Invoke();
        });
        _declineBtn?.onClick.AddListener(() =>
        {
            _onDecline?.Invoke();
        });
    }

    public void Show(Action onAccept, Action onDecline, string title, string description = null, string acceptButtonText = null, string declineButtonText = null)
    {
        _onAccept = onAccept;
        _onDecline = onDecline;
        _title.text = title;

        if (_description)
        {
            _description.text = description;
        }
        if (_acceptBtn)
        {
            if (!string.IsNullOrEmpty(acceptButtonText))
                _acceptButtonText.text = acceptButtonText;
        }
        if (_declineBtn)
        {
            if (!string.IsNullOrEmpty(declineButtonText))
                _declineButtonText.text = declineButtonText;
        }

        _title.gameObject.SetActive(!string.IsNullOrEmpty(title));

        gameObject.SetActive(true);
    }

    public void Show(string description)
    {
        _description.text = description;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}