using System;
namespace io.adjoe.sdk
 {
     public class AdjoeUserProfile
     {
         internal AdjoeGender gender;
         internal DateTime birthday;

         public AdjoeUserProfile SetGender(AdjoeGender val)
         {
             this.gender = val;
             return this;
         }

         public AdjoeUserProfile SetBirthday(DateTime val)
         {
             this.birthday = val;
             return this;
         }
     }
 }