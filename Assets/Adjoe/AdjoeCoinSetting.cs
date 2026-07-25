using System;
using UnityEngine;

namespace io.adjoe.sdk
{

    public class AdjoeCoinSetting
    {
        private AndroidJavaObject coinSetting;

        internal AdjoeCoinSetting(AndroidJavaObject coinSetting)
        {
            this.coinSetting = coinSetting;
        }

        public int GetDay()
        {
            return coinSetting.Call<int>("getDay");
        }

        public int GetCoins()
        {
            return coinSetting.Call<int>("getCoins");
        }

        override public string ToString() {
            return "day: "+ GetDay() +" > Coins: "+ GetCoins();
        }
    }
}