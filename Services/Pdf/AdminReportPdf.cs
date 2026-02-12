using Admin_Student_Teacher.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace Admin_Student_Teacher.Services.Pdf
{
    public class AdminReportPdf
    {
        //public static byte[] Generate(AdminFullReportVM model)
        //{
        //    return Document.Create(container =>
        //    {
        //        container.Page(page =>
        //        {
        //            page.Size(PageSizes.A4);
        //            page.Margin(25);

        //            page.Content().Column(col =>
        //            {
        //                col.Spacing(10);

        //                // 🔹 HEADER
        //                col.Item().AlignCenter().Text(model.SchoolName).Bold().FontSize(16);
        //                col.Item().AlignCenter().Text(model.Address);
        //                col.Item().AlignCenter().Text(model.Contact);

        //                col.Item().PaddingTop(10)
        //                    .AlignCenter()
        //                    .Text("School Management System Report")
        //                    .Bold().FontSize(14);

        //                col.Item().Text($"Date: {DateTime.Now:dd-MM-yyyy}");
        //                col.Item().Text("Academic Year: 2025-26");

        //                // 🔹 SUMMARY
        //                col.Item().PaddingTop(10).Text("SUMMARY").Bold();

        //                col.Item().Text($"Total Teachers: {model.TotalTeachers}");
        //                col.Item().Text($"Total Students: {model.TotalStudents}");
        //                col.Item().Text($"Total Subjects: {model.TotalSubjects}");
        //                col.Item().Text($"Total Users: {model.TotalUsers}");

        //                // 🔹 TEACHERS
        //                col.Item().PaddingTop(10).Text("TEACHERS INFORMATION").Bold();
        //                TeachersTable(col, model.Teachers);

        //                // 🔹 STUDENTS
        //                col.Item().PaddingTop(10).Text("STUDENTS INFORMATION").Bold();
        //                StudentsTable(col, model.Students);

        //                // 🔹 USERS
        //                col.Item().PaddingTop(10).Text("ALL USERS").Bold();
        //                UsersTable(col, model.Users);
        //            });
        //        });
        //    }).GeneratePdf();
        //}
        //// 2️⃣ TEACHERS TABLE (ADD HERE ⬇️)
        //static void TeachersTable(ColumnDescriptor col, List<TeacherVM> teachers)
        //{
        //    col.Item().Table(table =>
        //    {
        //        table.ColumnsDefinition(c =>
        //        {
        //            c.RelativeColumn();
        //            c.RelativeColumn();
        //            c.RelativeColumn();
        //            c.RelativeColumn();
        //        });

        //        table.Header(h =>
        //        {
        //            h.Cell().Text("Name").Bold();
        //            h.Cell().Text("Email").Bold();
        //            h.Cell().Text("Subject").Bold();
        //            h.Cell().Text("Status").Bold();
        //        });

        //        foreach (var t in teachers)
        //        {
        //            table.Cell().Text(t.Name);
        //            table.Cell().Text(t.Email);
        //            table.Cell().Text(t.Subject);
        //            table.Cell().Text(t.Status ?? "Assigned");
        //        }
        //    });
        //}
        ////students table
        //static void StudentsTable(ColumnDescriptor col, List<StudentVM> students)
        //{
        //    col.Item().Table(table =>
        //    {
        //        table.ColumnsDefinition(columns =>
        //        {
        //            columns.ConstantColumn(60);
        //            columns.RelativeColumn();
        //            columns.RelativeColumn();
        //            columns.ConstantColumn(80);
        //        });

        //        table.Header(header =>
        //        {
        //            header.Cell().Text("Roll No").Bold();
        //            header.Cell().Text("Name").Bold();
        //            header.Cell().Text("Email").Bold();
        //            header.Cell().Text("Total Subjects").Bold();
        //        });

        //        foreach (var s in students)
        //        {
        //            table.Cell().Text(s.RollNo.ToString());
        //            table.Cell().Text(s.Name);
        //            table.Cell().Text(s.Email);
        //            table.Cell().Text(s.TotalSubjects.ToString());
        //        }
        //    });
        //}
        ////users table
        //static void UsersTable(ColumnDescriptor col, List<UserVM> users)
        //{
        //    col.Item().Table(table =>
        //    {
        //        table.ColumnsDefinition(columns =>
        //        {
        //            columns.RelativeColumn();
        //            columns.RelativeColumn();
        //            columns.RelativeColumn();
        //        });

        //        table.Header(header =>
        //        {
        //            header.Cell().Text("Name").Bold();
        //            header.Cell().Text("Email").Bold();
        //            header.Cell().Text("Role").Bold();
        //        });

        //        foreach (var u in users)
        //        {
        //            table.Cell().Text(u.Name);
        //            table.Cell().Text(u.Email);
        //            table.Cell().Text(u.Role);
        //        }
        //    });
        //}


        //updated design 

        public static byte[] Generate(AdminFullReportVM model)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);

                    page.Content().Column(col =>
                    {
                        col.Spacing(12);

                        // ===== HEADER (NO ERROR) =====
                        col.Item().Padding(10).Background(Colors.Blue.Darken2).Column(h =>
                        {
                            h.Item().AlignCenter()
                                .Text(model.SchoolName)
                                .FontSize(18)
                                .Bold()
                                .FontColor(Colors.White);

                            h.Item().AlignCenter()
                                .Text(model.Address)
                                .FontSize(10)
                                .FontColor(Colors.White);

                            h.Item().AlignCenter()
                                .Text(model.Contact)
                                .FontSize(10)
                                .FontColor(Colors.White);
                        });

                        col.Item().AlignCenter()
                            .Text("School Management System Report")
                            .FontSize(14)
                            .Bold();

                        col.Item().Text($"Date: {DateTime.Now:dd-MM-yyyy}");
                        col.Item().Text("Academic Year: 2025-26");

                        // ===== SUMMARY =====
                        col.Item().PaddingTop(10).Text("SUMMARY").Bold();

                        col.Item().Background(Colors.Grey.Lighten3).Padding(8).Column(s =>
                        {
                            s.Item().Text($"Total Teachers: {model.TotalTeachers}");
                            s.Item().Text($"Total Students: {model.TotalStudents}");
                            s.Item().Text($"Total Subjects: {model.TotalSubjects}");
                            s.Item().Text($"Total Users: {model.TotalUsers}");
                        });

                        // ===== TABLES =====
                        col.Item().PaddingTop(10).Text("TEACHERS INFORMATION").Bold();
                        TeachersTable(col, model.Teachers);

                        col.Item().PaddingTop(10).Text("STUDENTS INFORMATION").Bold();
                        StudentsTable(col, model.Students);

                        col.Item().PaddingTop(10).Text("ALL USERS").Bold();
                        UsersTable(col, model.Users);
                    });

                    // ===== FOOTER =====
                    page.Footer().AlignCenter()
                        .Text("Generated by School Management System")
                        .FontSize(9)
                        .FontColor(Colors.Grey.Darken1);
                });
            }).GeneratePdf();
        }

        // ===== TEACHERS TABLE =====
        static void TeachersTable(ColumnDescriptor col, List<TeacherVM> teachers)
        {
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(c =>
                {
                    c.RelativeColumn();
                    c.RelativeColumn();
                    c.RelativeColumn();
                    c.RelativeColumn();
                });

                table.Header(h =>
                {
                    h.Cell().Padding(5).Background(Colors.Blue.Lighten2).Text("Name").Bold();
                    h.Cell().Padding(5).Background(Colors.Blue.Lighten2).Text("Email").Bold();
                    h.Cell().Padding(5).Background(Colors.Blue.Lighten2).Text("Subject").Bold();
                    h.Cell().Padding(5).Background(Colors.Blue.Lighten2).Text("Status").Bold();
                });

                foreach (var t in teachers)
                {
                    table.Cell().Padding(5).Text(t.Name);
                    table.Cell().Padding(5).Text(t.Email);
                    table.Cell().Padding(5).Text(t.Subject);
                    table.Cell().Padding(5).Text(t.Status ?? "Assigned");
                }
            });
        }

        // ===== STUDENTS TABLE =====
        static void StudentsTable(ColumnDescriptor col, List<StudentVM> students)
        {
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(c =>
                {
                    c.ConstantColumn(60);
                    c.RelativeColumn();
                    c.RelativeColumn();
                    c.ConstantColumn(80);
                });

                table.Header(h =>
                {
                    h.Cell().Padding(5).Background(Colors.Blue.Lighten2).Text("Roll No").Bold();
                    h.Cell().Padding(5).Background(Colors.Blue.Lighten2).Text("Name").Bold();
                    h.Cell().Padding(5).Background(Colors.Blue.Lighten2).Text("Email").Bold();
                    h.Cell().Padding(5).Background(Colors.Blue.Lighten2).Text("Subjects").Bold();
                });

                foreach (var s in students)
                {
                    table.Cell().Padding(5).Text(s.RollNo.ToString());
                    table.Cell().Padding(5).Text(s.Name);
                    table.Cell().Padding(5).Text(s.Email);
                    table.Cell().Padding(5).Text(s.TotalSubjects.ToString());
                }
            });
        }

        // ===== USERS TABLE =====
        static void UsersTable(ColumnDescriptor col, List<UserVM> users)
        {
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(c =>
                {
                    c.RelativeColumn();
                    c.RelativeColumn();
                    c.RelativeColumn();
                });

                table.Header(h =>
                {
                    h.Cell().Padding(5).Background(Colors.Blue.Lighten2).Text("Name").Bold();
                    h.Cell().Padding(5).Background(Colors.Blue.Lighten2).Text("Email").Bold();
                    h.Cell().Padding(5).Background(Colors.Blue.Lighten2).Text("Role").Bold();
                });

                foreach (var u in users)
                {
                    table.Cell().Padding(5).Text(u.Name);
                    table.Cell().Padding(5).Text(u.Email);
                    table.Cell().Padding(5).Text(u.Role);
                }
            });
        }



    }
}
