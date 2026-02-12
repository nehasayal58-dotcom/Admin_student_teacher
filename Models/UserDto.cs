namespace Admin_Student_Teacher.Models
{
    public class UserDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
        // for subjects
        public string Subjects { get; set; }

    }
}
