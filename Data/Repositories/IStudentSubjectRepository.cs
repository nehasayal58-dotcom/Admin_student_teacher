using Admin_Student_Teacher.Models;

namespace Admin_Student_Teacher.Data.Repositories
{
    public interface IStudentSubjectRepository
    {
        IEnumerable<Subjects> GetSubjects();
        void AssignSubjectToStudent(StudentSubjectAssignVM model);
    }
}
