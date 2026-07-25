using System;
namespace io.adjoe.sdk
{
    /// <summary>
    /// Playtime Extension class holds additional optional subids to be used by publisher
    /// to pass different values
    /// </summary>
    public class PlaytimeExtensions
    {
        internal string subId1;
        internal string subId2;
        internal string subId3;
        internal string subId4;
        internal string subId5;

        public PlaytimeExtensions setSubId1(string subId1)
        {
            this.subId1 = subId1;
            return this;
        }

        public PlaytimeExtensions setSubId2(string subId2)
        {
            this.subId2 = subId2;
            return this;
        }

        public PlaytimeExtensions setSubId3(string subId3)
        {
            this.subId3 = subId3;
            return this;
        }

        public PlaytimeExtensions setSubId4(string subId4)
        {
            this.subId4 = subId4;
            return this;
        }

        public PlaytimeExtensions setSubId5(string subId5)
        {
            this.subId5 = subId5;
            return this;
        }
    }
}
