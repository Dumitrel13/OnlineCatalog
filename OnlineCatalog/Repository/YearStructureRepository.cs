using Microsoft.EntityFrameworkCore;
using OnlineCatalog.Data;
using OnlineCatalog.Models;
using OnlineCatalog.Repository.Interfaces;

namespace OnlineCatalog.Repository
{
    public class YearStructureRepository :  BaseRepository<YearStructure>, IYearStructure
    {
        public YearStructureRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<YearStructure> GetCurrentYearStructure()
        {
            return await _dbSet.SingleAsync(x => DateTime.Now.Date >= x.StartingYear && DateTime.Now.Date <= x.EndingYear);
        }

        public async Task<YearStructure> GetYearStructureById(int id)
        {
            return await _dbSet.Include("Periods").SingleAsync(x => x.YearStructureId == id);
        }
    }
}
