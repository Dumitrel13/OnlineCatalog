
namespace OnlineCatalog.Repository.Interfaces
{
    public interface IUnitOfWork : IDisposable, IAsyncDisposable
    {
        IRoleRepository RoleRepository { get; }
        ISpecializationRepository SpecializationRepository { get; }
        ISubjectRepository SubjectRepository { get; }
        ITeacherRepository TeacherRepository { get; }
        ISchoolRepository SchoolRepository { get; }
        IJobPeriodRolesRepository JobPeriodRolesRepository { get; }
        IParentRepository ParentRepository { get; }
        IPupilRepository PupilRepository { get; }
        IClassroomRepository ClassroomRepository { get; }
        IYearStructure YearStructureRepository { get; }
        IYearSubjectsPlan YearSubjectsPlanRepository { get; }
        IGradeRepository GradeRepository { get; }
        IGradeTypeRepository GradeTypeRepository { get; }
        IAbsenceRepository AbsenceRepository { get; }
        IMessageRepository MessageRepository { get; }
        ITeacherAssignmentRepository TeacherAssignmentRepository { get; }
        IManageFilesRepository ManageFilesRepository { get; }

        Task CompleteAsync();
    }
}
