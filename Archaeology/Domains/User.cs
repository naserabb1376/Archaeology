using Microsoft.AspNetCore.Identity;

namespace Domains
{
    public class User : IdentityUser<int>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? TimeOfLogin { get; set; }
        public string Telephone { get; set; }

        public Address Address { get; set; }
    }
    public class UserRole : IdentityRole<int>
    {

    }
}
