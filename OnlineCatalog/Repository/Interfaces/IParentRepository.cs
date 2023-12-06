using OnlineCatalog.Models;

namespace OnlineCatalog.Repository.Interfaces
{
    public interface IParentRepository : IBaseRepository<Parent>
    {
        Task<Parent> GetParentById(int parentId);
    }
}
