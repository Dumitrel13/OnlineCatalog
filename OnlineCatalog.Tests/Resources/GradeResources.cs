using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using OnlineCatalog.Models;
using System.Security.Claims;
using OnlineCatalog.ViewModels;

namespace OnlineCatalog.Tests.Resources
{
    public static class GradeResources
    {
        public static List<TeacherAssignment> TeacherAssignments => new()
        {
            new TeacherAssignment
            {
                TeacherAssignmentId = 1,
                ApplicationRole = new ApplicationRole
                {
                    Name = "Profesor"
                },
                Subject = new Subject(),
                Classroom = new Classroom{ClassId = 1}
            }
        };

        public static Pupil PupilWithoutClass => new() { Classroom = null };
        public static Pupil PupilWithClass => new() { Classroom = new Classroom { ClassId = 1 } };

        public static Mock<HttpContext> MockHttpContext => new();
        public static YearStructure YearStructure => new() { YearStructureId = 1 };
        public static List<Grade> Grades => new()
        {
            new Grade { GradeId = 1, Subject = new Subject{Name = "Test"}, Score = "10", GradeType = new GradeType { Type = "Teza" } },
            new Grade { Subject = new Subject{Name = "Test"}, Score = "10", GradeType = new GradeType { Type = "Media" } },
            new Grade { Subject = new Subject{Name = "Test"}, Score = "10", GradeType = new GradeType { Type = "Nota" } },
        };

        public static List<Grade> GradesWithoutFinalGrade => new()
        {
            new Grade { Subject = new Subject{Name = "Test"}, Score = "10", GradeType = new GradeType { Type = "Nota" } },
            new Grade { Subject = new Subject{Name = "Test"}, Score = "10", GradeType = new GradeType { Type = "Nota" } },
        };

        public static List<Grade> RatingGrade => new()
        {
            new Grade() { Score = "Admis" }
        };
        public static List<Subject> Subjects => new() { new Subject { Name = "Test" } };

        public static List<GradeType> GradeTypes => new() { new GradeType { GradeTypeId = 1, Type = "Test" }, new GradeType { Type = "Teza" } };
        public static Teacher Teacher => new();
        public static AddGradeViewModel AddGradeViewModel => new()
        {
            Grade = new Grade { Teacher = new Teacher(), Pupil = new Pupil(), Subject = new Subject() },
            SelectedType = "1",
            SelectedScore = "10"
        };

        public static AddGradeViewModel AddGradeViewModelWithRating => new()
        {
            Grade = new Grade { Teacher = new Teacher(), Pupil = new Pupil(), Subject = new Subject() },
            SelectedType = "1",
            SelectedScore = "Admis"
        };
        public static CalculateFinalGradeViewModel CalculateFinalGradeViewModel => new()
        {
            Subject = new Subject { Name = "Test" }
        };

        public static GradeType GradeType => new() { Type = "Teza" };

        public static Classroom Classroom => new() { Subjects = new List<Subject> { new() { Name = "Subject" } } };


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
                .ReturnsAsync(new Teacher { Id = 1, UserName = "Test" });

            mgr.Setup(u => u.GetRolesAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(new List<string> { "Parinte", "Elev" });

            return mgr;
        }
    }
}
