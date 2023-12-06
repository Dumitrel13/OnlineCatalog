using Microsoft.EntityFrameworkCore;
using OnlineCatalog.Data;
using OnlineCatalog.Models;
using OnlineCatalog.Repository.Interfaces;

namespace OnlineCatalog.Repository
{
    public class ClassroomRepository : BaseRepository<Classroom>, IClassroomRepository
    {
        public ClassroomRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Classroom> GetClassroomByIdAsync(int id)
        {
            return await _dbSet.Include("Pupils").Include("Subjects").SingleAsync(c => c.ClassId == id);
        }

        public async Task<Classroom> GetClassroomWithoutDataByIdAsync(int id)
        {
            return await _dbSet.SingleAsync(c => c.ClassId == id);
        }

        public async Task<List<Classroom>> GetAllClassroomsWithPupilsAsync()
        {
            return await _dbSet.Include("Pupils").ToListAsync();
        }
    }
}
