using System;
using UnityEngine;

namespace io.adjoe.sdk.studio
{
    /// <summary>
    /// The model that represents a campaign and/or installed application.
    /// </summary>
    [System.Serializable]
    public class PlaytimeCampaign
    {
        [SerializeField] private string appName;
        [SerializeField] private string appDescription;
        [SerializeField] private string appID;
        [SerializeField] private string appBundleID;
        [SerializeField] private string appStoreID;
        [SerializeField] private string installedAt;
        [SerializeField] private string uninstalledAt;
        [SerializeField] private int rewardingExpiresAfter;
        [SerializeField] private string rewardingExpiresAt;
        [SerializeField] private string campaignExpiresAt;
        [SerializeField] private PlaytimeEventConfig eventConfig;
        [SerializeField] private string appCategory;
        [SerializeField] private string campaignType;
        [SerializeField] private int featuredPosition;
        [SerializeField] private float score;
        [SerializeField] private PlaytimeMedia image;
        [SerializeField] private PlaytimeMedia video;
        [SerializeField] private string iconImage;
        [SerializeField] private PlaytimePromotion promotion;
        [SerializeField] private bool isCompleted;
        [SerializeField] private string campaignUUID;
        [SerializeField] private PlaytimeCampaignStatus status;
        [SerializeField] private bool isCpa;
        [SerializeField] private float cpa;

        /// <summary>
        /// App name.
        /// </summary>
        public string AppName => appName;
        
        /// <summary>
        /// Short description of the app.
        /// </summary>
        public string AppDescription => appDescription;
        
        /// <summary>
        /// Unique identifier for the app.
        /// </summary>
        public string AppID => appID;
        
        /// <summary>
        /// ID in the format `com.example.myapp`.
        /// </summary>
        public string? AppBundleID => appBundleID;
        
        /// <summary>
        /// Application's store ID number that is part of the App Store link.
        /// </summary>
        public string? AppStoreID => appStoreID;
        
        /// <summary>
        /// App installation timestamp (ISO 8601).
        /// </summary>
        public string? InstalledAt => installedAt;
        
        /// <summary>
        /// App uninstallation timestamp (ISO 8601).
        /// </summary>
        public string? UninstalledAt => uninstalledAt;
        
        /// <summary>
        /// Time (in days) after which rewards expire after installation.
        /// </summary>
        public int? RewardingExpiresAfter => rewardingExpiresAfter;
        
        /// <summary>
        /// Expiration timestamp (ISO 8601) denoting how long players can get rewards after installing the app.
        /// </summary>
        public string? RewardingExpiresAt => rewardingExpiresAt;
        
        /// <summary>
        /// Timestamp (ISO 8601) denoting the maximum amount of time after fetching the campaign that it can be installed.
        /// </summary>
        public string? CampaignExpiresAt => campaignExpiresAt;
        
        /// <summary>
        /// Event and rewards configuration.
        /// </summary>
        public PlaytimeEventConfig? EventConfig => eventConfig;
        
        /// <summary>
        /// App category.
        /// </summary>
        public string? AppCategory => appCategory;
        
        /// <summary>
        /// Campaign type.
        /// </summary>
        public string? CampaignType => campaignType;
        
        /// <summary>
        /// Ordered position for specific featured campaigns.
        /// </summary>
        public int? FeaturedPosition => featuredPosition;
        
        /// <summary>
        /// eCPM of the campaign.
        /// </summary>
        public float? Score => score;
        
        /// <summary>
        /// Campaign media assets (portrait & landscape).
        /// </summary>
        public PlaytimeMedia? Image => image;
        
        /// <summary>
        /// Campaign video media assets.
        /// </summary>
        public PlaytimeMedia? Video => video;
        
        /// <summary>
        /// URL to the campaign icon image.
        /// </summary>
        public string? IconImage => iconImage;
        
        /// <summary>
        /// Active promotion details, if any.
        /// </summary>
        public PlaytimePromotion? Promotion => promotion;
        
        /// <summary>
        /// Flag indicating whether all rewards have been collected.
        /// </summary>
        public bool IsCompleted => isCompleted;
        
        /// <summary>
        /// Campaign UUID.
        /// </summary>
        public string? CampaignUUID => campaignUUID;

        /// <summary>
        /// Current status of the campaign.
        /// </summary>
        public PlaytimeCampaignStatus Status => status;

        /// <summary>
        /// Flag indicating whether CPA is enabled.
        /// </summary>
        public bool IsCpa => isCpa;

        /// <summary>
        /// CPA of the campaign.
        /// </summary>
        public float Cpa => cpa;

        public PlaytimeCampaign(string appName, string appDescription, string appID, string? appBundleID, string? appStoreID,
                               string? installedAt, string? uninstalledAt, int? rewardingExpiresAfter, string? rewardingExpiresAt,
                               string? campaignExpiresAt, PlaytimeEventConfig? eventConfig, string? appCategory, string? campaignType,
                               int? featuredPosition, float? score, PlaytimeMedia? image, PlaytimeMedia? video, string? iconImage,
                               PlaytimePromotion? promotion, bool isCompleted, string? campaignUUID, PlaytimeCampaignStatus status, bool isCpa, float? cpa)
        {
            this.appName = appName;
            this.appDescription = appDescription;
            this.appID = appID;
            this.appBundleID = appBundleID;
            this.appStoreID = appStoreID;
            this.installedAt = installedAt;
            this.uninstalledAt = uninstalledAt;
            this.rewardingExpiresAfter = rewardingExpiresAfter ?? 0;
            this.rewardingExpiresAt = rewardingExpiresAt;
            this.campaignExpiresAt = campaignExpiresAt;
            this.eventConfig = eventConfig;
            this.appCategory = appCategory;
            this.campaignType = campaignType;
            this.featuredPosition = featuredPosition ?? 0;
            this.score = score ?? 0f;
            this.image = image;
            this.video = video;
            this.iconImage = iconImage;
            this.promotion = promotion;
            this.isCompleted = isCompleted;
            this.campaignUUID = campaignUUID;
            this.status = status;
            this.isCpa = isCpa;
            this.cpa = cpa ?? 0f;
        }

        public PlaytimeCampaign(AndroidJavaObject campaign)
        {
            this.appName = campaign.Call<string>("getAppName");
            this.appDescription = campaign.Call<string>("getAppDescription");
            this.appID = campaign.Call<string>("getAppID");
            this.installedAt = campaign.Call<string>("getInstalledAt");
            this.uninstalledAt = campaign.Call<string>("getUninstalledAt");
            this.appCategory = campaign.Call<string>("getAppCategory");
            this.rewardingExpiresAt = campaign.Call<string>("getRewardingExpiresAt");
            this.campaignExpiresAt = campaign.Call<string>("getCampaignExpiresAt");
            this.campaignType = campaign.Call<string>("getCampaignType");
            this.iconImage = campaign.Call<string>("getIconImage");
            this.isCompleted = campaign.Call<bool>("isCompleted");
            this.campaignUUID = campaign.Call<string>("getCampaignUUID");
            this.isCpa = campaign.Call<bool>("isCpa");
            this.cpa = campaign.Call<float>("getCpa");

            AndroidJavaObject imageJava = campaign.Call<AndroidJavaObject>("getImage");
            AndroidJavaObject videoJava = campaign.Call<AndroidJavaObject>("getVideo");
            AndroidJavaObject promoitonJava = campaign.Call<AndroidJavaObject>("getPromotion");
            AndroidJavaObject eventConfigJava = campaign.Call<AndroidJavaObject>("getEventConfig");
            AndroidJavaObject rewardingExpiresAfterJava = campaign.Call<AndroidJavaObject>("getRewardingExpiresAfter");
            AndroidJavaObject featuredPositionJava = campaign.Call<AndroidJavaObject>("getFeaturedPosition");
            AndroidJavaObject scoreJava = campaign.Call<AndroidJavaObject>("getScore");
            AndroidJavaObject statusJava = campaign.Call<AndroidJavaObject>("getStatus");

            if (rewardingExpiresAfterJava != null)
            {
                this.rewardingExpiresAfter = rewardingExpiresAfterJava.Call<int>("intValue");
            }

            if (featuredPositionJava != null)
            {
                this.featuredPosition = featuredPositionJava.Call<int>("intValue");
            }

            if (scoreJava != null)
            {
                this.score = scoreJava.Call<float>("floatValue");
            }

            if (imageJava != null)
            {
                this.image = new PlaytimeMedia(imageJava);
            }

            if (videoJava != null)
            {
                this.video = new PlaytimeMedia(videoJava);
            }

            if (promoitonJava != null)
            {
                this.promotion = new PlaytimePromotion(promoitonJava);
            }

            if (eventConfigJava != null)
            {
                this.eventConfig = new PlaytimeEventConfig(eventConfigJava);
            }

            if (statusJava != null)
            {
                string statusStr = statusJava.Call<string>("name");
                if (Enum.TryParse(statusStr, out PlaytimeCampaignStatus parsedStatus))
                {
                    this.status = parsedStatus;
                }
            }
        }
    }
} 
