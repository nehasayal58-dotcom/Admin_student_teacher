using Microsoft.AspNetCore.Identity;

namespace Admin_Student_Teacher.Models
{
    public class ApplicationUser:IdentityUser
    {
        // Extra fields
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
