using System;
namespace io.adjoe.sdk
 {
     public class PlaytimeParams
     {
         internal string uaNetwork;
         internal string uaChannel;
         internal string uaSubPublisherEncrypted;
         internal string uaSubPublisherCleartext;
         internal string placement;


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

     }
     
 }