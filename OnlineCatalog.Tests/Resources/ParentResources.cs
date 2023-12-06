using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using OnlineCatalog.Models;
using System.Security.Claims;
using OnlineCatalog.ViewModels;

namespace OnlineCatalog.Tests.Resources
{
    public static class ParentResources
    {
        public static Mock<HttpContext> MockHttpContext => new();
        public static List<Parent> Parents => new();
        public static Parent Parent => new() { Email = "Test", Pupils = new HashSet<Pupil>() { new() } };
        public static ParentViewModel ParentViewModel => new()
        {
            Parent = new Parent() { Email = "Test" },
            SelectedGender = "M"
        };
        public static ParentViewModel ParentViewModelWithoutGender => new()
        {
            Parent = new Parent() { Email = "Test" }
        };
        public static List<Pupil> Pupils => new(){new Pupil()};

        public static DeleteUserViewModel DeleteUserViewModel => new() { User = new ApplicationUser() };

        public static AssignPupilsViewModel AssignPupils => new()
        {
            Pupils = new List<Pupil> { new() { Id = 1 }, new() { Id = 2 } },
            PupilsIds = "[]",
            Parent = new Parent()
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
                .ReturnsAsync(new Parent() { Id = 1, UserName = "Test" });

            mgr.Setup(u => u.GetRolesAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(new List<string> { "Parinte", "Elev" });

            return mgr;
        }
    }
}
