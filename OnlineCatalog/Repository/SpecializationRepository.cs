using Microsoft.EntityFrameworkCore;
using OnlineCatalog.Data;
using OnlineCatalog.Repository;
using OnlineCatalog.Repository.Interfaces;

namespace OnlineCatalog.Models.Repository
{
    public class SpecializationRepository : BaseRepository<Specialization>, ISpecializationRepository
    {
        public SpecializationRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<Specialization> GetSpecializationByIdAsync(int id)
        {
            return await _dbSet.SingleAsync(s => s.SpecializationId == id);
        }
    }
}
