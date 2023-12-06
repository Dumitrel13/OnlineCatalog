using Microsoft.EntityFrameworkCore;
using OnlineCatalog.Data;
using OnlineCatalog.Models;
using OnlineCatalog.Repository.Interfaces;

namespace OnlineCatalog.Repository
{
    public class SubjectRepository : BaseRepository<Subject>, ISubjectRepository
    {
        public SubjectRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Subject> GetSubjectByIdAsync(int id)
        {
            return await _dbSet.SingleAsync(s => s.SubjectId == id);
        }

        public async Task<List<Subject>> GetSubjectsByIds(List<int> subjectsIds)
        {
            return await _dbSet.Where(s => subjectsIds.Contains(s.SubjectId)).ToListAsync();
        }

        public async Task<Subject> GetSubjectByName(string name)
        {
            return await _dbSet.SingleAsync(s => s.Name.Contains(name));
        }

        public async Task<IEnumerable<Subject>> SearchSubjectAsync(string searchQuery)
        {
            return await _dbSet.Where(s => s.Name.Contains(searchQuery)).ToListAsync();
        }
    }
}
