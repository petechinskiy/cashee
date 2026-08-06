using System;
namespace io.adjoe.sdk
 {
    [System.Serializable]
     public class PlaytimeUserProfile
     {
        internal PlaytimeGender Gender;
        internal DateTime Birthday;

        // Serializable string properties
        [UnityEngine.SerializeField]
        private string gender;
        [UnityEngine.SerializeField]
        private string birthday;

         public PlaytimeUserProfile SetGender(PlaytimeGender val)
         {
             this.Gender = val;
             switch (val)
             {
                case PlaytimeGender.MALE:
                    this.gender = "male";
                    break;
                case PlaytimeGender.FEMALE:
                    this.gender = "female";
                    break;
                case PlaytimeGender.UNKNOWN:
                    this.gender = "unknown";
                    break;
             }
             return this;
         }

         public PlaytimeUserProfile SetBirthday(DateTime val)
         {
             this.Birthday = val;
             this.birthday = val.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
             return this;
         }
     }
 }