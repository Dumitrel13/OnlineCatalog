using Microsoft.EntityFrameworkCore;
using OnlineCatalog.Data;
using OnlineCatalog.Models;
using OnlineCatalog.Repository.Interfaces;

namespace OnlineCatalog.Repository
{
    public class PupilRepository : BaseRepository<Pupil>, IPupilRepository

    {
        public PupilRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Pupil> GetPupilWithClassroomByIdForParent(int pupilId, int parentId)
        {
			return await _dbSet.Include("Classroom").SingleAsync(p => p.Id == pupilId && p.Parents.Any(pa => pa.Id == parentId));
		}

        public async Task<Pupil> GetPupilWithClassroomById(int id)
        {
            return await _dbSet.Include("Classroom").SingleAsync(p => p.Id == id);
        }

        public async Task<List<Pupil>> GetPupilsByParent(Parent parent)
        {
            return await _dbSet.Where(p => p.Parents.Any(x => x.Id == parent.Id)).ToListAsync();
        }

        public async Task<List<Pupil>> GetAllPupilsWithClassroom()
        {
            return await _dbSet.Include("Classroom").ToListAsync();
        }

        public async Task<List<Pupil>> GetPupilsByIds(List<int> pupilIds)
        {
            return await _dbSet.Where(p => pupilIds.Contains(p.Id)).ToListAsync();
        }
    }
}
