using System;
using System.Collections;
using UnityEngine;

namespace io.adjoe.sdk
{
    /// <summary>
    /// This class represents an entry from adjoe's AppDetails Category Translation.
    /// </summary>

    class AdjoeCategoryTranslation
    {
        private AndroidJavaObject categoryTranslation;
        internal AdjoeCategoryTranslation(AndroidJavaObject categoryTranslation)
        {
            this.categoryTranslation = categoryTranslation;
        }

        public string getName() {
            return categoryTranslation.Call<string>("getName");
        }

        public string getLanguage() {
            return categoryTranslation.Call<string>("getLanguage");
        }


    }
}