using Microsoft.EntityFrameworkCore;
using OnlineCatalog.Data;
using OnlineCatalog.Models;
using OnlineCatalog.Repository.Interfaces;

namespace OnlineCatalog.Repository
{
    public class AbsenceRepository : BaseRepository<Absence>, IAbsenceRepository
    {
        public AbsenceRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<List<Absence>> GetAbsencesByPupilIdAndSubjectId(int pupilId, int subjectId)
        {
            return await _dbSet.Where(a => a.Pupil.Id == pupilId && a.Subject.SubjectId == subjectId).ToListAsync();
        }

        public async Task<List<Absence>> GetAbsencesForTheCurrentYearByPupilId(int pupilId, DateTime start, DateTime end)
        {
            return await _dbSet.Include("Subject").Where(a => a.Pupil.Id == pupilId && a.Date >= start && a.Date <= end).ToListAsync();
        }

        public async Task<Absence> GetAbsenceById(int absenceId)
        {
            return await _dbSet.SingleAsync(a => a.AbsenceId == absenceId);
        }
    }
}
