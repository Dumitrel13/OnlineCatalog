using Microsoft.EntityFrameworkCore;
using OnlineCatalog.Data;
using OnlineCatalog.Models;
using OnlineCatalog.Repository.Interfaces;

namespace OnlineCatalog.Repository
{
    public class JobPeriodRolesRepository : BaseRepository<JobPeriodRoles>, IJobPeriodRolesRepository
    {
        public JobPeriodRolesRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<List<JobPeriodRoles>> GetLastJobPeriodRolesForSpecificTeacher(int teacherId)
        {
            return await _dbSet.Where(x => x.Teacher.Id == teacherId).ToListAsync();
        }
    }
}
