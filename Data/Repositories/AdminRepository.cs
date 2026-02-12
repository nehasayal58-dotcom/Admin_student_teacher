using Admin_Student_Teacher.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        public bool DeleteUser(string userId)
        {
            using var conn = _context.CreateConnection();
            conn.Open();

            using var transaction = conn.BeginTransaction();

            try
            {
                // delete teacher-subject mapping
                conn.Execute("DELETE FROM TeacherSubjects WHERE TeacherId = @UserId",
                    new {userId=userId},
                    transaction
                    
                    
                    );


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
        // for edit 
        public AdminUserVM GetUserById(string userId)
        {
            var sql = @"
    SELECT 
        u.Id AS UserId,
        u.Email,
        p.FirstName,
        p.LastName,
        p.RollNo,                                                           
        r.Name AS Role
    FROM AspNetUsers u
    LEFT JOIN UserProfiles p ON u.Id = p.UserId
    LEFT JOIN AspNetUserRoles ur ON u.Id = ur.UserId
    LEFT JOIN AspNetRoles r ON ur.RoleId = r.Id
    WHERE u.Id = @UserId";

            using var conn = _context.CreateConnection();
            return conn.QueryFirstOrDefault<AdminUserVM>(sql, new { UserId = userId });
        }
        //update 
        public bool UpdateUser(AdminUserVM model)
        {
            using var conn = _context.CreateConnection();
            conn.Open();
            using var tx = conn.BeginTransaction();

            try
            {
                // 1️⃣ Update profile
                conn.Execute(@"
            UPDATE UserProfiles
            SET FirstName = @FirstName,
                LastName = @LastName,
                RollNo = CASE 
                    WHEN @Role = 'Student' THEN @RollNo
                    ELSE NULL
                 END
            WHERE UserId = @UserId",
                    model, tx);

                // 2️⃣ Update role
                conn.Execute(
                    "DELETE FROM AspNetUserRoles WHERE UserId = @UserId",
                    new { model.UserId }, tx);

                var roleId = conn.QuerySingle<string>(
                    "SELECT Id FROM AspNetRoles WHERE Name = @Role",
                    new { model.Role }, tx);

                conn.Execute(
                    "INSERT INTO AspNetUserRoles (UserId, RoleId) VALUES (@UserId, @RoleId)",
                    new { model.UserId, RoleId = roleId }, tx);

                tx.Commit();
                return true;
            }
            catch
            {
                tx.Rollback();
                return false;
            }
        }
        // to get teachers with assigned subjects
        public IEnumerable<AdminUserVM> GetTeachers()
        {
            using var conn = _context.CreateConnection();

            var sql = @"
        SELECT 
            u.Id AS UserId,
            u.Email,
            up.FirstName,
            up.LastName,
            r.Name AS Role,
            s.SubjectName
        FROM AspNetUsers u
        JOIN AspNetUserRoles ur ON u.Id = ur.UserId
        JOIN AspNetRoles r ON ur.RoleId = r.Id
        LEFT JOIN UserProfiles up ON u.Id = up.UserId
        LEFT JOIN TeacherSubjects ts ON u.Id = ts.TeacherId
        LEFT JOIN Subjects s ON ts.SubjectId = s.Id
        WHERE r.Name = 'Teacher'
        ORDER BY u.Email;
    ";

            return conn.Query<AdminUserVM>(sql);
        }
        public IEnumerable<AdminUserVM> GetStudents()
        {
            using var conn = _context.CreateConnection();
            var sql = @"
            SELECT u.Id AS UserId, u.Email, p.FirstName, p.LastName, r.Name AS Role
            FROM AspNetUsers u
            JOIN AspNetUserRoles ur ON u.Id = ur.UserId
            JOIN AspNetRoles r ON ur.RoleId = r.Id
            LEFT JOIN UserProfiles p ON u.Id = p.UserId
            WHERE r.Name = 'Student'";
            return conn.Query<AdminUserVM>(sql);
        }
        // for pdf logic 
        public AdminFullReportVM GetFullReportData()
        {
            using var conn = _context.CreateConnection();
            // 1️⃣ Teachers
            var teachers = conn.Query<TeacherVM>(@"
        SELECT 
            CONCAT(p.FirstName, ' ', p.LastName) AS Name,
            u.Email,
            s.SubjectName AS Subject,
            CASE 
                WHEN s.SubjectName IS NULL THEN 'Not Assigned'
                ELSE 'Assigned'
            END AS Status
        FROM AspNetUsers u
        JOIN UserProfiles p ON p.UserId = u.Id
        JOIN AspNetUserRoles ur ON ur.UserId = u.Id
        JOIN AspNetRoles r ON r.Id = ur.RoleId AND r.Name = 'Teacher'
        LEFT JOIN TeacherSubjects ts ON ts.TeacherId = u.Id
        LEFT JOIN Subjects s ON s.Id = ts.SubjectId
    ").ToList();


            // 2️⃣ Students
            var students = conn.Query<StudentVM>(@"
        SELECT
        p.RollNo,
        CONCAT(p.FirstName, ' ', p.LastName) AS Name,
        u.Email,
        COUNT(DISTINCT sm.SubjectId) AS TotalSubjects
        FROM AspNetUsers u
        JOIN UserProfiles p ON p.UserId = u.Id
        JOIN AspNetUserRoles ur ON ur.UserId = u.Id
        JOIN AspNetRoles r 
        ON r.Id = ur.RoleId AND r.Name = 'Student'
        LEFT JOIN StudentMarks sm 
        ON sm.StudentId = u.Id
        GROUP BY 
        p.RollNo, 
        p.FirstName, 
        p.LastName, 
        u.Email
").ToList();



            // 3️⃣ All Users
            var users = conn.Query<UserVM>(@"
        SELECT
            CONCAT(p.FirstName, ' ', p.LastName) AS Name,
            u.Email,
            r.Name AS Role
        FROM AspNetUsers u
        JOIN UserProfiles p ON p.UserId = u.Id
        JOIN AspNetUserRoles ur ON ur.UserId = u.Id
        JOIN AspNetRoles r ON r.Id = ur.RoleId
    ").ToList();
            // 4️⃣ Summary
            return new AdminFullReportVM
            {
                SchoolName = "NECHO SANSKRITI SENIOR SECONDARY SCHOOL",
                Address = "Plot No. D-258, Phase 8-A, Industrial Area, Sector 75, Mohali",
                Contact = "+91-7717474088 | info@nechonetworks.com | www.nechonetworks.com",

                Teachers = teachers,
                Students = students,
                Users = users,

                TotalTeachers = teachers.Count,
                TotalStudents = students.Count,
                TotalUsers = users.Count,
                TotalSubjects = conn.ExecuteScalar<int>("SELECT COUNT(*) FROM Subjects")
            };

        }

        //updated with search button 
    //    public PagedResult<AdminUserVM> GetUsersWithProfileAndRole(int page, int pageSize, string searchText = "")
    //    {
    //        using var conn = _context.CreateConnection();   //open database connection 
                                                            
    //        // 🔹 IMPORTANT: handle null searchText
    //        searchText = searchText ?? "";
    //        // Total items (apply search filter)
    //        var totalItemsSql = @"
    //    SELECT COUNT(*)
    //    FROM AspNetUsers u
    //    LEFT JOIN UserProfiles p ON u.Id = p.UserId
    //    WHERE (@SearchText = '' OR p.FirstName LIKE '%' + @SearchText + '%' OR p.LastName LIKE '%' + @SearchText + '%')
    //";
    //        var totalItems = conn.Query<int>(totalItemsSql, new { SearchText = searchText }).FirstOrDefault();

    //        // Paginated query
    //        var sql = @"
    //SELECT *
    //FROM
    //(
    //    SELECT 
    //        u.Id AS UserId,
    //        u.Email,
    //        p.FirstName,
    //        p.LastName,
    //        p.RollNo,
    //        r.Name AS Role,
    //        STRING_AGG(s.SubjectName, ', ') AS Subjects,
    //        sp.Percentage,
    //        sp.SubjectCount,
    //        ROW_NUMBER() OVER(ORDER BY u.Email) AS RowNum
    //    FROM AspNetUsers u
    //    LEFT JOIN UserProfiles p ON u.Id = p.UserId
    //    LEFT JOIN AspNetUserRoles ur ON u.Id = ur.UserId
    //    LEFT JOIN AspNetRoles r ON ur.RoleId = r.Id
    //    LEFT JOIN TeacherSubjects ts ON u.Id = ts.TeacherId
    //    LEFT JOIN Subjects s ON ts.SubjectId = s.Id
    //    LEFT JOIN (
    //        SELECT
    //            sm.StudentId,
    //            CAST((SUM(sm.Marks) * 100.0) / NULLIF(COUNT(sm.SubjectId) * 100,0) AS DECIMAL(5,2)) AS Percentage,
    //            CONCAT(COUNT(sm.SubjectId), ' / ', (SELECT COUNT(*) FROM Subjects)) AS SubjectCount
    //        FROM StudentMarks sm
    //        GROUP BY sm.StudentId
    //    ) sp ON sp.StudentId = u.Id
    //    WHERE (@SearchText = '' OR p.FirstName LIKE '%' + @SearchText + '%' OR p.LastName LIKE '%' + @SearchText + '%')
    //    GROUP BY u.Id, u.Email, p.FirstName, p.LastName, p.RollNo, r.Name, sp.Percentage, sp.SubjectCount
    //) AS UsersWithRowNum
    //WHERE RowNum BETWEEN @StartRow AND @EndRow
    //ORDER BY RowNum;
    //";

    //        var startRow = (page - 1) * pageSize + 1;
    //        var endRow = page * pageSize;

    //        var users = conn.Query<AdminUserVM>(sql, new { StartRow = startRow, EndRow = endRow, SearchText = searchText }).ToList();

    //        return new PagedResult<AdminUserVM>
    //        {
    //            Items = users,
    //            CurrentPage = page,
    //            PageSize = pageSize,
    //            TotalItems = totalItems,
    //            SearchText = searchText
    //        };
    //    }

        public IEnumerable<SelectListItem> GetRolesForDropdown()
        {
            using var conn = _context.CreateConnection();
            var sql = "SELECT Id, Name FROM AspNetRoles";
            return conn.Query(sql).Select(r => new SelectListItem
            {
                Value = r.Id,
                Text = r.Name
            }).ToList();
        }

        //public PagedResult<AdminUserVM> GetUsersWithProfileAndRole(int page, int pageSize, string searchText = "", string roleId = "", int? subjectId = null)
        //{
        //    using var conn = _context.CreateConnection();

        //    searchText = searchText ?? "";
        //    roleId = roleId ?? "";

        //    // 🔹 TOTAL COUNT QUERY (WITH FILTERS)
        //    var totalItemsSql = @"
        //    SELECT COUNT(DISTINCT u.Id)
        //    FROM AspNetUsers u
        //    LEFT JOIN UserProfiles p ON u.Id = p.UserId
        //    LEFT JOIN AspNetUserRoles ur ON u.Id = ur.UserId
        //    LEFT JOIN TeacherSubjects ts ON u.Id = ts.TeacherId
        //    WHERE 
        //        (@SearchText = '' OR 
        //         p.FirstName LIKE '%' + @SearchText + '%' OR 
        //         p.LastName LIKE '%' + @SearchText + '%')
        //    AND (@RoleId = '' OR ur.RoleId = @RoleId)
        //    AND (@SubjectId IS NULL OR ts.SubjectId = @SubjectId)
        //";

        //    var totalItems = conn.Query<int>(totalItemsSql, new
        //    {
        //        SearchText = searchText,
        //        RoleId = roleId,
        //        SubjectId = subjectId
        //    }).FirstOrDefault();

        //    // 🔹 PAGINATED QUERY
        //    var sql = @"
        //SELECT *
        //FROM
        //(
        //    SELECT 
        //        u.Id AS UserId,
        //        u.Email,
        //        p.FirstName,
        //        p.LastName,
        //        p.RollNo,
        //        r.Name AS Role,
        //        STRING_AGG(s.SubjectName, ', ') AS Subjects,
        //        sp.Percentage,
        //        sp.SubjectCount,
        //        ROW_NUMBER() OVER(ORDER BY u.Email) AS RowNum
        //    FROM AspNetUsers u
        //    LEFT JOIN UserProfiles p ON u.Id = p.UserId
        //    LEFT JOIN AspNetUserRoles ur ON u.Id = ur.UserId
        //    LEFT JOIN AspNetRoles r ON ur.RoleId = r.Id
        //    LEFT JOIN TeacherSubjects ts ON u.Id = ts.TeacherId
        //    LEFT JOIN Subjects s ON ts.SubjectId = s.Id
        //    LEFT JOIN (
        //        SELECT
        //            sm.StudentId,
        //            CAST((SUM(sm.Marks) * 100.0) /
        //                 NULLIF(COUNT(sm.SubjectId) * 100,0) AS DECIMAL(5,2)) AS Percentage,
        //            CONCAT(COUNT(sm.SubjectId), ' / ',
        //                   (SELECT COUNT(*) FROM Subjects)) AS SubjectCount
        //        FROM StudentMarks sm
        //        GROUP BY sm.StudentId
        //    ) sp ON sp.StudentId = u.Id
        //    WHERE 
        //        (@SearchText = '' OR 
        //         p.FirstName LIKE '%' + @SearchText + '%' OR 
        //         p.LastName LIKE '%' + @SearchText + '%')
        //    AND (@RoleId = '' OR ur.RoleId = @RoleId)
        //    AND (@SubjectId IS NULL OR ts.SubjectId = @SubjectId)
        //    GROUP BY 
        //        u.Id, u.Email, p.FirstName, p.LastName, 
        //        p.RollNo, r.Name, sp.Percentage, sp.SubjectCount
        //) AS UsersWithRowNum
        //WHERE RowNum BETWEEN @StartRow AND @EndRow
        //ORDER BY RowNum;
        //";

        //    var startRow = (page - 1) * pageSize + 1;
        //    var endRow = page * pageSize;

        //    var users = conn.Query<AdminUserVM>(sql, new
        //    {
        //        StartRow = startRow,
        //        EndRow = endRow,
        //        SearchText = searchText,
        //        RoleId = roleId,
        //        SubjectId = subjectId
        //    }).ToList();

        //    return new PagedResult<AdminUserVM>
        //    {
        //        Items = users,
        //        CurrentPage = page,
        //        PageSize = pageSize,
        //        TotalItems = totalItems,
        //        SearchText = searchText,
        //        SelectedRoleId = roleId,
        //        SelectedSubjectId = subjectId
        //    };
        //}





        public PagedResult<AdminUserVM> GetUsersWithProfileAndRole(int page, int pageSize, string nameSearch = "", string emailSearch = "", string roleId = "", int? subjectId = null)
        {
            using var conn = _context.CreateConnection();

            // Ensure null values are handled
            nameSearch = nameSearch ?? "";
            emailSearch = emailSearch ?? "";
            roleId = roleId ?? "";

            // 🔹 TOTAL COUNT QUERY
            var totalItemsSql = @"
            SELECT COUNT(DISTINCT u.Id)
            FROM AspNetUsers u
            LEFT JOIN UserProfiles p ON u.Id = p.UserId
            LEFT JOIN AspNetUserRoles ur ON u.Id = ur.UserId
            LEFT JOIN TeacherSubjects ts ON u.Id = ts.TeacherId
            WHERE 
                (@NameSearch = '' OR p.FirstName LIKE '%' + @NameSearch + '%' OR p.LastName LIKE '%' + @NameSearch + '%')
            AND (@EmailSearch = '' OR u.Email LIKE '%' + @EmailSearch + '%')
            AND (@RoleId = '' OR ur.RoleId = @RoleId)
            AND (@SubjectId IS NULL OR ts.SubjectId = @SubjectId)
        ";

            var totalItems = conn.Query<int>(totalItemsSql, new
            {
                NameSearch = nameSearch,
                EmailSearch = emailSearch,
                RoleId = roleId,
                SubjectId = subjectId
            }).FirstOrDefault();

            // 🔹 PAGINATED QUERY
            var sql = @"
            SELECT *
            FROM
            (
                SELECT 
                    u.Id AS UserId,
                    u.Email,
                    p.FirstName,
                    p.LastName,
                    p.RollNo,
                    r.Name AS Role,
                    STRING_AGG(s.SubjectName, ', ') AS Subjects,
                    sp.Percentage,
                    sp.SubjectCount,
                    ROW_NUMBER() OVER(ORDER BY u.Email) AS RowNum
                FROM AspNetUsers u
                LEFT JOIN UserProfiles p ON u.Id = p.UserId
                LEFT JOIN AspNetUserRoles ur ON u.Id = ur.UserId
                LEFT JOIN AspNetRoles r ON ur.RoleId = r.Id
                LEFT JOIN TeacherSubjects ts ON u.Id = ts.TeacherId
                LEFT JOIN Subjects s ON ts.SubjectId = s.Id
                LEFT JOIN (
                    SELECT
                        sm.StudentId,
                        CAST((SUM(sm.Marks) * 100.0) / NULLIF(COUNT(sm.SubjectId) * 100, 0) AS DECIMAL(5,2)) AS Percentage,
                        CONCAT(COUNT(sm.SubjectId), ' / ', (SELECT COUNT(*) FROM Subjects)) AS SubjectCount
                    FROM StudentMarks sm
                    GROUP BY sm.StudentId
                ) sp ON sp.StudentId = u.Id
                WHERE 
                    (@NameSearch = '' OR p.FirstName LIKE '%' + @NameSearch + '%' OR p.LastName LIKE '%' + @NameSearch + '%')
                AND (@EmailSearch = '' OR u.Email LIKE '%' + @EmailSearch + '%')
                AND (@RoleId = '' OR ur.RoleId = @RoleId)
                AND (@SubjectId IS NULL OR ts.SubjectId = @SubjectId)
                GROUP BY 
                    u.Id, u.Email, p.FirstName, p.LastName, 
                    p.RollNo, r.Name, sp.Percentage, sp.SubjectCount
            ) AS UsersWithRowNum
            WHERE RowNum BETWEEN @StartRow AND @EndRow
            ORDER BY RowNum;
        ";

            var startRow = (page - 1) * pageSize + 1;
            var endRow = page * pageSize;

            var users = conn.Query<AdminUserVM>(sql, new
            {
                StartRow = startRow,
                EndRow = endRow,
                NameSearch = nameSearch,
                EmailSearch = emailSearch,
                RoleId = roleId,
                SubjectId = subjectId
            }).ToList();

            return new PagedResult<AdminUserVM>
            {
                Items = users,
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                SearchText = $"{nameSearch} | {emailSearch}",  // optional display
                SelectedRoleId = roleId,
                SelectedSubjectId = subjectId
            };
        }


        //Download pdf according  to filter condition 
        public List<AdminUserVM> GetUsersForPdf(string nameSearch = "", string emailSearch = "", string roleId = "", int? subjectId = null)
        {
            using var conn = _context.CreateConnection();

            nameSearch ??= "";
            emailSearch ??= "";
            roleId ??= "";

            var sql = @"
        SELECT 
            u.Id AS UserId,
            u.Email,
            p.FirstName,
            p.LastName,
            p.RollNo,
            r.Name AS Role,
            STRING_AGG(s.SubjectName, ', ') AS Subjects,
            sp.Percentage,
            sp.SubjectCount
        FROM AspNetUsers u
        LEFT JOIN UserProfiles p ON u.Id = p.UserId
        LEFT JOIN AspNetUserRoles ur ON u.Id = ur.UserId
        LEFT JOIN AspNetRoles r ON ur.RoleId = r.Id
        LEFT JOIN TeacherSubjects ts ON u.Id = ts.TeacherId
        LEFT JOIN Subjects s ON ts.SubjectId = s.Id
        LEFT JOIN (
            SELECT
                sm.StudentId,
                CAST((SUM(sm.Marks) * 100.0) / NULLIF(COUNT(sm.SubjectId) * 100, 0) AS DECIMAL(5,2)) AS Percentage,
                CONCAT(COUNT(sm.SubjectId), ' / ', (SELECT COUNT(*) FROM Subjects)) AS SubjectCount
            FROM StudentMarks sm
            GROUP BY sm.StudentId
        ) sp ON sp.StudentId = u.Id
        WHERE 
            (@NameSearch = '' OR p.FirstName LIKE '%' + @NameSearch + '%' OR p.LastName LIKE '%' + @NameSearch + '%')
        AND (@EmailSearch = '' OR u.Email LIKE '%' + @EmailSearch + '%')
        AND (@RoleId = '' OR ur.RoleId = @RoleId)
        AND (@SubjectId IS NULL OR ts.SubjectId = @SubjectId)
        GROUP BY 
            u.Id, u.Email, p.FirstName, p.LastName, 
            p.RollNo, r.Name, sp.Percentage, sp.SubjectCount
        ORDER BY u.Email
    ";

            return conn.Query<AdminUserVM>(sql, new
            {
                NameSearch = nameSearch,
                EmailSearch = emailSearch,
                RoleId = roleId,
                SubjectId = subjectId
            }).ToList();
        }
    }
}
