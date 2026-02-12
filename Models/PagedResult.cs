using Microsoft.AspNetCore.Mvc.Rendering;

namespace Admin_Student_Teacher.Models
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();  // the user for this page 
        public int CurrentPage { get; set; } // the current page number
        public int PageSize { get; set; }  // how many items per page
        public int TotalItems { get; set; } //total number of filtered users (used to calculate total pages).
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
        public string SearchText { get; set; }

        // 🔹 FILTERS
        public string SelectedRoleId { get; set; }
        public int? SelectedSubjectId { get; set; }

        public IEnumerable<SelectListItem> Roles { get; set; }
        public IEnumerable<SelectListItem> Subjects { get; set; }

        // 🔹 Search filters
        public string NameSearch { get; set; } = "";   // Search by first/last name
        public string EmailSearch { get; set; } = "";  // Search by email

        public string RoleId { get; set; }
        public int? SubjectId { get; set; }
    }
    // class for pagination   brain 
}
