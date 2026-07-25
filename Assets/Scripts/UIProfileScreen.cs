using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIProfileScreen : MonoBehaviour
{
    [SerializeField] private TMP_InputField _nameText;
    [SerializeField] private TMP_InputField _surnameText;
    [SerializeField] private TMP_InputField _emailText;
    [SerializeField] private TMP_InputField _phoneText;
    [SerializeField] private Button _saveButton;
    [SerializeField] private Button _deleteButton;
    [SerializeField] private Button _closeButton;
    [SerializeField] private UICommonWindow _commonWindow;
    [SerializeField] private Button _inviteButton;
    [SerializeField] private UIReferrerScreen _referralScreen;

    public void Init(ApplicationController appController, ReferrersData referrersData)
    {
        _saveButton.onClick.AddListener(() =>
        {
            _saveButton.interactable = false;
            appController.UpdateProfile(_nameText.text, _surnameText.text, _emailText.text, _phoneText.text);
        });

        _inviteButton.onClick.AddListener(() =>
        {
            appController.ChangePanel(_referralScreen.gameObject);
            gameObject.SetActive(false);
        });

        _deleteButton.onClick.AddListener(() =>
        {
            var desc = LocalizationManager.GetTranslation("Profile/DeleteAccountPopup/Desc");
            var acceptButtonText = LocalizationManager.GetTranslation("Profile/DeleteAccountPopup/AcceptButton");
            var declineButtonText = LocalizationManager.GetTranslation("Profile/DeleteAccountPopup/DeclineButton");

            _commonWindow.Show(appController.DeleteAccount, null, desc, string.Empty, acceptButtonText, declineButtonText);
        });

        _closeButton.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });

        _nameText.text = appController.SettingsData.Name;
        _surnameText.text = appController.SettingsData.Surname;
        _emailText.text = !string.IsNullOrEmpty(appController.SettingsData.Email) ? appController.SettingsData.Email : "";
        _phoneText.text = appController.SettingsData.PhoneNumber != "0" ? $"+{appController.SettingsData.PhoneNumber}" : "";

        _nameText.onValueChanged.AddListener((text) => { _saveButton.interactable = true; });
        _surnameText.onValueChanged.AddListener((text) => { _saveButton.interactable = true; });
        _emailText.onValueChanged.AddListener((text) => { _saveButton.interactable = true; });
        _phoneText.onValueChanged.AddListener((text) => { _saveButton.interactable = true; });

        _referralScreen.Show(appController, referrersData);
    }
}
