using Admin_Student_Teacher.Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Admin_Student_Teacher.Controllers
{
    public class StudentController : Controller
    {
        IStudentRepository _studentRepo;
        public StudentController(IStudentRepository studentRepo)
        {
               _studentRepo = studentRepo; 
        }
       public IActionResult Profile()
        {
            var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var students = _studentRepo.GetStudentProfile(studentId);
            return View(students);
        }

        

        public IActionResult Marks()
        {
            var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //var marks = _studentRepo.GetStudentMarks(studentId);
            return View();
        }
        public IActionResult Index()
        {
            var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var marks = _studentRepo.GetStudentMarks(studentId);
            return PartialView("_MarkList", marks);
        }
    }
}
