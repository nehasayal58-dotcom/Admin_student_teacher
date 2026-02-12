using Dapper;

namespace Admin_Student_Teacher.Data.Repositories
{
    public class MarksRepository:IMarksRepository
    {
        private readonly DapperContext _context;

        public MarksRepository(DapperContext context)
        {
            _context = context;
        }

        public void SaveMarks(string studentId, int subjectId, int marks)
        {
            using var conn = _context.CreateConnection();

            var sql = @"
            IF EXISTS (
                SELECT 1 FROM StudentMarks
                WHERE StudentId = @StudentId AND SubjectId = @SubjectId
            )
            UPDATE StudentMarks
            SET Marks = @Marks
            WHERE StudentId = @StudentId AND SubjectId = @SubjectId
            ELSE
            INSERT INTO StudentMarks (StudentId, SubjectId, Marks)
            VALUES (@StudentId, @SubjectId, @Marks)
        ";

            conn.Execute(sql, new
            {
                StudentId = studentId,
                SubjectId = subjectId,
                Marks = marks
            });
        }
    }
}
