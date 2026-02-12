using Admin_Student_Teacher.Data.Repositories;
using Admin_Student_Teacher.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace Admin_Student_Teacher.Controllers
{

    [Authorize]
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
