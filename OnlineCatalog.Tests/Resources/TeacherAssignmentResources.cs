using OnlineCatalog.Models;
using OnlineCatalog.ViewModels;

namespace OnlineCatalog.Tests.Resources
{
    public static class TeacherAssignmentResources
    {
        public static List<TeacherAssignment> TeacherAssignments => new();
        public static Teacher Teacher => new();
        public static List<Classroom> Classrooms => new() { new Classroom { ClassId = 1, Grade = 9, Group = "A" } };
        public static List<ApplicationRole> Roles => new() { new ApplicationRole { Id = 1, Name = "Role" } };
        public static List<Subject> Subjects => new() { new Subject { SubjectId = 1, Name = "Subject" } };
        public static YearStructure YearStructure => new();
        public static TeacherAssignmentViewModel TeacherAssignmentViewModelEmpty => new()
        {
            TeacherAssignment = new TeacherAssignment
            {
                Teacher = new Teacher(),
                Classroom = new Classroom(),
                ApplicationRole = new ApplicationRole(),
                Subject = new Subject()
            }
        };
        public static TeacherAssignmentViewModel TeacherAssignmentViewModel => new()
        {
            SelectedClassroom = "1",
            SelectedSubject = "1",
            SelectedRole = "1",
            TeacherAssignment = new TeacherAssignment
            {
                StartingDate = new DateTime(2023, 01, 05),
                EndingDate = new DateTime(2023, 05, 05),
                Teacher = new Teacher(),
                SubjectHours = 8,
                Classroom = new Classroom(),
                ApplicationRole = new ApplicationRole(){Name = "Director"}
            }
        };
        public static TeacherAssignment TeacherAssignment => new() { Classroom = new Classroom(), Teacher = new Teacher() };
        public static Classroom ClassroomWithSubjects => new() { Subjects = new List<Subject> { new() { SubjectId = 1, Name = "Subject" } } };
    }
}
