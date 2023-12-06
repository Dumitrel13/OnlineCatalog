using OnlineCatalog.Models;

namespace OnlineCatalog.Repository.Interfaces
{
    public interface ITeacherAssignmentRepository : IBaseRepository<TeacherAssignment>
    {
        Task<List<TeacherAssignment>> GetTeacherAssignmentsForSpecificClass(int classroomId);

        Task<List<TeacherAssignment>> GetCurrentTeacherAssignmentsWithDataForSpecificTeacherAndClass(int teacherId,
            int classroomId);

        Task<List<TeacherAssignment>> GetCurrentTeacherAssignmentsForSpecificTeacher(int teacherId);
        Task<List<TeacherAssignment>> GetAllTeacherAssignmentsWithDataByDate(int teacherId, int year);
        Task<TeacherAssignment> GetTeacherAssignmentById(int assignmentId);
    }
}
