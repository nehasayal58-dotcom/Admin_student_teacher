using Admin_Student_Teacher.Models;
using Dapper;

namespace Admin_Student_Teacher.Data.Repositories
{
    public class StudentSubjectRepository:IStudentSubjectRepository
    {
        private readonly DapperContext _context;

        public StudentSubjectRepository(DapperContext context)
        {
            _context = context;
        }

        public IEnumerable<Subjects> GetSubjects()
        {
            var sql = "SELECT * FROM Subjects";
            using var conn = _context.CreateConnection();
            return conn.Query<Subjects>(sql);
        }

        public void AssignSubjectToStudent(StudentSubjectAssignVM model)
        {
            var sql = @"
        INSERT INTO StudentSubjects (StudentId, SubjectId)
        VALUES (@StudentId, @SubjectId)";

            using var conn = _context.CreateConnection();
            conn.Execute(sql, model);
        }
    }
}
