using Microsoft.AspNetCore.Identity;
using Moq;
using OnlineCatalog.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineCatalog.ViewModels;
using Microsoft.AspNetCore.Http;

namespace OnlineCatalog.Tests.Resources
{
    public static class ClassroomResources
    {
        public static List<Classroom> Classrooms => new();
        public static School School => new();
        public static ClassroomViewModel ClassroomViewModel => new ClassroomViewModel()
        {
            Grades = new List<SelectListItem> { new() { Value = "1", Text = "1" } },
            Groups = new List<SelectListItem> { new() { Value = "1", Text = "A" } },
            SelectedGrade = "1",
            SelectedGroup = "1"
        };
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
                Classroom = new Classroom{Grade = 9, Group = "A"}
            }
        };
        public static Classroom Classroom => new() { Subjects = new List<Subject> { new() { Name = "Subject" } }, Pupils = new List<Pupil>() };
        public static List<Subject> Subjects => new() { new Subject { Name = "Test" } };

        public static AssignSubjectsViewModel AssignSubjectsViewModel => new() { SubjectsId = "", Classroom = new Classroom { ClassId = 1 } };
        public static AssignSubjectsViewModel AssignSubjectsViewModelWithSubjectIds => new()
        {
            SubjectsId = "[1, 2, 3]",
            Classroom = new Classroom { ClassId = 1 },
            Subjects = new List<Subject>()
        };

        public static List<Pupil> Pupils => new() { new Pupil { Classroom = null } };

        public static AssignPupilsViewModel AssignPupilsViewModel =>
            new() { PupilsIds = "", Classroom = new Classroom { ClassId = 1 } };
        public static AssignPupilsViewModel AssignPupilsViewModelWithPupilIds =>
            new() { PupilsIds = "[1, 2, 3]", Classroom = new Classroom { ClassId = 1 } };

        public static Teacher Teacher => new() { School = new School() };

        public static List<Classroom> ClassroomsWithPupils => new()
        {
            new Classroom { Grade = 5, Group = "A", Pupils = new List<Pupil>(Pupils) },
            new Classroom {Grade = 6, Group = "A", Pupils = new List<Pupil>(Pupils)}
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
            //mgr.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            //    .ReturnsAsync(new Teacher() { Id = 1, UserName = "Test1" });

            mgr.Setup(x => x.GetUsersInRoleAsync(It.IsAny<string>()))
                .ReturnsAsync(new List<ApplicationUser> { new Teacher { Id = 1 } });

            mgr.Setup(u => u.GetRolesAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(new List<string> { "Parinte", "Elev" });

            return mgr;
        }
    }
}
