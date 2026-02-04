using Admin_Student_Teacher.Data.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Admin_Student_Teacher.Controllers
{

    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IUserRepository _repo;

        public AdminController(IUserRepository repo)
        {
            _repo = repo;
        }

        public IActionResult Profile()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = _repo.GetById(userId);

            return View(user);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult UserList()
        {
            var users = _repo.GetAllUsers();
            return View(users);
        }

    }
}
