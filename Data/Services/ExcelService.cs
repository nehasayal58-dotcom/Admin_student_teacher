using Admin_Student_Teacher.Models;
using ClosedXML.Excel;

namespace Admin_Student_Teacher.Data.Services
{
    public class ExcelService : IExcelService
    {
        public byte[] GeneGenerateUsersExcel(List<AdminUserVM> users)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Filtered Users");

                // Header
                worksheet.Cell(1, 1).Value = "Email";
                worksheet.Cell(1, 2).Value = "Name";
                worksheet.Cell(1, 3).Value = "Role";
                worksheet.Cell(1, 4).Value = "Roll No";
                worksheet.Cell(1, 5).Value = "Subjects";
                worksheet.Cell(1, 6).Value = "Percentage";
                worksheet.Cell(1, 7).Value = "Subject Count";

                int row = 2;

                foreach (var u in users)
                {
                    worksheet.Cell(row, 1).Value = u.Email;
                    worksheet.Cell(row, 2).Value = $"{u.FirstName} {u.LastName}";
                    worksheet.Cell(row, 3).Value = u.Role;

                    if (u.Role == "Student")
                    {
                        worksheet.Cell(row, 4).Value = u.RollNo;
                        worksheet.Cell(row, 7).Value = u.SubjectCount;
                    }

                    if (u.Role == "Teacher")
                    {
                        worksheet.Cell(row, 5).Value = u.Subjects;
                    }

                    worksheet.Cell(row, 6).Value = u.Percentage;

                    row++;
                }

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }
    }
}

