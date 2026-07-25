using System;
using System.Collections;
using UnityEngine;

namespace io.adjoe.sdk
{
    public class AdjoeStreakInfo
    {
        private AndroidJavaObject streakInfo;

        internal AdjoeStreakInfo(AndroidJavaObject streakInfo)
        {
            this.streakInfo = streakInfo;
        }

        public int GetLastAchievedDay()
        {
            return streakInfo.Call<int>("getLastAchievedDay");
        }

        public int IsFailed() 
        {
            return streakInfo.Call<int>("isFailed");
        }

        public ArrayList GetCoinSettings()
        {
            AndroidJavaObject javaCoinSettings = streakInfo.Call<AndroidJavaObject>("getCoinSettings");
            int size = javaCoinSettings.Call<int>("size");
            ArrayList coinSettings = new ArrayList(size);
            for (int i = 0; i < size; i++)
            {
                AndroidJavaObject javaCoinSetting = javaCoinSettings.Call<AndroidJavaObject>("get", i);
                AdjoeCoinSetting coinSetting = new AdjoeCoinSetting(javaCoinSetting);
                coinSettings.Add(coinSetting);
            }
            return coinSettings;
        }


        public string StringLog() {
            return "AdjoeStreakInfo: { \n" +
            "\t LastAchievedDay: " + GetLastAchievedDay() +
            "\t IsFailed: " + IsFailed() +
            "\t coinSettingsSize: " + GetCoinSettings().Count
            ;
        }

    }
}