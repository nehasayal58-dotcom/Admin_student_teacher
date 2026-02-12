using Admin_Student_Teacher.Models;

namespace Admin_Student_Teacher.Data.Repositories
{
    public interface ITeacherRepository
    {
        TeacherProfileVM GetMyProfile(string teacherId);
        IEnumerable<StudentVM> GetMyStudents(string teacherId);
        List<Subjects> GetTeacherSubjects(string teacherId);
        bool IsSubjectAssignedToTeacher(string teacherId, int subjectId);
        //get teacher assigned subject id
        int GetTeacherSubjectId(string teacherId);

    }
}
