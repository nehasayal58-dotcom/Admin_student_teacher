using Admin_Student_Teacher.Models;

namespace Admin_Student_Teacher.Data.Repositories
{
    public interface IUserRepository
    {
        UserDto GetById(string userId);
        List<UserDto> GetAllUsers();
    }
}
