namespace Admin_Student_Teacher.Models
{
    public class AssignMarksVM
    {
        public string StudentId { get; set; }
        public string StudentName { get; set; }

        public int SubjectId { get; set; } //hidden
        public int Marks { get; set; }

        public List<Subjects> Subjects { get; set; }


    }
}
