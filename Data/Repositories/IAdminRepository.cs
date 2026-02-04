using Admin_Student_Teacher.Models;

namespace Admin_Student_Teacher.Data.Repositories
{
    public interface IAdminRepository
    {

        public IEnumerable<AdminUserVM> GetUsersWithProfileAndRole();
        public bool DeleteUser(string userId);
    }
}
