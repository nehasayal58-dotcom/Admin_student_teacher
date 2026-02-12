using Admin_Student_Teacher.Models;

namespace Admin_Student_Teacher.Data.Repositories
{
    public interface IStudentRepository
    {
        int? GetRollNoByUserId(string userId);
        void UpdateRollNo(string userId, int? rollNo);
        bool RollNoExists(int rollNo, string userId);

        //
        StudentVM GetStudentProfile(string studentId);
        //IEnumerable<Subjects> GetAllSubjects();
        //void AssignSubjectToStudent(string studentId, int subjectId);

        //Student Name
        string GetStudentName(string studentId);
        public IEnumerable<StudentMarksVM> GetStudentMarks(string studentId);

    }
}
  