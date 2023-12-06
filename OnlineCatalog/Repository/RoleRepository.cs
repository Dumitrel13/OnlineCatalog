using Microsoft.EntityFrameworkCore;
using OnlineCatalog.Data;
using OnlineCatalog.Repository;
using OnlineCatalog.Repository.Interfaces;

namespace OnlineCatalog.Models.Repository
{
	public class RoleRepository : BaseRepository<ApplicationRole>, IRoleRepository
	{
		public RoleRepository(AppDbContext context) : base(context)
		{
		}

		public async Task<ApplicationRole> GetRoleByIdAsync(int id)
		{
            return await _dbSet.SingleAsync(r => r.Id == id);
        }

        public Task<ApplicationRole> GetRoleByNameAsync(string roleName)
        {
            return _dbSet.SingleAsync(r => r.Name.Contains(roleName));
        }

        public Task<IEnumerable<ApplicationRole>> SearchRoleAsync(string searchQuery)
		{
			throw new NotImplementedException();
		}
	}
}
