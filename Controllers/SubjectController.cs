using Admin_Student_Teacher.Data.Repositories;
using Admin_Student_Teacher.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Admin_Student_Teacher.Controllers
{
    public class SubjectController : Controller
    {
       private readonly ISubjectRepository _subjectRepository;
        public SubjectController(ISubjectRepository subjectRepository)
        {
            _subjectRepository = subjectRepository;
        }
        public IActionResult Index()
        {
            
            return View();
        }   
        public IActionResult SubjectTable()
        {
            //var subjects = _subjectRepository.GetAll();
            var subjects = _subjectRepository.GetAllSubjects();
            return PartialView("SubjectTable",subjects);
        }
        public IActionResult Create()
        {
            return PartialView();
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(Subjects subjects)
        {
            if (_subjectRepository.SubjectExists(subjects.SubjectName))
            {
                return Json(new
                {
                    success = false,
                    message = "This subject is already exist"
                });
            }
           
            _subjectRepository.Create(subjects);
            return Json(new
            {
                success = true,
                message = "subject created successfully",
                data = subjects
            });
        }

        public IActionResult Edit(int id)
        {
            var subject = _subjectRepository.GetById(id);
            return PartialView(subject);
        }

        [HttpPost]
        public IActionResult Edit(Subjects subjects) {
            var updated = _subjectRepository.Update(subjects);
            if (!updated)
            {
                return Json(new
                {
                    success = false,
                    message = "Update failed."
                });
            }
            return Json(new
            {
                success = true,
                message = "Node updated successfully",
                data = subjects
            });
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var deleted = _subjectRepository.Delete(id);
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
