using OnlineCatalog.Models;

namespace OnlineCatalog.Repository.Interfaces
{
    public interface IGradeTypeRepository : IBaseRepository<GradeType>
    {
        Task<GradeType> GetGradeTypeById(int id);
        Task<GradeType> GetGradeTypeByName(string name);
    }
}
