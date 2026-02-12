using Admin_Student_Teacher.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Admin_Student_Teacher.Data.Repositories
{
    public class TeacherSubjectRepository:ITeacherSubjectRepository
    {
        private readonly DapperContext _context;

        public TeacherSubjectRepository(DapperContext context)
        {
            _context = context;
        }

        //  Same teacher + same subject
        //public bool ExistsForTeacher(string teacherId, int subjectId)
        //{
        //    var query = @"SELECT COUNT(1)
        //              FROM TeacherSubjects
        //              WHERE TeacherId = @TeacherId
        //                AND SubjectId = @SubjectId";

        //    using var connection = _context.CreateConnection();
        //    return connection.ExecuteScalar<int>(query, new
        //    {
        //        TeacherId = teacherId,
        //        SubjectId = subjectId
        //    }) > 0;
        //}

        public bool TeacherHasSubject(string teacherId)
        {
            var sql = @"SELECT COUNT(1)
                FROM TeacherSubjects
                WHERE TeacherId = @TeacherId";

            using var connection = _context.CreateConnection();
            return connection.ExecuteScalar<int>(sql, new
            {
                TeacherId = teacherId
            }) > 0;
        }

        // One subject → only one teacher
        public bool SubjectAlreadyAssigned(int subjectId , string teacherId)
        {
            var query = @"SELECT COUNT(1)
                      FROM TeacherSubjects
                      WHERE SubjectId = @SubjectId
                      AND TeacherId !=@TeacherId"; 

            using var connection = _context.CreateConnection();
            return connection.ExecuteScalar<int>(query, new
            {
                SubjectId = subjectId,
                TeacherId = teacherId
            }) > 0;
        }
        public bool Assign(TeacherSubject model)
        {
            //if (ExistsForTeacher(model.TeacherId, model.SubjectId))
            //    return false; // rule 1 violated
            if (TeacherHasSubject(model.TeacherId))
                return false;
            if (SubjectAlreadyAssigned(model.SubjectId,model.TeacherId))
                return false; // rule 2 violated

            var query = @"INSERT INTO TeacherSubjects (TeacherId, SubjectId)
                      VALUES (@TeacherId, @SubjectId)";

            using var connection = _context.CreateConnection();
            return connection.Execute(query, model) > 0;
        }

        public IEnumerable<string> GetSubjectsByTeacher(string teacherId)
        {
            var sql = @"SELECT s.SubjectName
                    FROM TeacherSubjects ts
                    INNER JOIN Subjects s ON ts.SubjectId = s.Id
                    WHERE ts.TeacherId = @TeacherId";

            using var connection = _context.CreateConnection();
            return connection.Query<string>(sql, new { TeacherId = teacherId });
        }

        public bool UpdateTeacherSubject(string teacherId, int subjectId)
        {
            using var conn = _context.CreateConnection();
            conn.Open();
            using var tx = conn.BeginTransaction();

            try
            {
                // remove old subject
                conn.Execute(
                    "DELETE FROM TeacherSubjects WHERE TeacherId = @TeacherId",
                    new { TeacherId = teacherId }, tx);

                // assign new subject
                conn.Execute(
                    @"INSERT INTO TeacherSubjects (TeacherId, SubjectId)
              VALUES (@TeacherId, @SubjectId)",
                    new { TeacherId = teacherId, SubjectId = subjectId }, tx);

                tx.Commit();
                return true;
            }
            catch
            {
                tx.Rollback();
                return false;
            }
        }

        public int? GetAssignedSubject(string teacherId)
        {
            var sql = "SELECT SubjectId FROM TeacherSubjects WHERE TeacherId = @TeacherId";

            using var conn = _context.CreateConnection();
            return conn.QueryFirstOrDefault<int?>(sql, new { TeacherId = teacherId });
        }
        // if admin select no subject  for teacher that is to unassigned the subject to teacher 
        public void RemoveByTeacher(string teacherId)
        {
            using var conn = _context.CreateConnection();
            conn.Execute(
                "DELETE FROM TeacherSubjects WHERE TeacherId = @TeacherId",
                new { TeacherId = teacherId });
        }
    }
}
