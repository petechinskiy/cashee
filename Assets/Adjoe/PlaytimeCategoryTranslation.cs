using System;
using System.Collections;
using UnityEngine;

namespace io.adjoe.sdk
{
    /// <summary>
    /// This class represents an entry from adjoe's AppDetails Category Translation.
    /// </summary>

    class PlaytimeCategoryTranslation
    {
        private AndroidJavaObject categoryTranslation;
        internal PlaytimeCategoryTranslation(AndroidJavaObject categoryTranslation)
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