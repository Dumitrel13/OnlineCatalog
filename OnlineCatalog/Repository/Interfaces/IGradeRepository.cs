using OnlineCatalog.Models;

namespace OnlineCatalog.Repository.Interfaces
{
    public interface IGradeRepository : IBaseRepository<Grade>
    {
        Task<List<Grade>> GetPupilGradesForThisYear(int pupilId, DateTime startingDate, DateTime endingDate);
        Task<List<Grade>> GetGradesForSpecificSubject(int pupilId, int subjectId, DateTime startingDate, DateTime endingDate);
    }
}
