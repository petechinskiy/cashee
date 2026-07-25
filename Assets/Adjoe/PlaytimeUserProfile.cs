using System;
namespace io.adjoe.sdk
 {
     public class PlaytimeUserProfile
     {
         internal PlaytimeGender gender;
         internal DateTime birthday;

         public PlaytimeUserProfile SetGender(PlaytimeGender val)
         {
             this.gender = val;
             return this;
         }

         public PlaytimeUserProfile SetBirthday(DateTime val)
         {
             this.birthday = val;
             return this;
         }
     }
 }