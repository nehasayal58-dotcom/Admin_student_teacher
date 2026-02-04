using Admin_Student_Teacher.Models;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace Admin_Student_Teacher.Data.Repositories
{
    public class AdminRepository:IAdminRepository
    {
        private readonly DapperContext _context;

        public AdminRepository(DapperContext context)
        {
            _context = context;
        }
        public IEnumerable<AdminUserVM> GetUsersWithProfileAndRole()
        {
            var sql = @"
        SELECT 
            u.Id AS UserId,
            u.Email,
            p.FirstName,
            p.LastName,
            r.Name AS Role
        FROM AspNetUsers u
        LEFT JOIN UserProfiles p ON u.Id = p.UserId
        LEFT JOIN AspNetUserRoles ur ON u.Id = ur.UserId
        LEFT JOIN AspNetRoles r ON ur.RoleId = r.Id
        ORDER BY u.Email";

            using var conn = _context.CreateConnection();
            return conn.Query<AdminUserVM>(sql);
        }

        public bool DeleteUser(string userId)
        {
            using var conn = _context.CreateConnection();
            conn.Open();

            using var transaction = conn.BeginTransaction();

            try
            {
                // 1. Delete profile
                conn.Execute(
                    "DELETE FROM UserProfiles WHERE UserId = @UserId",
                    new { UserId = userId },
                    transaction
                );

                // 2. Delete roles
                conn.Execute(
                    "DELETE FROM AspNetUserRoles WHERE UserId = @UserId",
                    new { UserId = userId },
                    transaction
                );

                // 3. Delete user
                var rows = conn.Execute(
                    "DELETE FROM AspNetUsers WHERE Id = @UserId",
                    new { UserId = userId },
                    transaction
                );

                transaction.Commit();
                return rows > 0;
            }
            catch
            {
                transaction.Rollback();
                return false;
            }
        }


    }
}
