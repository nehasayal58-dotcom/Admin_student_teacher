using Admin_Student_Teacher.Models;
using System.Text;

namespace Admin_Student_Teacher.Data.Services
{
    public class CsvService : ICsvService
    {
        public byte[] GenerateUsersCsv(List<AdminUserVM> users)
        {
        
            var sb = new StringBuilder();

            // 🔹 Header Row
            sb.AppendLine("Email,Name,Role,Roll No,Subjects,Percentage,Subject Count");

            foreach (var u in users)
            {
                var name = $"{u.FirstName} {u.LastName}";

                var rollNo = u.Role == "Student" ? u.RollNo?.ToString() : "";
                var subjects = u.Role == "Teacher" ? u.Subjects : "";
                var subjectCount = u.Role == "Student" ? u.SubjectCount : "";

                sb.AppendLine($"{u.Email},{name},{u.Role},{rollNo},{subjects},{u.Percentage},{subjectCount}");
            }

            return Encoding.UTF8.GetBytes(sb.ToString());
        }
    } 
}

