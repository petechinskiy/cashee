using System;
using UnityEngine;

namespace io.adjoe.sdk
{
    public class PlaytimeAdvancePlusEvent 
    {
        private AndroidJavaObject javaAdvancePlusEvent;

        internal PlaytimeAdvancePlusEvent(AndroidJavaObject javaAdvancePlusEvent)
        {
            this.javaAdvancePlusEvent = javaAdvancePlusEvent;
        }


        public string GetName() {
            return javaAdvancePlusEvent.Call<string>("getName");
        }

        public string GetDescription() {
            return javaAdvancePlusEvent.Call<string>("getDescription");
        }

        public int GetCoins() {
            return javaAdvancePlusEvent.Call<int>("getCoins");
        }

        public int GetEventType() {
            return javaAdvancePlusEvent.Call<int>("getType");
        }

        public string GetRewardedAt() {
            return javaAdvancePlusEvent.Call<string>("getRewardedAt");
        }

        public string StringLog() {
            Debug.Log( "Name: " + GetName() + 
                " \nDescription: " + GetDescription() + 
                "\n coins: " + GetCoins() + 
                "\n type: " + GetEventType() +
                "\n rewardedAt: " + GetRewardedAt());
            return "PlaytimeAdvancePlusEvent";
        }
    }
}