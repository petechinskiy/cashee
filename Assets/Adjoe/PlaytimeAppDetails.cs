using System;
using System.Collections;
using UnityEngine;

namespace io.adjoe.sdk
{
    /// <summary>
    /// This class represents an entry from adjoe's PartnerApp details.
    /// </summary>

    public class PlaytimeAppDetails
    {
        private AndroidJavaObject appDetails;

        internal PlaytimeAppDetails(AndroidJavaObject appDetails)
        {
            this.appDetails = appDetails;
        }
        

        public string GetPlatform() {
        return appDetails.Call<string>("getPlatform");
        }

        public int GetAndroidVersion() {
            return appDetails.Call<int>("getAndroidVersion");
        }

        public string GetRating() {
            return appDetails.Call<string>("getRating");
        }

        public string GetNumOfRatings() {
            return appDetails.Call<string>("getNumOfRatings");
        }

        public string GetSize() {
            return appDetails.Call<string>("getSize");
        }

        public string GetInstalls() {
            return appDetails.Call<string>("getInstalls");
        }

        public string GetAgeRating() {
            return appDetails.Call<string>("getAgeRating");
        }

        public string GetCategory() {
            return appDetails.Call<string>("getCategory");
        }

        public ArrayList GetCategoryTranslations() {
            AndroidJavaObject javaCategoryTranslationList = appDetails.Call<AndroidJavaObject>("getCategoryTranslations");
            if (javaCategoryTranslationList == null)
            {
                return new ArrayList();
            }
            int size = javaCategoryTranslationList.Call<int>("size");
            ArrayList categoryTranslations = new ArrayList(size);
            for (int i = 0; i < size; i++)
            {
                AndroidJavaObject javaCategoryTranslation = javaCategoryTranslationList.Call<AndroidJavaObject>("get", i);
                PlaytimeCategoryTranslation categoryTranslation = new PlaytimeCategoryTranslation(javaCategoryTranslation);
                categoryTranslations.Add(categoryTranslation);
            }
            
            return categoryTranslations;
        }

        public bool getHasInAppPurchases() {
            return appDetails.Call<bool>("getHasInAppPurchases");
        }

        override public string ToString() {
            return string.Format(
							"\tpartner app details:\n" +
								"\t- platform: " + GetPlatform() + "\n" +
								"\t- rating: " + GetRating() + "\n" +
								"\t- size: " + GetSize() + "\n" +
								"\t- category: " + GetCategory() + "\n" +
                                "\t- translation size: " + GetCategoryTranslations().Count + "\n" +
							"--------------------------------------------"
						);
        }
    }
}