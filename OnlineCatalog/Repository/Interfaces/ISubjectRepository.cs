using OnlineCatalog.Models;

namespace OnlineCatalog.Repository.Interfaces
{
    public interface ISubjectRepository : IBaseRepository<Subject>
    {
        Task<Subject> GetSubjectByIdAsync(int id);
        Task<List<Subject>> GetSubjectsByIds(List<int> subjectsIds);
        Task<Subject> GetSubjectByName(string name);
        Task<IEnumerable<Subject>> SearchSubjectAsync(string searchQuery);
    }
}
