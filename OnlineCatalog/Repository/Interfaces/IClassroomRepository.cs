using OnlineCatalog.Models;

namespace OnlineCatalog.Repository.Interfaces
{
    public interface IClassroomRepository : IBaseRepository<Classroom>
    {
        Task<Classroom> GetClassroomByIdAsync(int id);
        Task<Classroom> GetClassroomWithoutDataByIdAsync(int id);
        Task <List<Classroom>> GetAllClassroomsWithPupilsAsync();
    }
}
