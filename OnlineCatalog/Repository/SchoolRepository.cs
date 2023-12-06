
using Microsoft.EntityFrameworkCore;
using OnlineCatalog.Data;
using OnlineCatalog.Models;
using OnlineCatalog.Repository.Interfaces;

namespace OnlineCatalog.Repository
{
    public class SchoolRepository : BaseRepository<School>, ISchoolRepository
    {
        public SchoolRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<School> GetSchoolByName(string name)
        {
            return await _dbSet.Where(s => s.Name.Contains(name)).SingleAsync();
        }

        public async Task<School> GetSchoolById(int id)
        {
            return await _dbSet.SingleAsync(x => x.SchoolId == id);
        }

        public async Task<School> GetSchoolByPrinciple(Teacher teacher)
        {
            return await _dbSet.SingleAsync(x => x.Teachers.Contains(teacher));
        }
    }
}
