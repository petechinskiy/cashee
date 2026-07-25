using I2.Loc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class CashbackAppData
{
    public string AppId;
    public List<CashbackRewardData> Rewards;
}

[Serializable]
public class CashbackRewardData
{
    public float Coins;
    public int Percent;
    public string RewardTerm;
}

[Serializable]
public class CashbackData
{
    public List<CashbackAppData> Slots;
}

public class UICashbackScreen : MonoBehaviour
{
    [SerializeField] private Button _adjoeButton;
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _descriptionText;

    private void Awake()
    {
        var appController = FindFirstObjectByType<ApplicationController>();
        var adjoeController = FindFirstObjectByType<AdjoeController>();

        _adjoeButton.gameObject.SetActive(appController.SettingsData.AdjoeEnabled);

        _adjoeButton.onClick.AddListener(() =>
        {
            if (appController.SettingsData.AdjoeEnabled)
            {
                adjoeController.ShowOfferwall();
            }
        });

        var titleLocalization = LocalizationManager.GetTranslation("CashbackOffers/Title");
        var descLocalization = LocalizationManager.GetTranslation("CashbackOffers/HowItWork/Desc");

        _titleText.text = string.Format(titleLocalization, appController.SettingsData.SpecialOfferCoins);
        _descriptionText.text = string.Format(descLocalization, appController.SettingsData.SpecialOfferCoins);
    }
}
