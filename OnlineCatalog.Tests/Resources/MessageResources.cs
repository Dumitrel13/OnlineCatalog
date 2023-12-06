using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using OnlineCatalog.Models;
using System.Security.Claims;
using Amazon.S3.Model;
using OnlineCatalog.ViewModels;

namespace OnlineCatalog.Tests.Resources
{
    public static class MessageResources
    {
        public static Mock<HttpContext> MockHttpContext => new();

        public static Message Message { get; set; } = new()
        {
            MessageId = 1,
            Date = new DateTime(2023, 06, 20),
            Pupil = new Pupil(),
            Teacher = new Teacher()
        };
        public static Pupil Pupil => new() { Id = 1, Classroom = new Classroom() };

        public static List<TeacherAssignment> TeacherAssignments => new()
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

        public static Teacher Teacher => new();

        public static AddMessageViewModel AddMessageViewModel => new()
        {
            Message = new Message()
            {
                Pupil = new Pupil(),
                Teacher = new Teacher()
            },
            SelectedFile = new FormFile(It.IsAny<Stream>(), It.IsAny<long>(), It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>()),
        };

        public static GetObjectResponse GetObjectResponse => new()
        {
            ResponseStream = new MemoryStream(),
            Headers = { ContentType = "application/pdf" }
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
                .ReturnsAsync(new Teacher { Id = 1, UserName = "Test" });

            mgr.Setup(u => u.GetRolesAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(new List<string> { "Parinte", "Elev" });

            return mgr;
        }
    }
}
