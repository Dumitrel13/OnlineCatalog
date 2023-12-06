using OnlineCatalog.Models;

namespace OnlineCatalog.Repository.Interfaces
{
	public interface IRoleRepository : IBaseRepository<ApplicationRole>
	{
        Task<ApplicationRole> GetRoleByIdAsync(int id);
        Task<ApplicationRole> GetRoleByNameAsync(string roleName);
        Task<IEnumerable<ApplicationRole>> SearchRoleAsync(string searchQuery);
    }
}
