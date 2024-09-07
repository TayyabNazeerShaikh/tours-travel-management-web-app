using Microsoft.AspNetCore.Identity;

namespace ToursAndTravelsManagement.Models
{
    public class User : IdentityUser
    {
        // Additional properties specific to your application
        public string Name { get; set; }
        public string Role { get; set; }
    }
}