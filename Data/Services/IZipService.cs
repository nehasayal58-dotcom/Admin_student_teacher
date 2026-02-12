using Admin_Student_Teacher.Models;

namespace Admin_Student_Teacher.Data.Services
{
    public interface IZipService
    {
        byte[] GenerateUsersZip(
        List<AdminUserVM> users,
        byte[] pdfBytes,
        byte[] excelBytes,
        byte[] csvBytes);
    }
}
