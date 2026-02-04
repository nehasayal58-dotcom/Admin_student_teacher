using Admin_Student_Teacher.Models;
using Dapper;

namespace Admin_Student_Teacher.Data.Repositories
{
    public class UserRepository:IUserRepository
    {
        private readonly DapperContext _context;

        public UserRepository(DapperContext context)
        {
            _context = context;
        }

        public UserDto GetById(string userId)
        {
            using var con = _context.CreateConnection();

            var sql = @"
            SELECT u.Id, u.Email, u.FirstName, u.LastName, r.Name AS Role
            FROM AspNetUsers u
            LEFT JOIN AspNetUserRoles ur ON u.Id = ur.UserId
            LEFT JOIN AspNetRoles r ON ur.RoleId = r.Id
            WHERE u.Id = @UserId";

            return con.QuerySingleOrDefault<UserDto>(sql, new { UserId = userId });
        }

        public List<UserDto> GetAllUsers()
        {
            using var con = _context.CreateConnection();

            var sql = @"
            SELECT u.Id, u.Email, u.FirstName, u.LastName, r.Name AS Role
            FROM AspNetUsers u
            LEFT JOIN AspNetUserRoles ur ON u.Id = ur.UserId
            LEFT JOIN AspNetRoles r ON ur.RoleId = r.Id";

            return con.Query<UserDto>(sql).ToList();
        }
    }
}
