using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OnlineCatalog.Controllers;
using OnlineCatalog.Models;
using OnlineCatalog.Repository.Interfaces;
using OnlineCatalog.Tests.Resources;
using OnlineCatalog.Helpers.Interfaces;
using OnlineCatalog.ViewModels;

namespace OnlineCatalog.Tests
{
    public class TeacherControllerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly Mock<IEmailService> _mockEmailService;
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private readonly TeacherController _teacherController;

        public TeacherControllerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockUserManager = TeacherResources.MockUserManager();
            _mockEmailService = new Mock<IEmailService>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

            _teacherController = new TeacherController(_mockUnitOfWork.Object, _mockUserManager.Object, _mockEmailService.Object, _mockHttpContextAccessor.Object);
        }

        [Fact]
        public async Task Index_ReturnsAViewResult_WithAListOfTeachers()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.TeacherRepository.GetAllAsync()).ReturnsAsync(TeacherResources.Teachers);
            _mockHttpContextAccessor.SetupGet(x => x.HttpContext).Returns(TeacherResources.MockHttpContext.Object);

            //Act
            var result = await _teacherController.Index();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<List<Teacher>>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Index_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            _mockHttpContextAccessor.SetupGet(x => x.HttpContext).Throws(new Exception());

            //Act
            var result = await _teacherController.Index();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task Add_ReturnsAViewResult_WithTeacherModel()
        {
            //Act
            var result = _teacherController.Add();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<TeacherModel>(viewResult.ViewData.Model);
        }


        [Fact]
        public async Task Add_ReturnsTheSameView_WhenTheEmailIsAlreadyUsed()
        {
            //Arrange
            var userList = new List<ApplicationUser> { new() { Email = "Test" } };
            _mockUserManager.Setup(x => x.Users).Returns(userList.AsQueryable());

            //Act
            var result = await _teacherController.Add(TeacherResources.TeacherModel);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<TeacherModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Add_ReturnsTheSameView_WhenTheModelStateIsNotValid()
        {
            //Arrange
            var userList = new List<ApplicationUser> { new() { Email = "test" } };
            _mockUserManager.Setup(x => x.Users).Returns(userList.AsQueryable());

            _teacherController.ModelState.AddModelError("Error", "Error");

            //Act
            var result = await _teacherController.Add(TeacherResources.TeacherModel);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<TeacherModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Add_ReturnsTheSameView_WhenNoGenderWasSelected()
        {
            //Arrange
            var userList = new List<ApplicationUser> { new() { Email = "test" } };
            _mockUserManager.Setup(x => x.Users).Returns(userList.AsQueryable());

            //Act
            var result = await _teacherController.Add(TeacherResources.TeacherModel);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<TeacherModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Add_ReturnsARedirect_AndCreatesAnUser()
        {
            //Arrange
            var userList = new List<ApplicationUser> { new() { Email = "test" } };
            _mockUserManager.Setup(x => x.Users).Returns(userList.AsQueryable());

            _mockUnitOfWork.Setup(x => x.SchoolRepository.GetSchoolByName(It.IsAny<string>()))
                .ReturnsAsync(TeacherResources.School);
            _mockUnitOfWork.Setup(x => x.TeacherRepository.GetTeacherWithSchoolById(It.IsAny<int>()))
                .ReturnsAsync(TeacherResources.Teacher);
            _mockUnitOfWork
                .Setup(x =>
                    x.TeacherAssignmentRepository.GetCurrentTeacherAssignmentsForSpecificTeacher(It.IsAny<int>()))
                .ReturnsAsync(TeacherResources.TeacherAssignments);
            _mockUnitOfWork.Setup(x => x.YearStructureRepository.GetCurrentYearStructure())
                .ReturnsAsync(TeacherResources.YearStructure);
            _mockUnitOfWork.Setup(x => x.RoleRepository.GetRoleByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(new ApplicationRole());
            _mockUnitOfWork.Setup(x => x.ClassroomRepository.GetAllAsync()).ReturnsAsync(TeacherResources.Classrooms);
            _mockUnitOfWork.Setup(x => x.SubjectRepository.GetAllAsync()).ReturnsAsync(TeacherResources.Subjects);

            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            _mockEmailService.Setup(x => x.AccountCreationEmail(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            //Act
            var result = await _teacherController.Add(TeacherResources.TeacherModelWithGender);

            //Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Add_ReturnsAnErrorView_WhenTheEmailIsNotSent()
        {
            //Arrange
            var userList = new List<ApplicationUser> { new() { Email = "test" } };
            _mockUserManager.Setup(x => x.Users).Returns(userList.AsQueryable());

            _mockUnitOfWork.Setup(x => x.SchoolRepository.GetSchoolByName(It.IsAny<string>()))
                .ReturnsAsync(TeacherResources.School);
            _mockUnitOfWork.Setup(x => x.TeacherRepository.GetTeacherWithSchoolById(It.IsAny<int>()))
                .ReturnsAsync(TeacherResources.Teacher);

            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            _mockEmailService.Setup(x => x.AccountCreationEmail(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            //Act
            var result = await _teacherController.Add(TeacherResources.TeacherModelWithGender);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task Add_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            _mockUserManager.Setup(x => x.Users).Throws(new Exception());

            //Act
            var result = await _teacherController.Add(TeacherResources.TeacherModelWithGender);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task Edit_ReturnsAViewResult_WithTeacherModel()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.TeacherRepository.GetTeacherByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(TeacherResources.Teacher);

            //Act
            var result = await _teacherController.Edit(It.IsAny<int>());

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<TeacherModel>(viewResult.ViewData.Model);
            Assert.Equal("Add", viewResult.ViewName);
        }

        [Fact]
        public async Task Edit_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.TeacherRepository.GetTeacherByIdAsync(It.IsAny<int>()))
                .ThrowsAsync(new Exception());

            //Act
            var result = await _teacherController.Edit(It.IsAny<int>());

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<ErrorViewModel>(viewResult.ViewData.Model);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task Edit_ReturnsTheSameView_WhenTheEmailIsAlreadyUsed()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.TeacherRepository.GetTeacherByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(TeacherResources.Teacher);

            var userList = new List<ApplicationUser> { new() { Email = "Test" } };
            _mockUserManager.Setup(x => x.Users).Returns(userList.AsQueryable());

            //Act
            var result = await _teacherController.Edit(TeacherResources.TeacherModel);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<TeacherModel>(viewResult.ViewData.Model);
            Assert.Equal("Add", viewResult.ViewName);
        }

        [Fact]
        public async Task Edit_ReturnsTheSameView_WhenTheModelStateIsNotValid()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.TeacherRepository.GetTeacherByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(TeacherResources.Teacher);

            var userList = new List<ApplicationUser> { new() { Email = "test" } };
            _mockUserManager.Setup(x => x.Users).Returns(userList.AsQueryable());

            _teacherController.ModelState.AddModelError("Error", "Error");

            //Act
            var result = await _teacherController.Edit(TeacherResources.TeacherModel);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<TeacherModel>(viewResult.ViewData.Model);
            Assert.Equal("Add", viewResult.ViewName);
        }

        [Fact]
        public async Task Edit_ReturnsARedirect_AndUpdatesTheUserInformation()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.TeacherRepository.GetTeacherByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(TeacherResources.Teacher);

            var userList = new List<ApplicationUser> { new() { Email = "test" } };
            _mockUserManager.Setup(x => x.Users).Returns(userList.AsQueryable());
            _mockUnitOfWork.Setup(x => x.TeacherRepository.UpdateAsync(It.IsAny<Teacher>()))
                .Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(x => x.CompleteAsync()).Returns(Task.CompletedTask);

            //Act
            var result = await _teacherController.Edit(TeacherResources.TeacherModelWithGender);

            //Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task EditPost_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.TeacherRepository.GetTeacherByIdAsync(It.IsAny<int>()))
                .ThrowsAsync(new Exception());

            //Act
            var result = await _teacherController.Edit(TeacherResources.TeacherModelWithGender);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task Details_ReturnsAViewResult_WithATeacherModel()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.TeacherRepository.GetTeacherByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(TeacherResources.Teacher);

            //Act
            var result = await _teacherController.Details(It.IsAny<int>());

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<TeacherModel>(viewResult.ViewData.Model);
            Assert.Equal("Add", viewResult.ViewName);
        }

        [Fact]
        public async Task Details_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.TeacherRepository.GetTeacherByIdAsync(It.IsAny<int>()))
                .ThrowsAsync(new Exception());

            //Act
            var result = await _teacherController.Details(It.IsAny<int>());

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task Delete_ReturnsAViewResult_WithDeleteUserViewModel()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.TeacherRepository.GetTeacherByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(TeacherResources.Teacher);

            //Act
            var result = await _teacherController.Delete(It.IsAny<int>());

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<DeleteUserViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Delete_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.TeacherRepository.GetTeacherByIdAsync(It.IsAny<int>()))
                .ThrowsAsync(new Exception());

            //Act
            var result = await _teacherController.Delete(It.IsAny<int>());

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task DeleteConfirm_ReturnsARedirect_AndDeletesAnUser()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.TeacherRepository.GetTeacherByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(TeacherResources.Teacher);
            _mockUnitOfWork.Setup(x => x.TeacherRepository.DeleteAsync(It.IsAny<Teacher>()))
                .Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(x => x.CompleteAsync()).Returns(Task.CompletedTask);

            //Act
            var result = await _teacherController.DeleteConfirm(TeacherResources.DeleteUser);

            //Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task DeleteConfirm_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.TeacherRepository.GetTeacherByIdAsync(It.IsAny<int>()))
                .ThrowsAsync(new Exception());

            //Act
            var result = await _teacherController.DeleteConfirm(TeacherResources.DeleteUser);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }
    }
}
