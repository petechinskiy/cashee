using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIProfilePanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _shortNameText;
    [SerializeField] private TextMeshProUGUI _statusText;
    [SerializeField] private TextMeshProUGUI _dateText;
    [SerializeField] private TextMeshProUGUI _balanceText;
    [SerializeField] private Button _button;
    [SerializeField] private GameObject _placeholder;
    [SerializeField] private GameObject _dataParent;

    private float _currentCoins;
    private float _velocity;
    private ApplicationController _appController;

    private void Awake()
    {
        _appController = FindFirstObjectByType<ApplicationController>();
        var profileScreen = FindFirstObjectByType<UIProfileScreen>(FindObjectsInactive.Include);

        if (_button)
        {
            _button.onClick.AddListener(() => profileScreen.gameObject.SetActive(true));
        }
    }

    private void Update()
    {
        _shortNameText.text = $"{_appController.SettingsData.Name?.FirstOrDefault()}{_appController.SettingsData.Surname?.FirstOrDefault()}";
        _statusText.text = _appController.SettingsData.GetStatus();
        _dateText.text = "Joined " +_appController.SettingsData.RegistrationDate;
    }

    public void UpdateView(int coins)
    {
        if (!_balanceText)
        {
            return;
        }

        _placeholder.SetActive(false);
        _dataParent.SetActive(true);

        if ((int)_currentCoins != coins)
        {
            _currentCoins = Mathf.SmoothDamp(_currentCoins, coins, ref _velocity, 0.5f);
            _balanceText.text = Mathf.RoundToInt(_currentCoins).ToString() + "<sprite name=\"coin_small\">";
        }
    }
}
