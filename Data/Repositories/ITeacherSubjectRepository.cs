using Admin_Student_Teacher.Models;

namespace Admin_Student_Teacher.Data.Repositories
{
    public interface ITeacherSubjectRepository
    {
        bool Assign(TeacherSubject model);
        IEnumerable<string> GetSubjectsByTeacher(string teacherId);

        //bool ExistsForTeacher(string teacherId, int subjectId);  

        bool TeacherHasSubject(string teacherId);
        //bool SubjectAlreadyAssigned(int subjectId);
        bool SubjectAlreadyAssigned(int subjectId, string teacherId);

        // to update teacher subject 
        bool UpdateTeacherSubject(String teacherId, int subjectId);
        int? GetAssignedSubject(string teacherId);
        void RemoveByTeacher(string teacherId);
    }
}
