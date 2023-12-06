using Microsoft.EntityFrameworkCore;
using OnlineCatalog.Data;
using OnlineCatalog.Models;
using OnlineCatalog.Repository.Interfaces;

namespace OnlineCatalog.Repository
{
    public class GradeRepository : BaseRepository<Grade>, IGradeRepository
    {
        public GradeRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<List<Grade>> GetPupilGradesForThisYear(int pupilId, DateTime startingDate, DateTime endingDate)
        {
            return await _dbSet.Include("Subject").Include("GradeType").Where(g => g.Date >= startingDate && g.Date <= endingDate && 
                                                 g.Pupil.Id == pupilId).ToListAsync();
        }

        public async Task<List<Grade>> GetGradesForSpecificSubject(int pupilId, int subjectId, DateTime startingDate, DateTime endingDate)
        {
            return await _dbSet.Include("GradeType").Where(g => g.Date >= startingDate && g.Date <= endingDate &&
                                           g.Pupil.Id == pupilId && g.Subject.SubjectId == subjectId).ToListAsync();
        }
    }
}
