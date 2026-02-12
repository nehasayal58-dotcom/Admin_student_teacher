using Admin_Student_Teacher.Models;

namespace Admin_Student_Teacher.Data.Services
{
    public interface IExcelService
    {
        byte[] GeneGenerateUsersExcel(List<AdminUserVM> users);
    }
}
