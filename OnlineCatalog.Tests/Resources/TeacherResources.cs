using Microsoft.AspNetCore.Identity;
using Moq;
using OnlineCatalog.Models;
using System.Security.Claims;
using OnlineCatalog.ViewModels;
using Microsoft.AspNetCore.Http;

namespace OnlineCatalog.Tests.Resources
{
    public static class TeacherResources
    {
        public static Mock<HttpContext> MockHttpContext => new();
        public static List<Teacher> Teachers => new();
        public static TeacherModel TeacherModel => new() { Teacher = new Teacher { Email = "Test" } };
        public static TeacherModel TeacherModelWithGender => new()
        {
            Teacher = new Teacher { Email = "Test" },
            SelectedGender = "M",
            IsPrincipal = true
        };
        public static School School => new();
        public static Teacher Teacher => new() { Gender = "F" };
        public static DeleteUserViewModel DeleteUser => new() { User = new ApplicationUser() };
        public static List<TeacherAssignment> TeacherAssignments => new() { new TeacherAssignment { ApplicationRole = new ApplicationRole { Name = "Director" } } };
        public static YearStructure YearStructure => new();
        public static List<Classroom> Classrooms => new() { new Classroom { ClassId = 1 } };
        public static List<Subject> Subjects => new() { new Subject() { SubjectId = 1 } };

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

            mgr.Setup(x => x.GetUsersInRoleAsync(It.IsAny<string>()))
                .ReturnsAsync(new List<ApplicationUser> { new Teacher { Id = 1 } });

            mgr.Setup(u => u.GetRolesAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(new List<string> { "Parinte", "Elev" });

            return mgr;
        }
    }
}
