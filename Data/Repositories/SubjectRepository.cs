using Admin_Student_Teacher.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Admin_Student_Teacher.Data.Repositories
{
    public class SubjectRepository : ISubjectRepository
    {
        private readonly DapperContext _context;
        public SubjectRepository(DapperContext context)
        {
            _context = context;
        }
        //public bool Create(Subjects subjects)
        //{
        //    var sql = "INSERT INTO Subjects (SubjectName) VALUES (@SubjectName)";

        //    using var connection = _context.CreateConnection();
        //    var result = connection.Execute(sql, subjects);

        //    return result > 0;
        //}

        public bool Delete(int id)
        {
            var sql = "DELETE FROM Subjects WHERE Id = @Id";

            using var connection = _context.CreateConnection();
            var result = connection.Execute(sql, new { Id = id });

            return result > 0;
        }

        public List<Subjects> GetAll()
        {
            var sql = "SELECT * FROM Subjects";

            using var connection = _context.CreateConnection();
            return connection.Query<Subjects>(sql).ToList();

        }

        public Subjects GetById(int id)
        {
            var sql = "SELECT * FROM Subjects WHERE Id = @Id";

            using var connection = _context.CreateConnection();
            return connection.QueryFirstOrDefault<Subjects>(sql, new { Id = id });
        }

        public bool Update(Subjects subjects)
        {
            var sql = @"UPDATE Subjects 
                    SET SubjectName = @SubjectName 
                    WHERE Id = @Id";

            using var connection = _context.CreateConnection();
            var result = connection.Execute(sql, subjects);

            return result > 0;
        }
        // subject already exist
        public bool SubjectExists(string subjectName)
        {
            var query = "SELECT COUNT(1) FROM Subjects WHERE SubjectName = @SubjectName";

            using var connection = _context.CreateConnection();
            var count = connection.ExecuteScalar<int>(query, new { SubjectName = subjectName });

            return count > 0;
        }
        public bool Create(Subjects subject)
        {
            if (SubjectExists(subject.SubjectName))
                return false; //  already exists

            var query = @"INSERT INTO Subjects (SubjectName)
                      VALUES (@SubjectName)";

            using var connection = _context.CreateConnection();
            var rows = connection.Execute(query, subject);

            return rows > 0;
        }
        //public List<Subjects> GetUnassignedByTeacher(string teacherId)
        //{
        //    var sql = @"
        //SELECT s.Id, s.SubjectName
        //FROM Subjects s
        //WHERE NOT EXISTS (
        //    SELECT 1
        //    FROM TeacherSubjects ts
        //    WHERE ts.SubjectId = s.Id
        //      AND ts.TeacherId = @TeacherId
        //)";

        //    using var conn = _context.CreateConnection();
        //    return conn.Query<Subjects>(sql, new { TeacherId = teacherId }).ToList();
        //}
        public List<Subjects> GetUnassignedSubjects()
        {
            var sql = @"
        SELECT s.Id, s.SubjectName
        FROM Subjects s
        WHERE NOT EXISTS (
            SELECT 1
            FROM TeacherSubjects ts
            WHERE ts.SubjectId = s.Id
        )";

            using var conn = _context.CreateConnection();
            return conn.Query<Subjects>(sql).ToList();
        }

        public IEnumerable<Subjects> GetSubjectsForEdit(int? currentSubjectId)
        {
            using var conn = _context.CreateConnection();

            var sql = @"
        SELECT * FROM Subjects
        WHERE Id = @CurrentSubjectId
           OR Id NOT IN (SELECT SubjectId FROM TeacherSubjects)
    ";

            return conn.Query<Subjects>(sql, new { CurrentSubjectId = currentSubjectId });
        }

        public IEnumerable<Subjects> GetAllSubjects()
        {
            using var conn = _context.CreateConnection();

            var sql = @"
    SELECT 
        s.Id,
        s.SubjectName,
        CASE 
            WHEN EXISTS (
                SELECT 1 
                FROM TeacherSubjects ts 
                WHERE ts.SubjectId = s.Id
            )
            THEN 1
            ELSE 0
        END AS IsAssigned
    FROM Subjects s
    ORDER BY s.SubjectName";

            return conn.Query<Subjects>(sql);
        }

        public IEnumerable<SelectListItem> GetSubjectsForDropdown()
        {
            using var conn = _context.CreateConnection();

            var sql = "SELECT Id, SubjectName FROM Subjects";

            return conn.Query(sql)
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.SubjectName
                }).ToList();
        }





    }
}
