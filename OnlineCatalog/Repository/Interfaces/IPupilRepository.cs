using OnlineCatalog.Models;

namespace OnlineCatalog.Repository.Interfaces
{
    public interface IPupilRepository : IBaseRepository<Pupil>
    {
        Task<Pupil> GetPupilWithClassroomByIdForParent(int pupilId, int parentId);
        Task<Pupil> GetPupilWithClassroomById(int id);
        Task<List<Pupil>> GetPupilsByParent(Parent parent);
        Task<List<Pupil>> GetAllPupilsWithClassroom();
        Task<List<Pupil>> GetPupilsByIds(List<int>  pupilIds);

    }
}
