using System;
using UnityEngine;

namespace io.adjoe.sdk.studio
{
    /// <summary>
    /// Configuration for multi-event rewards.
    /// The class provides detailed information about events and rewards.
    /// </summary>
    [System.Serializable]
    public class PlaytimeEventConfig
    {
        [SerializeField] private PlaytimeRewardAction[] sequentialActions;
        [SerializeField] private PlaytimeRewardAction[] bonusActions;
        [SerializeField] private PlaytimeRewardAction[] timeBasedActions;
        [SerializeField] private PlaytimeRewardAction[] cpaActions;
        [SerializeField] private int totalCoinsCollected;
        [SerializeField] private int totalCoinsPossible;
        [SerializeField] private int totalOriginalCoinsPossible;
        [SerializeField] private PlaytimeCashbackConfig cashbackReward;
        [SerializeField] private PlaytimeRewardActionMultiplier[] multipliersActions;
        [SerializeField] private int secondsToNextLevel;
        [SerializeField] private int totalSequentialCoins;
        [SerializeField] private int totalOriginalSequentialCoins;
        [SerializeField] private int totalBonusCoins;
        [SerializeField] private int totalOriginalBonusCoins;

        /// <summary>
        /// Array of rewarded events that must be completed in the given order.
        /// These will usually be progress events a user can reach in a game or app.
        /// </summary>
        public PlaytimeRewardAction[] SequentialActions => sequentialActions;
        
        /// <summary>
        /// Array of rewarded events that can be completed in any order.
        /// These will be bonus rewards a user should get on top of the other rewards.
        /// </summary>
        public PlaytimeRewardAction[] BonusActions => bonusActions;
        
        /// <summary>
        /// Array of events that are rewarded based on the time played.
        /// </summary>
        public PlaytimeRewardAction[] TimeBasedActions => timeBasedActions;

        /// <summary>
        /// Array of CPA (Cost Per Action) events that reward the user for completing specific in-app actions.
        /// </summary>
        public PlaytimeRewardAction[] CpaActions => cpaActions;
        
        /// <summary>
        /// Total coins collected by the user.
        /// </summary>
        public int TotalCoinsCollected => totalCoinsCollected;
        
        /// <summary>
        /// Maximum possible coins for this config.
        /// </summary>
        public int? TotalCoinsPossible => totalCoinsPossible;

        /// <summary>
        /// Maximum possible coins for this config if there was no promotion.
        /// </summary>
        public int? TotalOriginalCoinsPossible => totalOriginalCoinsPossible;

        /// <summary>
        /// Cashback reward configuration for in-app purchases. A missing value means that the feature is not supported for the campaign or the SDK.
        /// </summary>
        public PlaytimeCashbackConfig? CashbackReward => cashbackReward;

        /// <summary>
        /// Array of events that multiply the rewards.
        /// </summary>
        public PlaytimeRewardActionMultiplier[]? MultipliersActions => multipliersActions;

        /// <summary>
        /// The number of seconds until the user reaches the next level. A missing value means that the information is not available.
        /// </summary>
        public int? SecondsToNextLevel => secondsToNextLevel;

        /// <summary>
        /// Total amount of coins for all sequential events with promotion multiplier.
        /// </summary>
        public int? TotalSequentialCoins => totalSequentialCoins;

        /// <summary>
        /// Total amount of coins for all sequential events without promotion multiplier.
        /// </summary>
        public int? TotalOriginalSequentialCoins => totalOriginalSequentialCoins;

        /// <summary>
        /// Total amount of coins for all bonus events with promotion multiplier.
        /// </summary>
        public int? TotalBonusCoins => totalBonusCoins;

        /// <summary>
        /// Total amount of coins for all bonus events without promotion multiplier.
        /// </summary>
        public int? TotalOriginalBonusCoins => totalOriginalBonusCoins;

        public PlaytimeEventConfig(PlaytimeRewardAction[] sequentialActions, PlaytimeRewardAction[] bonusActions,
                                  PlaytimeRewardAction[] timeBasedActions, PlaytimeRewardAction[] cpaActions, int totalCoinsCollected, int? totalCoinsPossible,
                                  PlaytimeCashbackConfig? cashbackReward, PlaytimeRewardActionMultiplier[]? multipliersActions,
                                  int? totalOriginalCoinsPossible = null, int? secondsToNextLevel = null,
                                  int? totalSequentialCoins = null, int? totalOriginalSequentialCoins = null,
                                  int? totalBonusCoins = null, int? totalOriginalBonusCoins = null)
        {
            this.sequentialActions = sequentialActions;
            this.bonusActions = bonusActions;
            this.timeBasedActions = timeBasedActions;
            this.cpaActions = cpaActions;
            this.totalCoinsCollected = totalCoinsCollected;
            this.totalCoinsPossible = totalCoinsPossible ?? 0;
            this.totalOriginalCoinsPossible = totalOriginalCoinsPossible ?? 0;
            this.cashbackReward = cashbackReward;
            this.multipliersActions = multipliersActions;
            this.secondsToNextLevel = secondsToNextLevel ?? 0;
            this.totalSequentialCoins = totalSequentialCoins ?? 0;
            this.totalOriginalSequentialCoins = totalOriginalSequentialCoins ?? 0;
            this.totalBonusCoins = totalBonusCoins ?? 0;
            this.totalOriginalBonusCoins = totalOriginalBonusCoins ?? 0;
        }
        
        public PlaytimeEventConfig(AndroidJavaObject javaObject)
        {
            AndroidJavaObject javaSequentialActions = javaObject.Call<AndroidJavaObject>("getSequentialActions");
            AndroidJavaObject javaBonusActions = javaObject.Call<AndroidJavaObject>("getBonusActions");
            AndroidJavaObject javaTimeBasedActions = javaObject.Call<AndroidJavaObject>("getTimeBasedActions");
            AndroidJavaObject javaCpaActions = javaObject.Call<AndroidJavaObject>("getCpaActions");
            AndroidJavaObject javaTotalCoinsCollected = javaObject.Call<AndroidJavaObject>("getTotalCoinsCollected");
            AndroidJavaObject javaTotalCoinsPossible = javaObject.Call<AndroidJavaObject>("getTotalCoinsPossible");
            AndroidJavaObject javaTotalOriginalCoinsPossible = javaObject.Call<AndroidJavaObject>("getTotalOriginalCoinsPossible");
            AndroidJavaObject javaCashbackReward = javaObject.Call<AndroidJavaObject>("getCashbackReward");
            AndroidJavaObject javaMultipliersActions = javaObject.Call<AndroidJavaObject>("getMultipliersActions");
            AndroidJavaObject javaSecondsToNextLevel = javaObject.Call<AndroidJavaObject>("getSecondsToNextLevel");
            AndroidJavaObject javaTotalSequentialCoins = javaObject.Call<AndroidJavaObject>("getTotalSequentialCoins");
            AndroidJavaObject javaTotalOriginalSequentialCoins = javaObject.Call<AndroidJavaObject>("getTotalOriginalSequentialCoins");
            AndroidJavaObject javaTotalBonusCoins = javaObject.Call<AndroidJavaObject>("getTotalBonusCoins");
            AndroidJavaObject javaTotalOriginalBonusCoins = javaObject.Call<AndroidJavaObject>("getTotalOriginalBonusCoins");

            if (javaSequentialActions != null)
            {
                int sequentialActionsSize = javaSequentialActions.Call<int>("size");
                sequentialActions = new PlaytimeRewardAction[sequentialActionsSize];

                for (int i = 0; i < sequentialActionsSize; i++) {
                    AndroidJavaObject javaSequentialAction = javaSequentialActions.Call<AndroidJavaObject>("get", i);
                    sequentialActions[i] = new PlaytimeRewardAction(javaSequentialAction);
                }
            }

            if (javaBonusActions != null)
            {
                int bonusActionsSize = javaBonusActions.Call<int>("size");
                bonusActions = new PlaytimeRewardAction[bonusActionsSize];

                for (int i = 0; i < bonusActionsSize; i++) {
                    AndroidJavaObject javaBonusAction = javaBonusActions.Call<AndroidJavaObject>("get", i);
                    bonusActions[i] = new PlaytimeRewardAction(javaBonusAction);
                }
            }

            if (javaTimeBasedActions != null)
            {
                int timeBasedActionsSize = javaTimeBasedActions.Call<int>("size");
                timeBasedActions = new PlaytimeRewardAction[timeBasedActionsSize];

                for (int i = 0; i < timeBasedActionsSize; i++) {
                    AndroidJavaObject javaTimeBasedAction = javaTimeBasedActions.Call<AndroidJavaObject>("get", i);
                    timeBasedActions[i] = new PlaytimeRewardAction(javaTimeBasedAction);
                }
            }

            if (javaCpaActions != null)
            {
                int cpaActionsSize = javaCpaActions.Call<int>("size");
                cpaActions = new PlaytimeRewardAction[cpaActionsSize];

                for (int i = 0; i < cpaActionsSize; i++) {
                    AndroidJavaObject javaCpaAction = javaCpaActions.Call<AndroidJavaObject>("get", i);
                    cpaActions[i] = new PlaytimeRewardAction(javaCpaAction);
                }
            }

            if (javaTotalCoinsCollected != null)
            {
                this.totalCoinsCollected = javaTotalCoinsCollected.Call<int>("intValue");
            }
            
            if (javaTotalCoinsPossible != null)
            {
                this.totalCoinsPossible = javaTotalCoinsPossible.Call<int>("intValue");
            }

            if (javaTotalOriginalCoinsPossible != null)
            {
                this.totalOriginalCoinsPossible = javaTotalOriginalCoinsPossible.Call<int>("intValue");
            }

            if (javaCashbackReward != null)
            {
                this.cashbackReward = new PlaytimeCashbackConfig(javaCashbackReward);
            }

            if (javaMultipliersActions != null)
            {
                int multipliersActionsSize = javaMultipliersActions.Call<int>("size");
                PlaytimeRewardActionMultiplier[] multipliersActions = new PlaytimeRewardActionMultiplier[multipliersActionsSize];

                for (int i = 0; i < multipliersActionsSize; i++) {
                    AndroidJavaObject javaMultipliersAction = javaMultipliersActions.Call<AndroidJavaObject>("get", i);
                    multipliersActions[i] = new PlaytimeRewardActionMultiplier(javaMultipliersAction);
                }
            }

            if (javaSecondsToNextLevel != null)
            {
                this.secondsToNextLevel = javaSecondsToNextLevel.Call<int>("intValue");
            }

            if (javaTotalSequentialCoins != null)
            {
                this.totalSequentialCoins = javaTotalSequentialCoins.Call<int>("intValue");
            }

            if (javaTotalOriginalSequentialCoins != null)
            {
                this.totalOriginalSequentialCoins = javaTotalOriginalSequentialCoins.Call<int>("intValue");
            }

            if (javaTotalBonusCoins != null)
            {
                this.totalBonusCoins = javaTotalBonusCoins.Call<int>("intValue");
            }

            if (javaTotalOriginalBonusCoins != null)
            {
                this.totalOriginalBonusCoins = javaTotalOriginalBonusCoins.Call<int>("intValue");
            }
        }
    }
} 
