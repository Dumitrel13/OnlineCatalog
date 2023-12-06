using Microsoft.AspNetCore.Identity;
using Moq;
using OnlineCatalog.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using OnlineCatalog.ViewModels;

namespace OnlineCatalog.Tests.Resources
{
    public static class AbsenceResources
    {
        public static Classroom Classroom => new()
        {
            ClassId = 1,
            Grade = 9,
            Group = "A",
            Subjects = new List<Subject>
            {
                new() {SubjectId = 1}
            }
        };
        public static Pupil Pupil => new() { Id = 1, Classroom = Classroom };
        public static Mock<HttpContext> MockHttpContext => new();
        public static List<TeacherAssignment> TeacherAssignments => new()
        {
            new TeacherAssignment
            {
                TeacherAssignmentId = 1,
                ApplicationRole = new ApplicationRole
                {
                    Name = "Diriginte"
                },
                Subject = new Subject(){Name = "Profesor"}
            }
        };

        public static List<TeacherAssignment> DifferentTeacherAssignments => new()
        {
            new TeacherAssignment
            {
                TeacherAssignmentId = 1,
                ApplicationRole = new ApplicationRole
                {
                    Name = "Profesor"
                },
                Subject = new Subject()
            }
        };

        public static List<Absence> Absences => new();
        public static List<Absence> DuplicateAbsences => new()
        {
            new Absence
            {
                Date = DateTime.Now,
                StartingHour = new TimeSpan(0, 10, 0, 0),
                EndingHour = new TimeSpan(0, 11, 0, 0)
            }
        };
        public static YearStructure YearStructure => new() { YearStructureId = 1 };
        public static Subject Subject => new() { SubjectId = 1 };
        public static Teacher Teacher => new() { Id = 1 };
        public static AddAbsenceViewModel AddAbsenceViewModel => new()
        {
            Absence = new Absence
            {
                AbsenceId = 1,
                StartingHour = new TimeSpan(0, 10, 0, 0),
                EndingHour = new TimeSpan(0, 11, 0, 0),
                Teacher = Teacher,
                Pupil = Pupil,
                Subject = Subject
            }
        };

        public static Mock<UserManager<ApplicationUser>> MockUserManager()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            var mgr = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);
            mgr.Object.UserValidators.Add(new UserValidator<ApplicationUser>());
            mgr.Object.PasswordValidators.Add(new PasswordValidator<ApplicationUser>());

            mgr.Setup(x => x.DeleteAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Success);
            mgr.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(),
                It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            mgr.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Success);

            mgr.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new ApplicationUser { Id = 1, UserName = "Test" });
            mgr.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new Teacher() { Id = 1, UserName = "Test1" });

            mgr.Setup(u => u.GetRolesAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(new List<string> { "Parinte", "Elev" });

            return mgr;
        }
    }
}
