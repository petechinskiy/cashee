using System;
using UnityEngine;

namespace io.adjoe.sdk
{
    public class PlaytimePromoEvent
    {
        AndroidJavaObject javaPromoEvent;
        internal PlaytimePromoEvent(AndroidJavaObject javaPromoEvent)
        {
            this.javaPromoEvent = javaPromoEvent;
        }

        public double GetFactor() {
            return javaPromoEvent.Call<double>("getFactor");
        }

        public DateTime? GetStartDate() {
            AndroidJavaObject javaStartDate = javaPromoEvent.Call<AndroidJavaObject>("getStartDate");
            if (javaStartDate == null)
            {
                return null;
            }

            long startDate = javaStartDate.Call<long>("getTime");
            return new DateTime(startDate);
        }

        public DateTime? GetEndDate() {
            AndroidJavaObject javaEndDate = javaPromoEvent.Call<AndroidJavaObject>("getEndDate");
            if (javaEndDate == null)
            {
                return null;
            }

            long endDate = javaEndDate.Call<long>("getTime");
            return new DateTime(endDate);
        }

        public bool IsRunningNow() {
            return javaPromoEvent.Call<bool>("isRunningNow");
        }

        public string StringLog()
        {
            return string.Format(
                "Factor: {0}, \n StartingTime: {1}, \n End Time: {2}",
                GetFactor(), GetStartDate(), GetEndDate()
            );
        }

    }
}