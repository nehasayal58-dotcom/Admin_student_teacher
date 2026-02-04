using Admin_Student_Teacher.Models;
using System.Xml.Linq;

namespace Admin_Student_Teacher.Data.Repositories
{
    public interface IUserProfileRepository
    {
        IEnumerable<UserProfile> GetAll();
        UserProfile? GetByUserId(string userId);
        //void Create(UserProfile profile);
        bool Create(UserProfile profile);
        //UserProfile GetById(int id);
        bool delete(int id);
    }
}
