using System;
namespace io.adjoe.sdk
 {
    [System.Serializable]
     public class PlaytimeParams
     {
        [UnityEngine.SerializeField]
        internal string uaNetwork;
        [UnityEngine.SerializeField]
        internal string uaChannel;
        [UnityEngine.SerializeField]
        internal string uaSubPublisherEncrypted;
        [UnityEngine.SerializeField]
        internal string uaSubPublisherCleartext;
        [UnityEngine.SerializeField]
        internal string placement;
        [UnityEngine.SerializeField]
        internal string promotionTag;

         public PlaytimeParams SetUaNetwork(string val)
         {
             this.uaNetwork = val;
             return this;
         }

         public PlaytimeParams SetUaChannel(string val)
         {
             this.uaChannel = val;
             return this;
         }

         public PlaytimeParams SetUaSubPublisherEncrypted(string val)
         {
             this.uaSubPublisherEncrypted = val;
             return this;
         }

         public PlaytimeParams SetUaSubPublisherCleartext(string val)
         {
             this.uaSubPublisherCleartext = val;
             return this;
         }

         public PlaytimeParams SetPlacement(string val)
         {
             this.placement = val;
             return this;
         }

         public PlaytimeParams SetPromotionTag(string val)
         {
             this.promotionTag = val;
             return this;
         }
     }
     
 }