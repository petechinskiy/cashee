using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

#if UNITY_ANDROID
/// <summary>
/// Used to convert Playtime objects to JavaObject
/// </summary>
namespace io.adjoe.sdk
{
    public static class ToJavaOptionsConverter
    {
        /// <summary>
        /// Convert PlaytimeOptions to JavaObject
        /// </summary>
        /// <param name="playtimeOptions">The PlaytimeOptions to convert</param>
        /// <returns>JavaObject representation of PlaytimeOptions</returns>
        internal static AndroidJavaObject PlaytimeOptionsToJavaObject(PlaytimeOptions playtimeOptions)
        {
            AndroidJavaObject javaPlaytimeOptions = new AndroidJavaObject("io.adjoe.sdk.PlaytimeOptions");
            
            if (playtimeOptions != null)
            {
                if (playtimeOptions.userId != null) {
                    javaPlaytimeOptions.Call<AndroidJavaObject>("setUserId", new object[] {playtimeOptions.userId});
                }

                if (playtimeOptions.sdkHash != null) {
                    javaPlaytimeOptions.Call<AndroidJavaObject>("setSDKHash", new object[] {playtimeOptions.sdkHash});
                }

                AndroidJavaObject playtimeParams = Playtime.GetJavaPlaytimeParams(playtimeOptions.playtimeParams);

                if (playtimeParams != null) 
                {
                    javaPlaytimeOptions.Call<AndroidJavaObject>("setParams", new object[] { playtimeParams });
                }

                if (playtimeOptions.applicationProcessName != null) 
                {
                    javaPlaytimeOptions.Call<AndroidJavaObject>(
                        "setApplicationProcessName", 
                        new object[] {playtimeOptions.applicationProcessName}
                    );
                }

                if (playtimeOptions.playtimeExtensions != null) 
                {
                    AndroidJavaObject javaPlaytimeExtensions = Playtime.GetJavaPlaytimeExtensions(
                        playtimeOptions.playtimeExtensions
                    );
                    javaPlaytimeOptions.Call<AndroidJavaObject>(
                        "setExtensions", 
                        new object[] { javaPlaytimeExtensions }
                    );
                }
                
                if (playtimeOptions.playtimeUserProfile != null) 
                {
                    AndroidJavaObject javaPlaytimeUserProfile = Playtime.GetJavaPlaytimeUserProfile(
                        playtimeOptions.playtimeUserProfile
                    );
                    javaPlaytimeOptions.Call<AndroidJavaObject>(
                        "setUserProfile", 
                        new object[] { javaPlaytimeUserProfile }
                    );
                }
            }
            
            return javaPlaytimeOptions;
        }
    }
}
#endif
