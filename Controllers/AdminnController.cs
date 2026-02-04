using Admin_Student_Teacher.Areas.Identity.Pages.Account;
using Admin_Student_Teacher.Data.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Admin_Student_Teacher.Controllers
{
    public class AdminnController : Controller
    {
        private readonly IAdminRepository _repo;
        //private readonly UserManager<IdentityUser> _userManager;

        public AdminnController( IAdminRepository repo)
        {
            _repo = repo;
            //_userManager = userManager;
        }

        public IActionResult UserList()
        {
            var users = _repo.GetUsersWithProfileAndRole();
            return View(users);
        }

        //[HttpPost]
        //public async Task<IActionResult> AssignRole(string userId, string role)
        //{
        //    var user = await _userManager.FindByIdAsync(userId);

        //    var roles = await _userManager.GetRolesAsync(user);
        //    await _userManager.RemoveFromRolesAsync(user, roles);

        //    await _userManager.AddToRoleAsync(user, role);

        //    return RedirectToAction(nameof(UserList));
        //}
        public IActionResult Index()
        {
            var users = _repo.GetUsersWithProfileAndRole();

            return PartialView("Index", users);
        }
        [HttpPost]
        public IActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Json(new
                {
                    success = false,
                    message = "Invalid user id."
                });
            }

            var deleted = _repo.DeleteUser(id);

            if (!deleted)
            {
                return Json(new
                {
                    success = false,
                    message = "Deletion failed."
                });
            }

            return Json(new
            {
                success = true,
                message = "User deleted successfully",
                data = id
            });
        }
        public IActionResult RegisterUser()
        {
            var model = new RegisterModel.InputModel();
            return PartialView("~/Areas/Identity/Pages/Account/_RegisterPartial.cshtml", model);
        }
    }
}
