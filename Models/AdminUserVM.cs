using Microsoft.AspNetCore.Mvc.Rendering;

namespace Admin_Student_Teacher.Models
{
    public class AdminUserVM
    {
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public string? Role { get; set; }
        public string? Subjects { get; set; }
        public int? SubjectId { get; set; }
        public string SubjectName { get; set; }
        public  int ? RollNo { get; set; }

        public decimal? Percentage { get; set; } // for total percentage

        public string SubjectCount { get; set; }
        public string StudentId { get; set; }

      
    }
}
