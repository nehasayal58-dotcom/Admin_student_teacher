using Admin_Student_Teacher.Areas.Identity.Pages.Account;
using Admin_Student_Teacher.Data.Repositories;
using Admin_Student_Teacher.Data.Services;
using Admin_Student_Teacher.Models;
using Admin_Student_Teacher.Services.Pdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.Protocol;
using QuestPDF.Helpers;
using Rotativa.AspNetCore;
using Rotativa.AspNetCore.Options;
using System.Security.Claims;

namespace Admin_Student_Teacher.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminnController : Controller
    {
        private readonly IAdminRepository _repo;
        private readonly ISubjectRepository _subjectRepository;
        private readonly ITeacherSubjectRepository _teacherSubjectRepository;
        private readonly IStudentRepository _stduentRepo;
        private readonly IExcelService _excelService;
        private readonly ICsvService _csvService;
        private readonly EmailService _emailService;
        private readonly IZipService _zipService;


        //private readonly UserManager<IdentityUser> _userManager;

        public AdminnController( IAdminRepository repo,ISubjectRepository subjectRepository,ITeacherSubjectRepository teacherSubjectRepository,IStudentRepository studentRepo,
            IExcelService excelService,ICsvService csvService,EmailService emailService, IZipService zipService)
        {
            _repo = repo;
            _subjectRepository = subjectRepository;
            _teacherSubjectRepository = teacherSubjectRepository;
            _stduentRepo = studentRepo;
            _excelService = excelService;
            _csvService = csvService;
            _emailService = emailService;
            //_userManager = userManager;
            _zipService = zipService;
        }

        public IActionResult UserList(int page=1, int pageSize = 3,string searchText="",string roleId="",int?subjectId=null,string nameSearch = "", string emailSearch = "")
        {
            var users = _repo.GetUsersWithProfileAndRole(page, pageSize, nameSearch, emailSearch, roleId, subjectId);
            users.PageSize = pageSize;
            users.CurrentPage = page;
            users.NameSearch = nameSearch;
            users.EmailSearch = emailSearch;
            
            users.Roles = _repo.GetRolesForDropdown();
            users.Subjects = _subjectRepository.GetSubjectsForDropdown();
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
        public IActionResult Index(int page=1, int pageSize = 3,string searchText="" ,string roleId = "", int? subjectId = null,string nameSearch="",string emailSearch="" )
        {
            
            var users = _repo.GetUsersWithProfileAndRole(page, pageSize, nameSearch, emailSearch,roleId, subjectId);
            users.PageSize = pageSize;       
            users.CurrentPage = page;
            users.NameSearch = nameSearch;
            users.EmailSearch = emailSearch;
      

            users.Roles = _repo.GetRolesForDropdown();
            users.Subjects = _subjectRepository.GetSubjectsForDropdown();


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
            var model = new RegisterModel.InputModel()
            {
                Subjects = _subjectRepository.GetUnassignedSubjects()
            .Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.SubjectName
            }).ToList()
            };
            return PartialView("~/Areas/Identity/Pages/Account/_RegisterPartial.cshtml", model);
        }

      
        // updated  assign subject

        public IActionResult AssignSubject(string teacherId)
        {
            ViewBag.TeacherId = teacherId;
            ViewBag.Subjects = _subjectRepository.GetUnassignedSubjects();
            return PartialView("AssignSubject");
        }
     
        [HttpPost]
        public IActionResult AssignSubject(TeacherSubject model)
        {
            //if (_teacherSubjectRepository.ExistsForTeacher(model.TeacherId, model.SubjectId))
            //{
            //    return Json(new
            //    {
            //        success = false,
            //        message = "This subject is already assigned to this teacher."
            //    });
            //}
            if(_teacherSubjectRepository.TeacherHasSubject(model.TeacherId))
            {
                return Json(new
                {
                    Success = false,
                    message = "this teacher already  has a subject assigned "

                });
            }

            if (_teacherSubjectRepository.SubjectAlreadyAssigned(model.SubjectId ,model.TeacherId))
            {
                return Json(new
                {
                    success = false,
                    message = "This subject is already assigned to another teacher."
                });
            }

            _teacherSubjectRepository.Assign(model);

            return Json(new
            {
                success = true,
                message = "Subject assigned successfully."
            });
        }

        //edit 
        [HttpGet]
        public IActionResult Edit(string id)
        {
            var user = _repo.GetUserById(id);

            if (user.Role == "Teacher")
            {

                user.SubjectId = _teacherSubjectRepository.GetAssignedSubject(id);

                ViewBag.Subjects = _subjectRepository.GetSubjectsForEdit(user.SubjectId);

            }

            return PartialView("Edit", user);
        }

        [HttpPost]
        public IActionResult Edit(AdminUserVM model)
        {
            //Update user basic info
            //if (!_repo.UpdateUser(model))
            //{
            //    return Json(new
            //    {
            //        success = false,
            //        message = "User update failed"
            //    });
            //}
            _repo.UpdateUser(model);
            //  SUBJECT LOGIC
            if (model.Role == "Teacher")
            {
                // CASE 1: UNASSIGN SUBJECT
                if (!model.SubjectId.HasValue)
                {
                    _teacherSubjectRepository.RemoveByTeacher(model.UserId);
                }
                else
                {
                    //  CASE 2: ASSIGN / CHANGE SUBJECT
                    if (_teacherSubjectRepository.SubjectAlreadyAssigned(model.SubjectId.Value,model.UserId))
                    {
                        return Json(new
                        {
                            success = false,
                            message = "This subject is already assigned to another teacher."
                        });
                    }

                    _teacherSubjectRepository.UpdateTeacherSubject(
                        model.UserId,
                        model.SubjectId.Value
                    );
                }
            }
            else
            {
                // Role changed (Teacher → Student/Admin)
                _teacherSubjectRepository.RemoveByTeacher(model.UserId);
            }
            if (model.Role == "Student")
            {
                // RollNo is OPTIONAL for students
                if (model.RollNo.HasValue)
                {
                    // Only check uniqueness IF roll number is provided
                    if (_stduentRepo.RollNoExists(model.RollNo.Value, model.UserId))
                    {
                        return Json(new
                        {
                            success = false,
                            message = "This roll number is already assigned to another student"
                        });
                    }

                    // Save roll number
                    _stduentRepo.UpdateRollNo(model.UserId, model.RollNo);
                }
                else
                {
                    // Student with no roll number → store NULL
                    _stduentRepo.UpdateRollNo(model.UserId, null);
                }
            }

            return Json(new
            {
                success = true,
                message = "User updated successfully"
            });
        }
        [HttpGet]
        public IActionResult StudentMarksPartial(String studentId)
        {

            //var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var marks = _stduentRepo.GetStudentMarks(studentId);
            return PartialView("_MarkList", marks);

        }


        //[HttpGet]
        //public IActionResult DownloadFullReport()
        //{
        //    var report = _repo.GetFullReportData();

        //    var pdfBytes = AdminReportPdf.Generate(report);

        //    return File(
        //        pdfBytes,
        //        "application/pdf",
        //        "Admin_Full_Report.pdf"
        //    );
        //}
        public IActionResult DownloadAdminReport()
        {
            var model = _repo.GetFullReportData();

            return new ViewAsPdf("AdminReportPdf", model)
            {
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait
            };
        }


         public  IActionResult DownloadPdf(string nameSearch="",string emailSearch="",string roleId="",int?subjectId=null)
        {
            var users = _repo.GetUsersForPdf(nameSearch, emailSearch, roleId, subjectId);
            return new ViewAsPdf("UserPdf", users)
            {
                FileName = "FilteredUsers.pdf",
                PageOrientation = Orientation.Landscape,
                PageSize = Size.A4
            };
        }

        // for excel download 
        public IActionResult ExportToExcel(string nameSearch = "", string emailSearch = "", string roleId = "", int? subjectId = null)
        {
            var users = _repo.GetUsersForPdf(nameSearch, emailSearch, roleId, subjectId);
            var excelBytes = _excelService.GeneGenerateUsersExcel(users);
            return File(
                excelBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "FilteredUsers.xlsx"
            );
        }

        // for csv

        public IActionResult ExportToCsv(string nameSearch = "",
                                     string emailSearch = "",
                                     string roleId = "",
                                     int? subjectId = null)
        {
            var users = _repo.GetUsersForPdf(nameSearch, emailSearch, roleId, subjectId);

            var csvBytes = _csvService.GenerateUsersCsv(users);

            return File(
                csvBytes,
                "text/csv",
                "FilteredUsers.csv"
            );
        }
        // tp send email
        [HttpPost]
        public async Task<JsonResult> SendPdfToEmail(string toEmail,
                                             string nameSearch = "",
                                             string emailSearch = "",
                                             string roleId = "",
                                             int? subjectId = null)
        {
            if (string.IsNullOrEmpty(toEmail))
                return Json(new { success = false, message = "Email is required!" });

            var users = _repo.GetUsersForPdf(nameSearch, emailSearch, roleId, subjectId);

            var pdf = new ViewAsPdf("UserPdf", users)
            {
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape,
                PageSize = Rotativa.AspNetCore.Options.Size.A4
            };

            var pdfBytes = await pdf.BuildFile(ControllerContext);

            await _emailService.SendEmail(toEmail, pdfBytes);

            return Json(new { success = true, message = "PDF Sent Successfully!" });
        }


        // for zip file 
        public async Task<IActionResult> DownloadZip(string nameSearch = "",
                                                string emailSearch = "",
                                                string roleId = "",
                                                int? subjectId = null)
        {
            var users = _repo.GetUsersForPdf(nameSearch, emailSearch, roleId, subjectId);

            var excelBytes = _excelService.GeneGenerateUsersExcel(users);
            var csvBytes = _csvService.GenerateUsersCsv(users);

            var pdf = new ViewAsPdf("UserPdf", users)
            {
                PageOrientation = Orientation.Landscape,
                PageSize = Size.A4
            };

            var pdfBytes = await pdf.BuildFile(ControllerContext);

            var zipBytes = _zipService.GenerateUsersZip(
                users,
                pdfBytes,
                excelBytes,
                csvBytes);

            return File(zipBytes, "application/zip", "FilteredUsers.zip");
        }
    }

}





