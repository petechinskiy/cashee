using System;
using System.Collections;
using System.Text;
using UnityEngine;

namespace io.adjoe.sdk
{
    public class PlaytimeAdvancePlusConfig
    {
        AndroidJavaObject javaAdvancePlusConfig;

        internal PlaytimeAdvancePlusConfig(AndroidJavaObject javaAdvancePlusConfig)
        {
            this.javaAdvancePlusConfig = javaAdvancePlusConfig;
        }

        public int GetTotalCoins() {
            return javaAdvancePlusConfig.Call<int>("getTotalCoins");
        }

        public ArrayList GetSequentialEvents() {
            AndroidJavaObject javaEvents = javaAdvancePlusConfig.Call<AndroidJavaObject>("getSequentialEvents");
            int size = javaEvents.Call<int>("size");
            ArrayList sequentialEvents  = new ArrayList(size);
            for (int i = 0; i < size; i++)
            {
                AndroidJavaObject javaEvent = javaEvents.Call<AndroidJavaObject>("get", i);
                PlaytimeAdvancePlusEvent advancePlusEvent = new PlaytimeAdvancePlusEvent(javaEvent);
                sequentialEvents.Add(advancePlusEvent);
            }
            return sequentialEvents;
        }

        public ArrayList GetBonusEvents() {
            AndroidJavaObject javaEvents = javaAdvancePlusConfig.Call<AndroidJavaObject>("getBonusEvents");
            int size = javaEvents.Call<int>("size");
            ArrayList bonusEvents  = new ArrayList(size);
            for (int i = 0; i < size; i++)
            {
                AndroidJavaObject javaEvent = javaEvents.Call<AndroidJavaObject>("get", i);
                PlaytimeAdvancePlusEvent advancePlusEvent = new PlaytimeAdvancePlusEvent(javaEvent);
                bonusEvents.Add(advancePlusEvent);
            }
            return bonusEvents;
        }

        public string StringLog() 
        {
            Debug.Log("totalCoins:" + GetTotalCoins());
            Debug.Log("sequentialEvents: [");
            ArrayList sequentialEvents = GetSequentialEvents();
            foreach (PlaytimeAdvancePlusEvent seqEvent in sequentialEvents)
            {
                Debug.Log( "\n{" +
                "Name: " + seqEvent.GetName() + 
                        " \nDescription: " + seqEvent.GetDescription() + 
                        "\n coins: " + seqEvent.GetCoins() + 
                        "\n type: " + seqEvent.GetEventType() +
                        "\n rewardedAt: " + seqEvent.GetRewardedAt() +
                "}");

            }
            Debug.Log( "],\n");
            Debug.Log("bonusEvents: [");
            ArrayList bonusEvents = GetBonusEvents();
            foreach (PlaytimeAdvancePlusEvent bonusEvent in bonusEvents)
            {
                Debug.Log("\n{" +
                "Name: " + bonusEvent.GetName() + 
                    " \nDescription: " + bonusEvent.GetDescription() + 
                    "\n coins: " + bonusEvent.GetCoins() + 
                    "\n type: " + bonusEvent.GetEventType() +
                    "\n rewardedAt: " + bonusEvent.GetRewardedAt() +
                "}");
            }
            Debug.Log("]\n");
            return "PlaytimeAdvancePlusConfig";
        }
    }
}