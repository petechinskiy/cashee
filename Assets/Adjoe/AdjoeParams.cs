using System;
namespace io.adjoe.sdk
 {
     public class AdjoeParams
     {
         internal string uaNetwork;
         internal string uaChannel;
         internal string uaSubPublisherEncrypted;
         internal string uaSubPublisherCleartext;
         internal string placement;


         public AdjoeParams SetUaNetwork(string val)
         {
             this.uaNetwork = val;
             return this;
         }

         public AdjoeParams SetUaChannel(string val)
         {
             this.uaChannel = val;
             return this;
         }

         public AdjoeParams SetUaSubPublisherEncrypted(string val)
         {
             this.uaSubPublisherEncrypted = val;
             return this;
         }

         public AdjoeParams SetUaSubPublisherCleartext(string val)
         {
             this.uaSubPublisherCleartext = val;
             return this;
         }

         public AdjoeParams SetPlacement(string val)
         {
             this.placement = val;
             return this;
         }

     }
     
 }