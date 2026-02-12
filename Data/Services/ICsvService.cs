using Admin_Student_Teacher.Models;

namespace Admin_Student_Teacher.Data.Services
{
    public interface ICsvService
    {
        byte[] GenerateUsersCsv(List<AdminUserVM> users);
    }
}
