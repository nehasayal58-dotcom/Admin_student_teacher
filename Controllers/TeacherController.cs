using Admin_Student_Teacher.Data.Repositories;
using Admin_Student_Teacher.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Admin_Student_Teacher.Controllers
{
    public class TeacherController : Controller
    {

        private readonly ITeacherRepository _teacherRepository;
        private readonly IStudentRepository _studentRepo;
        private readonly ITeacherRepository _teacherRepo;
        private readonly IMarksRepository _marksRepo;
        public TeacherController(ITeacherRepository teacherRepository ,IStudentRepository studentRepo,ITeacherRepository teacherRepo
            ,IMarksRepository marksRepo)
        {
                _teacherRepository = teacherRepository;
            _studentRepo = studentRepo;
            _teacherRepo = teacherRepo;
            _marksRepo = marksRepo;
        }
        // 🔹 Teacher's own profile
        public IActionResult MyProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var teacher = _teacherRepository.GetMyProfile(userId);
            return View(teacher);
        }

        // 🔹 Students assigned to this teacher
        public IActionResult MyStudents()
        {
          
            return View();
        }

        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var students = _teacherRepository.GetMyStudents(userId);
            return PartialView("Index", students);
        }

        //Get
        // updated  assign subject

        public IActionResult AssignMarks(string studentId)
        {
            var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var vm = new AssignMarksVM
            {
                StudentId = studentId,
                StudentName = _studentRepo.GetStudentName(studentId),
                //Subjects = _teacherRepo.GetTeacherSubjects(teacherId)
                SubjectId = _teacherRepo.GetTeacherSubjectId(teacherId)
            };

            return PartialView("AssignMarks", vm);

        }
        [HttpPost]
        public IActionResult AssignMarks(AssignMarksVM model)
        {
            var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // 1️⃣ Security check
            if (!_teacherRepo.IsSubjectAssignedToTeacher(teacherId, model.SubjectId))
            {
                return Json(new
                {
                    success = false,
                    message = "You are not allowed to assign marks for this subject."
                });
            }

            // 2️⃣ Save / Update marks
            _marksRepo.SaveMarks(
                model.StudentId,
                model.SubjectId,
                model.Marks
            );

            // 3️⃣ Success JSON
            return Json(new
            {
                success = true,
                message = "Marks assigned successfully."
            });
        }
    }
}

