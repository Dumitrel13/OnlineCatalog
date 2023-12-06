using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using OnlineCatalog.Models;
using System.Security.Claims;
using OnlineCatalog.ViewModels;

namespace OnlineCatalog.Tests.Resources
{
    public static class PupilResources
    {
        public static Mock<HttpContext> MockHttpContext => new();
        public static List<Pupil> Pupils => new();
        public static Classroom Classroom => new(){Pupils = new List<Pupil>()};
        public static List<TeacherAssignment> TeacherAssignments => new()
        {
            new TeacherAssignment
            {
                TeacherAssignmentId = 1,
                ApplicationRole = new ApplicationRole
                {
                    Name = "Diriginte"
                },
                Classroom = new()
                {
                    ClassId = 1
                },

            }
        };
        public static PupilViewModel PupilViewModel => new()
        {
            Pupil = new Pupil { Email = "Test" },
            SelectedGender = "M"
        };
        public static PupilViewModel PupilViewModelWithoutGender => new()
        {
            Pupil = new Pupil { Email = "Test" },
        };
        public static Pupil Pupil => new(){StartingDate = new DateTime(), EndingDate = new DateTime()};

        public static DeleteUserViewModel DeleteUserViewModel => new() { User = new ApplicationUser() };

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
                .ReturnsAsync(new Pupil { Id = 1, UserName = "Test" });

            mgr.Setup(u => u.GetRolesAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(new List<string> { "Parinte", "Elev" });

            return mgr;
        }
    }
}
