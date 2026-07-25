using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserRegistrationInfo
{
    public bool IsMale;
    public int Age;
    public string ReferrerCode;
} 

public class UIRegistrationScreen : MonoBehaviour
{
    [SerializeField] private GameObject _welcomePanel;
    [SerializeField] private Button _welcomeButton;

    [SerializeField] private GameObject _signInPanel;
    [SerializeField] private Button _signInButton;
    [SerializeField] private GameObject _loadingScreen;
    [SerializeField] private TextMeshProUGUI _reffererDescText;
    [SerializeField] private TMP_InputField _referrerCodeInput;
    [Space()]
    [SerializeField] private GameObject _privacyWindow;
    [SerializeField] private Button _privacyAcceptButton;
    [SerializeField] private GameObject _bioSelectionScreen;
    [SerializeField] private Toggle _selectBioMale;
    [SerializeField] private Toggle _selectBioFamale;
    [SerializeField] private Button _bioContinueButton;
    [Space()]
    [SerializeField] private GameObject _ageSelectionScreen;
    [SerializeField] private TMP_InputField _ageInputField;
    [SerializeField] private Button _ageContinueButton;
    [SerializeField] private Color _inputFieldSelectionColor;
    [Space()]
    [SerializeField] private Button[] _termsButtons;

    private Action _onLogin;

    public void Show(Action<UserRegistrationInfo> onRegistration, Action onLogin, bool needRegistration, bool showRefCodeInput)
    {
        var registrationInfo = new UserRegistrationInfo();

        _onLogin = onLogin;

        _welcomePanel.SetActive(needRegistration);
        _signInPanel.SetActive(!needRegistration);
        _loadingScreen.SetActive(false);

        _privacyAcceptButton.onClick.AddListener(() =>
        {
            registrationInfo.Age = 0; //int.Parse(_ageInputField.text);
            registrationInfo.ReferrerCode = _referrerCodeInput.text;

            _loadingScreen.SetActive(true);

            onRegistration.Invoke(registrationInfo);
        });

        _welcomeButton.onClick.AddListener(() =>
        {
            _privacyWindow.gameObject.SetActive(true);
            // => _bioSelectionScreen.SetActive(true), () => { _privacyWindow.Hide(); _signInButton.interactable = true; }, desc, string.Empty, acceptButtonText, declineButtonText);
        });

        _signInButton.onClick.AddListener(() =>
        {
            _signInButton.interactable = false;
            Login();
        });

        _selectBioMale.onValueChanged.AddListener((isOn) =>
        {
            registrationInfo.IsMale = isOn;
            _selectBioMale.interactable = !isOn;
            _bioContinueButton.interactable = true;
            _bioContinueButton.GetComponent<UINavigationButton>().SetSelect(true);
        });

        _selectBioFamale.onValueChanged.AddListener((isOn) =>
        {
            registrationInfo.IsMale = !isOn;
            _selectBioFamale.interactable = !isOn;
            _bioContinueButton.interactable = true;
            _bioContinueButton.GetComponent<UINavigationButton>().SetSelect(true);
        });

        _bioContinueButton.onClick.AddListener(() =>
        {
            _bioContinueButton.interactable = false;
            _bioContinueButton.GetComponent<UINavigationButton>().SetSelect(false);
            _bioSelectionScreen.SetActive(false);
            _ageSelectionScreen.SetActive(true);
        });

        _ageInputField.onValueChanged.AddListener((value) =>
        {
            bool canContinue = !string.IsNullOrEmpty(value) && int.Parse(value) > 0;

            _ageContinueButton.interactable = canContinue;
            _ageContinueButton.GetComponent<UINavigationButton>().SetSelect(canContinue);
            _ageInputField.GetComponent<Image>().color = _inputFieldSelectionColor;
        });

        _referrerCodeInput.transform.parent.gameObject.SetActive(showRefCodeInput);

        if (showRefCodeInput)
        {
            _referrerCodeInput.onSelect.AddListener((value) =>
            {
                if (_reffererDescText.gameObject.activeSelf)
                {
                    _referrerCodeInput.placeholder.gameObject.SetActive(false);
                    _reffererDescText.gameObject.SetActive(false);

                    var rectTr = _referrerCodeInput.GetComponent<RectTransform>();
                    rectTr.DOSizeDelta(new Vector2(850f, rectTr.sizeDelta.y), 0.7f);
                }
            });
        }

        _ageContinueButton.onClick.AddListener(() =>
        {
            registrationInfo.Age = int.Parse(_ageInputField.text);
            registrationInfo.ReferrerCode = _referrerCodeInput.text;

            _ageContinueButton.interactable = false;
            _ageContinueButton.GetComponent<UINavigationButton>().SetSelect(false);
            _loadingScreen.SetActive(true);

            onRegistration.Invoke(registrationInfo);
        });

        foreach (var btn in _termsButtons)
        {
            btn.onClick.AddListener(() => Application.OpenURL("https://plus-games.com/cashee-terms"));
        }
    }

    public void Login()
    {
        _onLogin.Invoke();
        _loadingScreen.SetActive(true);
    }
}
