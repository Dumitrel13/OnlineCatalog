using Microsoft.EntityFrameworkCore;
using OnlineCatalog.Data;
using OnlineCatalog.Models;
using OnlineCatalog.Repository.Interfaces;

namespace OnlineCatalog.Repository
{
    public class ParentRepository : BaseRepository<Parent>, IParentRepository
    {
        public ParentRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Parent> GetParentById(int parentId)
        {
            return await _dbSet.Include("Pupils").SingleAsync(p => p.Id == parentId);
        }
    }
}
