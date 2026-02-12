using Admin_Student_Teacher.Data.Repositories;
using Admin_Student_Teacher.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Security.Claims;
using System.Xml.Linq;

namespace Admin_Student_Teacher.Controllers
{
    public class UserProfileController : Controller
    {
        
            private readonly IUserProfileRepository _repo;

            public UserProfileController(IUserProfileRepository repo)
            {
                _repo = repo;
            }
       public IActionResult Index()
        {
            //var profiles = _repo.GetAll();
            return View();
        }
        public IActionResult UserProfileList()
        {
            var profiles = _repo.GetAll();
            return PartialView("UserProfileList", profiles);
        }
        //updated create
        public IActionResult Create()
        {
            return PartialView(new UserProfile());
        }
        [HttpPost]

        public IActionResult Create(UserProfile model)
        {
            //if(!ModelState.IsValid) 
            //    return PartialView(node);
            model.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var Created = _repo.Create(model);
            if (!Created)
            {
                return Json(new
                {
                    success = false,
                    message = "Creation failed."
                });
            }

            return Json(new
            {
                success = true,
                message = "Node created successfully",
                data = model
            });
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var deleted = _repo.delete(id);
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
                message = "Node deleted successfully",
                data = id
            });
        }
    }
}
