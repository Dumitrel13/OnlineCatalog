using OnlineCatalog.Models;

namespace OnlineCatalog.Repository.Interfaces
{
    public interface ISchoolRepository : IBaseRepository<School>
    {
        Task<School> GetSchoolByName(string name);
        Task<School> GetSchoolById(int id);
        Task<School> GetSchoolByPrinciple(Teacher teacher);
    }
}
