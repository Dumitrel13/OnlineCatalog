using OnlineCatalog.Models;

namespace OnlineCatalog.Repository.Interfaces
{
	public interface ITeacherRepository : IBaseRepository<Teacher>
	{
		Task<Teacher> GetTeacherByIdAsync(int id);
        Task<Teacher> GetTeacherWithSchoolById(int id);
    }
}
