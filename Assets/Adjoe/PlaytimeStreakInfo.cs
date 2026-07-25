using System;
using System.Collections;
using UnityEngine;

namespace io.adjoe.sdk
{
    public class PlaytimeStreakInfo
    {
        private AndroidJavaObject streakInfo;

        internal PlaytimeStreakInfo(AndroidJavaObject streakInfo)
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
                PlaytimeCoinSetting coinSetting = new PlaytimeCoinSetting(javaCoinSetting);
                coinSettings.Add(coinSetting);
            }
            return coinSettings;
        }


        public string StringLog() {
            return "PlaytimeStreakInfo: { \n" +
            "\t LastAchievedDay: " + GetLastAchievedDay() +
            "\t IsFailed: " + IsFailed() +
            "\t coinSettingsSize: " + GetCoinSettings().Count
            ;
        }

    }
}