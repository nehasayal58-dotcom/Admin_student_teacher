using Admin_Student_Teacher.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Admin_Student_Teacher.Data.Repositories
{
    public interface IAdminRepository
    {

        //public IEnumerable<AdminUserVM> GetUsersWithProfileAndRole();
        public bool DeleteUser(string userId);
        // for edit 
        AdminUserVM GetUserById(string userId);
        bool UpdateUser(AdminUserVM model);

        //FOR TEACHER AND STUDENT

        IEnumerable<AdminUserVM> GetTeachers();
        IEnumerable<AdminUserVM> GetStudents();

        //all the data for the PDF is assembled into one object

        public AdminFullReportVM GetFullReportData();


        // for pagination 
        //public  PagedResult<AdminUserVM> GetUsersWithProfileAndRole(int page, int pageSize);
        //public PagedResult<AdminUserVM> GetUsersWithProfileAndRolePaged(int page, int pageSize);
        //public PagedResult<AdminUserVM> GetUsersWithProfileAndRole(int page, int pageSize, string searchText = "");
        //public PagedResult<AdminUserVM> GetUsersWithProfileAndRole(int page, int pageSize, string searchText = "", string roleId = "", int? subjectId = null);


        //for  roles dropdown

        IEnumerable<SelectListItem> GetRolesForDropdown();

        public PagedResult<AdminUserVM> GetUsersWithProfileAndRole(int page, int pageSize, string nameSearch = "", string emailSearch = "", string roleId = "", int? subjectId = null);


        // downlod pdf  according to filter condition
        public List<AdminUserVM> GetUsersForPdf(string nameSearch = "", string emailSearch = "", string roleId = "", int? subjectId = null);






    }
}
