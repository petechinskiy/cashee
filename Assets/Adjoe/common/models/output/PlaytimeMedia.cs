using System;
using UnityEngine;

namespace io.adjoe.sdk.studio
{
    /// <summary>
    /// A class representing a media item.
    /// </summary>
    [System.Serializable]
    public class PlaytimeMedia
    {
        [SerializeField] private string portrait;
        [SerializeField] private string landscape;

        /// <summary>
        /// URL to portrait-oriented media.
        /// </summary>
        public string? Portrait => portrait;
        
        /// <summary>
        /// URL to landscape-oriented media.
        /// </summary>
        public string? Landscape => landscape;

        public PlaytimeMedia(string? portrait, string? landscape)
        {
            this.portrait = portrait;
            this.landscape = landscape;
        }

        public PlaytimeMedia(AndroidJavaObject javaObject) 
        {
            this.portrait = javaObject.Call<string?>("getPortrait");
            this.landscape = javaObject.Call<string?>("getLandscape");
        }
    }
}
