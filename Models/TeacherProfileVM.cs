namespace Admin_Student_Teacher.Models
{
    public class TeacherProfileVM
    {
        //public string UserId { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

       //  Subject assigned to teacher
        public int? SubjectId { get; set; }
        public string SubjectName { get; set; }
    }
}
