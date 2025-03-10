namespace ABK_People_BackEnd.Models
{
    public class Admin : User
    {
        public override string Role { get; set; } = "Admin";
    }
}
