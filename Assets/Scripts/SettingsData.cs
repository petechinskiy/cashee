using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SettingsData
{
    public string Name;
    public string Surname;
    public string Email;
    public string PhoneNumber;
    public UserStatus Status;
    public string RegistrationDate;

    public bool AdjoeEnabled;
    public bool MyChipsEnabled;
    public bool PrimeEnabled;
    public bool ConnFromAnotherIP;
    public bool ConnFromSameIP;
    public bool AccessRestricted;
    public int CoinsLimitForRewardedAds; // количество монет, при котором будет доступно вознаграждение только за просмотр рекламы (только для органики) 
    public bool CheckUpdate; // обязательное обновление до последней версии
    public bool PayoutNotify; // уведомление об успешной выплате
    public bool IsOrganic;
    public string CampaignId;
    public string PublisherId;
    public string NetworkName;
    public int SpecialOfferCoins;
    public string CountryCode;
    public bool HasWelcomeBonus;
    public bool AdjoeOpened;
    public bool CanShowMissions;
    public bool MychipsPromo;
    public bool AdjoeForEarnButton;

    public string GetStatus()
    {
        switch (Status)
        {
            case UserStatus.QARecruit:
                return "QA Recruit";
            case UserStatus.TraineeGameTester:
                return "Trainee Game Tester";
            case UserStatus.ApprenticeGameTester:
                return "Apprentice Game Tester";
            case UserStatus.JuniorGameTester:
                return "Junior Game Tester";
            case UserStatus.GameTester:
                return "Game Tester";
            case UserStatus.IntermediateGameTester:
                return "Intermediate Game Tester";
            case UserStatus.SeniorGameTester:
                return "Senior Game Tester";
            case UserStatus.QAAssistant:
                return "QA Assistant";
            case UserStatus.QAIntern:
                return "QA Intern";
            case UserStatus.JuniorQAAnalyst:
                return "Junior QA Analyst";
            case UserStatus.QAAnalyst:
                return "QA Analyst";
            case UserStatus.SeniorQAAnalyst:
                return "Senior QA Analyst";
            case UserStatus.JuniorQAEngineer:
                return "Junior QA Engineer";
            case UserStatus.QAEngineer:
                return "QA Engineer";
            default:
                return Status.ToString();
        }
    }
}

public enum UserStatus
{
    QARecruit,
    TraineeGameTester,
    ApprenticeGameTester,
    JuniorGameTester,
    GameTester,
    IntermediateGameTester,
    SeniorGameTester,
    QAAssistant,
    QAIntern,
    JuniorQAAnalyst,
    QAAnalyst,
    SeniorQAAnalyst,
    JuniorQAEngineer,
    QAEngineer
}
