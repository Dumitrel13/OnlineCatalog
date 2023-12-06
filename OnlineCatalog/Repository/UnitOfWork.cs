using Amazon.S3;
using OnlineCatalog.Data;
using OnlineCatalog.Models.Repository;
using OnlineCatalog.Repository.Interfaces;

namespace OnlineCatalog.Repository
{
	public class UnitOfWork : IUnitOfWork
	{
        private readonly AppDbContext _context;
        private readonly IAmazonS3 _s3Client;
        private readonly IConfiguration _configuration;

        public UnitOfWork(AppDbContext context, IAmazonS3 s3Client, IConfiguration configuration)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _s3Client = s3Client ?? throw new ArgumentNullException(nameof(s3Client));
            _configuration = configuration;
        }

        private IRoleRepository? _roleRepository;
		public IRoleRepository RoleRepository => _roleRepository ??= new RoleRepository(_context);

        private ISpecializationRepository? _specializationRepository;
        public ISpecializationRepository SpecializationRepository => _specializationRepository ??= new SpecializationRepository(_context);

        private ISubjectRepository? _subjectRepository;
        public ISubjectRepository SubjectRepository => _subjectRepository ??= new SubjectRepository(_context);

        private ITeacherRepository? _teacherRepository;
        public ITeacherRepository TeacherRepository => _teacherRepository ??= new TeacherRepository(_context);

        private ISchoolRepository? _schoolRepository;
        public ISchoolRepository SchoolRepository => _schoolRepository ??= new SchoolRepository(_context);

        private IJobPeriodRolesRepository? _jobPeriodRolesRepository;
        public IJobPeriodRolesRepository JobPeriodRolesRepository =>
            _jobPeriodRolesRepository ??= new JobPeriodRolesRepository(_context);

        private IParentRepository _parentRepository;
        public IParentRepository ParentRepository => _parentRepository ??= new ParentRepository(_context);

        private IPupilRepository _pupilRepository;
        public IPupilRepository PupilRepository => _pupilRepository ??= new PupilRepository(_context);

        private IClassroomRepository _classroomRepository;
        public IClassroomRepository ClassroomRepository => _classroomRepository ??= new ClassroomRepository(_context);

        private IYearStructure _yearStructure;
        public IYearStructure YearStructureRepository => _yearStructure ??= new YearStructureRepository(_context);

        private IYearSubjectsPlan _yearSubjectsPlanRepository;
        public IYearSubjectsPlan YearSubjectsPlanRepository => _yearSubjectsPlanRepository ??= new YearSubjectsPlanRepository(_context);

        private IGradeRepository _gradeRepository;
        public IGradeRepository GradeRepository => _gradeRepository ??= new GradeRepository(_context);

        private IGradeTypeRepository _gradeTypeRepository;
        public IGradeTypeRepository GradeTypeRepository => _gradeTypeRepository ??= new GradeTypeRepository(_context);

        private IAbsenceRepository _absenceRepository;
        public IAbsenceRepository AbsenceRepository => _absenceRepository ??= new AbsenceRepository(_context);

        private IMessageRepository _messageRepository;
        public IMessageRepository MessageRepository => _messageRepository ??= new MessageRepository(_context);
        
        private ITeacherAssignmentRepository _teacherAssignmentRepository;
        public ITeacherAssignmentRepository TeacherAssignmentRepository =>
            _teacherAssignmentRepository ??= new TeacherAssignmentRepository(_context);

        private IManageFilesRepository _manageFilesRepository;
        public IManageFilesRepository ManageFilesRepository => _manageFilesRepository ??= new ManageFilesRepository(_s3Client, _configuration);

        public async Task CompleteAsync()
		{
			await _context.SaveChangesAsync();
		}

		public void Dispose()
		{
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
        }
        public async ValueTask DisposeAsync()
        {
            await DisposeAsync(true);
            GC.SuppressFinalize(this);
        }

        private bool _disposed;
        protected virtual async ValueTask DisposeAsync(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    await _context.DisposeAsync();
                }

                _disposed = true;
            }
        }
    }
}
