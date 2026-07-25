using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;

public class UIAuthWindow : MonoBehaviour
{
    [SerializeField] private InputField _phoneNumberInput;
    [SerializeField] private InputField _smsCodeInput;
    [SerializeField] private Button _sendButton;
    [SerializeField] private Button _verifyButton;
    [SerializeField] private GameObject _phoneInputParent;
    [SerializeField] private GameObject _waitingImage;
    [SerializeField] private UIVerifyingWindow _verifyingWindow;
    [SerializeField] private InfoPanel _errorWindow;
    [SerializeField] private UnityEvent _onClose;

    private ApplicationController _appController;

    private System.Action<string> _onSuccess;
    private Tween _waitingTween;

    private void Awake()
    {
        _appController = FindObjectOfType<ApplicationController>();

        _waitingTween = _waitingImage.transform.DOLocalRotate(Vector3.forward * 360f, 2f, RotateMode.LocalAxisAdd)
            .SetEase(Ease.Linear)
            .SetLoops(-1)
            .SetUpdate(true)
            .SetAutoKill(false);

        _sendButton.onClick.AddListener(() =>
        {
            if (string.IsNullOrEmpty(_phoneNumberInput.text) || !_phoneNumberInput.text.Contains("+"))
            {
                _errorWindow.Show("Error: enter number in international format");
                return;
            }

            //if (_phoneNumberInput.text != $"{_appController.SettingsData.PhoneNumber}")
            //{
            //    _errorWindow.Show(LocalizationManager.Instance.GetLocalizedText("phone_duplicate_error"));
            //    return;
            //}

            _onSuccess.Invoke(_phoneNumberInput.text);
            _onClose.Invoke();
            return;

            _sendButton.gameObject.SetActive(false);
            _waitingImage.SetActive(true);
            _waitingTween.Restart();

            _phoneNumberInput.DeactivateInputField();
            _smsCodeInput.DeactivateInputField();

            _sendButton.interactable = false;
        });

        _verifyButton.onClick.AddListener(() =>
        {
            if (string.IsNullOrEmpty(_smsCodeInput.text))
            {
                return;
            }

            _phoneNumberInput.DeactivateInputField();
            _smsCodeInput.DeactivateInputField();

            _verifyingWindow.Show(VerificationState.Process, null);
        });
    }

    public void Show(bool showPhoneInput, System.Action<string> onSuccess)
    {
        _onSuccess = onSuccess;
        _sendButton.gameObject.SetActive(true);
        _sendButton.interactable = true;
        _waitingImage.SetActive(false);
        _verifyButton.interactable = false;
        _smsCodeInput.text = string.Empty;
        gameObject.SetActive(true);

        if (!showPhoneInput)
        {
            _phoneNumberInput.text = showPhoneInput ? string.Empty : $"+{_appController.SettingsData.PhoneNumber}";
            _phoneNumberInput.interactable = false;
            //_firebaseManager.Login($"+{_appController.SettingsData.PhoneNumber}", (sent) =>
            //{
            //    _verifyButton.interactable = true;
            //}, () =>
            //{
            //    _verifyButton.interactable = true;

            //    _verifyingWindow.Show(VerificationState.Success, () =>
            //    {
            //        gameObject.SetActive(false);
            //        _onSuccess.Invoke();
            //    });
            //});
        }
    }
}
