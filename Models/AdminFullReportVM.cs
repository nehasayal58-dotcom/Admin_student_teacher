namespace Admin_Student_Teacher.Models
{
    public class AdminFullReportVM
    {
        public string SchoolName { get; set; }
        public string Address { get; set; }
        public string Contact { get; set; }

        public int TotalTeachers { get; set; }
        public int TotalStudents { get; set; }
        public int TotalSubjects { get; set; }
        public int TotalUsers { get; set; }

        public List<TeacherVM> Teachers { get; set; }
        public List<StudentVM> Students { get; set; }
        public List<UserVM> Users { get; set; }
    }
}
