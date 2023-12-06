using OnlineCatalog.Models;

namespace OnlineCatalog.Repository.Interfaces
{
    public interface IYearSubjectsPlan : IBaseRepository<YearSubjectsPlan>
    {
        Task<YearSubjectsPlan> GetCurrentYearSubjectsPlan();

    }
}
