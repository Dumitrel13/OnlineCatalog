using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineCatalog.Models;

namespace OnlineCatalog.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public AppDbContext()
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public virtual DbSet<ApplicationUser> Users { get; set; }
        public virtual DbSet<Absence> Absences { get; set; }
        public virtual DbSet<Classroom> Classrooms { get; set; }
        public virtual DbSet<ClassType> ClassroomTypes { get; set; }
        public virtual DbSet<FailedSubject> FailedSubjects { get; set; }
        public virtual DbSet<Grade> Grades { get; set; }
        public virtual DbSet<JobPeriodRoles> JobPeriodRoles { get; set; }
        public virtual DbSet<JobPeriodSubjects> JobPeriodSubjects { get; set; }
        public virtual DbSet<Message> Messages { get; set; }    
        public virtual DbSet<Parent> Parents { get; set; }
        public virtual DbSet<Period> Periods { get; set; }
        public virtual DbSet<Pupil> Pupils { get; set; }
        public virtual DbSet<ApplicationRole> Roles { get; set; }
        public virtual DbSet<School> Schools { get; set; }
        public virtual DbSet<SchoolarYear> ScholarYears { get; set; }
        public virtual DbSet<Specialization> Specializations { get; set; }
        public virtual DbSet<Subject> Subjects { get; set; }
        public virtual DbSet<Teacher> Teachers { get; set; }
        public virtual DbSet<TeacherAssignment> TeacherAssignments { get; set; }
        public virtual DbSet<YearStructure> YearStructures { get; set; }
        public virtual DbSet<YearSubjectsPlan> YearSubjectsPlans { get; set; }
    }
}
