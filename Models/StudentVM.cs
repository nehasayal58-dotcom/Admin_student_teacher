namespace Admin_Student_Teacher.Models
{
    public class StudentVM
    {
        //public string UserId { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        // Student-specific
        public int? RollNo { get; set; }
        public string StudentId { get; set; }
        public string SubjectName { get; set; }
        public int ?Marks { get; set; }
        public int TotalSubjects { get; set; }

        public string Name { get; set; }
    }
}
