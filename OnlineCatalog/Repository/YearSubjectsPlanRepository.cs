using Microsoft.EntityFrameworkCore;
using OnlineCatalog.Data;
using OnlineCatalog.Models;
using OnlineCatalog.Repository.Interfaces;

namespace OnlineCatalog.Repository
{
    public class YearSubjectsPlanRepository : BaseRepository<YearSubjectsPlan>, IYearSubjectsPlan
    {
        public YearSubjectsPlanRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<YearSubjectsPlan> GetCurrentYearSubjectsPlan()
        {
            return await _dbSet.Include("YearStructure").Include("TeacherAssignments")
                .Include("TeacherAssignments.Teacher").Include("TeacherAssignments.Subject")
                .Include("TeacherAssignments.Classroom").FirstAsync(x =>
                    x.YearStructure.StartingYear <= DateTime.Now.Date &&
                    DateTime.Now.Date <= x.YearStructure.EndingYear);
        }
    }
}
