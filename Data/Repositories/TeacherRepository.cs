using Admin_Student_Teacher.Models;
using Dapper;

namespace Admin_Student_Teacher.Data.Repositories
{
    public class TeacherRepository:ITeacherRepository
    {
            private readonly DapperContext _context;
            public TeacherRepository(DapperContext context)
            {
                _context = context;
            }
        //for teacher details page
        public TeacherProfileVM GetMyProfile(string teacherId)
        {
            using var conn = _context.CreateConnection();

            var sql = @"
        SELECT 
            u.Email,
            p.FirstName,
            p.LastName,
            s.SubjectName
        FROM AspNetUsers u
        LEFT JOIN UserProfiles p ON u.Id = p.UserId
        LEFT JOIN TeacherSubjects ts ON u.Id = ts.TeacherId
        LEFT JOIN Subjects s ON ts.SubjectId = s.Id
        WHERE u.Id = @TeacherId";

            return conn.QueryFirstOrDefault<TeacherProfileVM>(sql, new { TeacherId = teacherId });
        }



        //public IEnumerable<StudentVM> GetMyStudents(String teacherId)
        //{
        //    using var conn = _context.CreateConnection();

        //    var sql = @"
        //    SELECT
        //        u.Email,
        //        p.FirstName,
        //        p.LastName,
        //        p.RollNo
        //    FROM AspNetUsers u
        //    INNER JOIN UserProfiles p ON p.UserId = u.Id
        //    INNER JOIN AspNetUserRoles ur ON ur.UserId = u.Id
        //    INNER JOIN AspNetRoles r ON r.Id = ur.RoleId
        //    WHERE r.Name = 'Student'
        //    ORDER BY p.RollNo ASC
        //";

        //    return conn.Query<StudentVM>(sql);
        //}
        public IEnumerable<StudentVM> GetMyStudents(string teacherId)
        {
            using var conn = _context.CreateConnection();

            var sql = @"
    SELECT
        u.Id AS StudentId,      
        u.Email,
        p.FirstName,
        p.LastName,
        p.RollNo ,
        sm.Marks
    FROM AspNetUsers u
    INNER JOIN UserProfiles p ON p.UserId = u.Id
    INNER JOIN AspNetUserRoles ur ON ur.UserId = u.Id
    INNER JOIN AspNetRoles r ON r.Id = ur.RoleId
    

    INNER JOIN TeacherSubjects ts ON ts.TeacherId = @TeacherId
 
LEFT JOIN StudentMarks sm 
        ON sm.StudentId = u.Id 
        AND sm.SubjectId = ts.SubjectId
    WHERE r.Name = 'Student'
    ORDER BY p.RollNo ASC
    ";

            return conn.Query<StudentVM>(sql, new {TeacherId=teacherId});
        }
        // teacher assign marks to student 
        public List<Subjects> GetTeacherSubjects(string teacherId)
        {
            using var conn = _context.CreateConnection();

            var sql = @"
            SELECT s.Id, s.SubjectName
            FROM Subjects s
            JOIN TeacherSubjects ts ON s.Id = ts.SubjectId
            WHERE ts.TeacherId = @TeacherId
        ";

            return conn.Query<Subjects>(sql, new { TeacherId = teacherId }).ToList();
        }

        public bool IsSubjectAssignedToTeacher(string teacherId, int subjectId)
        {
            using var conn = _context.CreateConnection();

            var sql = @"
            SELECT COUNT(1)
            FROM TeacherSubjects
            WHERE TeacherId = @TeacherId
              AND SubjectId = @SubjectId
        ";

            return conn.ExecuteScalar<int>(sql, new
            {
                TeacherId = teacherId,
                SubjectId = subjectId
            }) > 0;
        }


        public int GetTeacherSubjectId(string teacherId)
        {
            using var conn = _context.CreateConnection();

            return conn.QuerySingle<int>(
                "SELECT SubjectId FROM TeacherSubjects WHERE TeacherId = @TeacherId",
                new { TeacherId = teacherId }
            );
        }


    }
}
