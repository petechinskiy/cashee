using System;

namespace io.adjoe.sdk
{
    /// <summary>
    /// Playtime Extension class holds additional optional subids to be used by publisher
    /// to pass different values
    /// </summary>
    [System.Serializable]
    public class PlaytimeExtensions
    {
        [UnityEngine.SerializeField]
        internal string subId1;
        [UnityEngine.SerializeField]
        internal string subId2;
        [UnityEngine.SerializeField]
        internal string subId3;
        [UnityEngine.SerializeField]
        internal string subId4;
        [UnityEngine.SerializeField]
        internal string subId5;

        public PlaytimeExtensions SetSubId1(string subId1)
        {
            this.subId1 = subId1;
            return this;
        }

        public PlaytimeExtensions SetSubId2(string subId2)
        {
            this.subId2 = subId2;
            return this;
        }

        public PlaytimeExtensions SetSubId3(string subId3)
        {
            this.subId3 = subId3;
            return this;
        }

        public PlaytimeExtensions SetSubId4(string subId4)
        {
            this.subId4 = subId4;
            return this;
        }

        public PlaytimeExtensions SetSubId5(string subId5)
        {
            this.subId5 = subId5;
            return this;
        }
    }
}
