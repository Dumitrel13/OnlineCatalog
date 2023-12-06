using Microsoft.EntityFrameworkCore;
using OnlineCatalog.Data;
using OnlineCatalog.Models;
using OnlineCatalog.Repository.Interfaces;

namespace OnlineCatalog.Repository
{
    public class GradeTypeRepository : BaseRepository<GradeType>, IGradeTypeRepository
    {
        public GradeTypeRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<GradeType> GetGradeTypeById(int id)
        {
            return await _dbSet.SingleAsync(t => t.GradeTypeId == id);
        }

        public async Task<GradeType> GetGradeTypeByName(string name)
        {
            return await _dbSet.SingleAsync(t => t.Type.Contains(name));
        }
    }
}
