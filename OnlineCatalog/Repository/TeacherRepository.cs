using Microsoft.EntityFrameworkCore;
using OnlineCatalog.Data;
using OnlineCatalog.Models;
using OnlineCatalog.Repository.Interfaces;

namespace OnlineCatalog.Repository
{
    public class TeacherRepository : BaseRepository<Teacher>, ITeacherRepository
    {
        public TeacherRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Teacher> GetTeacherByIdAsync(int id)
        {
            return await _dbSet.Include("JobPeriodRoles").Include("JobPeriodRoles.Roles")
                .Include("JobPeriodSubjects").Include("JobPeriodSubjects.Subjects").Include("Specializations").SingleAsync(x => x.Id == id);
        }

        public async Task<Teacher> GetTeacherWithSchoolById(int id)
        {
            return await _dbSet.Include("School").SingleAsync(x => x.Id == id);
        }

    }
}
