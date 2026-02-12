using Admin_Student_Teacher.Models;
using Dapper;

namespace Admin_Student_Teacher.Data.Repositories
{
    public class StudentRepository:IStudentRepository
    {
        private readonly DapperContext _context;
        public StudentRepository(DapperContext context)
        {
            _context = context;
        }
        public int? GetRollNoByUserId(string userId)
        {
            using var conn = _context.CreateConnection();

            var sql = @"SELECT RollNo FROM UserProfiles WHERE UserId = @UserId";

            return conn.QueryFirstOrDefault<int?>(sql, new { UserId = userId });
        }

        public bool RollNoExists(int rollNo, string userId)
        {
            using var conn = _context.CreateConnection();
            var sql = @"SELECT COUNT(1) FROM UserProfiles WHERE RollNo=@RollNo AND UserId <> @UserId";
            return conn.ExecuteScalar<int>(sql, new { RollNo = rollNo, UserId = userId })>0;
        }

        public void UpdateRollNo(string userId, int? rollNo)
        {
            using var conn = _context.CreateConnection();

            var sql = @"UPDATE UserProfiles
                    SET RollNo = @RollNo
                    WHERE UserId = @UserId";

            conn.Execute(sql, new { RollNo = rollNo, UserId = userId });
        }

        public StudentVM GetStudentProfile(string studentId)
        {
            var sql = @"
        SELECT u.Email, p.FirstName, p.LastName, p.RollNo
        FROM AspNetUsers u
        JOIN UserProfiles p ON u.Id = p.UserId
        WHERE u.Id = @StudentId";

            using var conn = _context.CreateConnection();
            return conn.QueryFirstOrDefault<StudentVM>(
                sql, new { StudentId = studentId });
        }

        public string GetStudentName(string studentId)
        {
            using var conn = _context.CreateConnection();

            var sql = @"
            SELECT p.FirstName + ' ' + p.LastName
            FROM AspNetUsers u
            JOIN UserProfiles p ON u.Id = p.UserId
            WHERE u.Id = @StudentId
        ";

            return conn.QueryFirstOrDefault<string>(sql, new { StudentId = studentId });
        }


        // to get student marks
        public IEnumerable<StudentMarksVM> GetStudentMarks(string studentId)
        {
            using var conn = _context.CreateConnection();

            var sql = @"
            SELECT 
                sub.SubjectName,
                sm.Marks
            FROM StudentMarks sm
            INNER JOIN Subjects sub ON sm.SubjectId = sub.Id
            WHERE sm.StudentId = @StudentId
        ";

            return conn.Query<StudentMarksVM>(sql, new { StudentId = studentId });
        }

    }
}
