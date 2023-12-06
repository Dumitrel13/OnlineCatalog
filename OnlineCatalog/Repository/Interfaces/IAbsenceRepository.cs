using OnlineCatalog.Models;

namespace OnlineCatalog.Repository.Interfaces
{
    public interface IAbsenceRepository : IBaseRepository<Absence>
    {
        Task<List<Absence>> GetAbsencesByPupilIdAndSubjectId(int pupilId, int subjectId);
        Task<List<Absence>> GetAbsencesForTheCurrentYearByPupilId(int pupilId, DateTime start, DateTime end);
        Task<Absence> GetAbsenceById(int absenceId);
    }
}
