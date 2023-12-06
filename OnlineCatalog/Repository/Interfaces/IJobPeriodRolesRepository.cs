using OnlineCatalog.Models;

namespace OnlineCatalog.Repository.Interfaces
{
    public interface IJobPeriodRolesRepository : IBaseRepository<JobPeriodRoles>
    {
        Task<List<JobPeriodRoles>> GetLastJobPeriodRolesForSpecificTeacher(int teacherId);
    }
}
