

using Microsoft.AspNetCore.Identity;

namespace ABK_People_BackEnd.Models
{
    public abstract class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string LastName { get; set; }
        public string? ProfilePicture { get; set; }

        public virtual string Role { get; set; }

        public EmployeePosition Position { get; set; }



        


        public enum EmployeePosition
        {
            ChiefOfficer,
            Officer,
            Manager,
            GeneralManager,
            AssitantGeneralManager,
            SeniorOfficer,
        }

    }
}
