using Admin_Student_Teacher.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Admin_Student_Teacher.Data.Repositories
{
    public interface ISubjectRepository
    {
        List<Subjects> GetAll();
        Subjects GetById(int id);
        bool Create(Subjects subjects);
        bool Delete(int id);
        bool Update(Subjects subjects);
        bool SubjectExists(string subjectName);

        // assigned subject not show in dropdwonlist logic 
        //List<Subjects> GetUnassignedByTeacher(string teacherId);
        List<Subjects> GetUnassignedSubjects();
        public IEnumerable<Subjects> GetSubjectsForEdit(int? currentSubjectId);

        public IEnumerable<Subjects> GetAllSubjects();


        // for subject dropdown

        IEnumerable<SelectListItem> GetSubjectsForDropdown();
    }
}
