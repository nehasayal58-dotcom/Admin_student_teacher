using Admin_Student_Teacher.Models;
using Dapper;
using System.Xml.Linq;

namespace Admin_Student_Teacher.Data.Repositories
{
    public class UserProfileRepository:IUserProfileRepository
    {
        private readonly DapperContext _context;

        public UserProfileRepository(DapperContext context)
        {
            _context = context;
        }

        // READ ALL
        public IEnumerable<UserProfile> GetAll()
        {
            var sql = "SELECT * FROM UserProfiles";

            using var connection = _context.CreateConnection();
            return connection.Query<UserProfile>(sql);
        }

        // READ BY USERID
        public UserProfile? GetByUserId(string userId)
        {
            var sql = "SELECT * FROM UserProfiles WHERE UserId = @UserId";

            using var connection = _context.CreateConnection();
            return connection.QuerySingleOrDefault<UserProfile>(sql, new { UserId = userId });
        }

        // CREATE
        //publMicrosoft.Data.SqlClient.SqlException: 'Must declare the scalar variable "@UserId".'ic void Create(UserProfile profile)
        //{
        //    var sql = @"INSERT INTO UserProfiles (UserId, FirstName, LastName)
        //            VALUES (@UserId, @FirstName, @LastName)";

        //    using var connection = _context.CreateConnection();
        //    connection.Execute(sql, profile);
        //}

        public bool Create(UserProfile profile)
        {
            using var conn = _context.CreateConnection();
            var rowsAffected = conn.Execute(@"INSERT INTO UserProfiles (UserId, FirstName, LastName)
                 VALUES (@UserId, @FirstName, @LastName)",profile
           );
            // true = insert happened, false = nothing inserted
            return rowsAffected > 0;
        }
        public bool delete(int id)
        {
            using var conn = _context.CreateConnection();
            var sql = "DELETE FROM UserProfiles WHERE Id = @Id";
            var rowsAffected = conn.Execute(sql, new { Id = id });
            return rowsAffected > 0;
        }
    }
}
