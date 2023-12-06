using OnlineCatalog.Models;

namespace OnlineCatalog.Repository.Interfaces
{
	public interface ISpecializationRepository : IBaseRepository<Specialization>
	{
		Task<Specialization> GetSpecializationByIdAsync(int id);
    }
}
