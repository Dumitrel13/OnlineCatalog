using Microsoft.EntityFrameworkCore;
using OnlineCatalog.Data;
using OnlineCatalog.Models;
using OnlineCatalog.Repository.Interfaces;

namespace OnlineCatalog.Repository
{
    public class TeacherAssignmentRepository : BaseRepository<TeacherAssignment>, ITeacherAssignmentRepository
    {
        public TeacherAssignmentRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<List<TeacherAssignment>> GetTeacherAssignmentsForSpecificClass(int classroomId)
        {
            return await _dbSet.Include("Teacher").Include("Teacher.JobPeriodRoles").Include("Teacher.JobPeriodRoles.Roles")
                .Where(ta => ta.Classroom.ClassId == classroomId).ToListAsync();
        }

        public async Task<List<TeacherAssignment>> GetCurrentTeacherAssignmentsWithDataForSpecificTeacherAndClass(int teacherId, int classroomId)
        {
            return await _dbSet.Include("Subject").Include("ApplicationRole").Where(ta =>
                ta.Teacher.Id == teacherId && ta.Classroom.ClassId == classroomId && DateTime.Now.Date >= ta.StartingDate.Date &&
                DateTime.Now.Date <= ta.EndingDate.Date).ToListAsync();
        }

        public async Task<List<TeacherAssignment>> GetCurrentTeacherAssignmentsForSpecificTeacher(int teacherId)
        {
            return await _dbSet.Include("Classroom").Include("ApplicationRole").Where(ta =>
                ta.Teacher.Id == teacherId && DateTime.Now.Date >= ta.StartingDate.Date &&
                DateTime.Now.Date <= ta.EndingDate.Date).ToListAsync();
        }

        public async Task<List<TeacherAssignment>> GetAllTeacherAssignmentsWithDataByDate(int teacherId, int year)
        {
            return await _dbSet.Include("Teacher").Include("ApplicationRole").Include("Subject")?.Include("Classroom")
                .Where(ta => ta.Teacher.Id == teacherId && ta.EndingDate.Year == year).ToListAsync();
        }

        public async Task<TeacherAssignment> GetTeacherAssignmentById(int assignmentId)
        {
            return await _dbSet.Include("Teacher").Include("Classroom").Include("ApplicationRole").Include("Subject")
                .SingleAsync(ta => ta.TeacherAssignmentId == assignmentId);
        }
    }
}
