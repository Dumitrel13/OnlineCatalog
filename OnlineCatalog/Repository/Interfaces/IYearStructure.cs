using OnlineCatalog.Models;

namespace OnlineCatalog.Repository.Interfaces
{
    public interface IYearStructure : IBaseRepository<YearStructure>
    {
        Task<YearStructure> GetCurrentYearStructure();
        Task<YearStructure> GetYearStructureById(int id);
    }
}
