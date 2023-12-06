using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OnlineCatalog.Controllers;
using OnlineCatalog.Models;
using OnlineCatalog.Repository.Interfaces;
using OnlineCatalog.Tests.Resources;
using OnlineCatalog.ViewModels;

namespace OnlineCatalog.Tests
{
    public class SchoolControllerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly Mock<IHttpContextAccessor> _mockContextAccessor;
        private SchoolController _schoolController;

        public SchoolControllerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockUserManager = ClassroomResources.MockUserManager();
            _mockContextAccessor = new Mock<IHttpContextAccessor>();

            _schoolController = new SchoolController(_mockUnitOfWork.Object, _mockUserManager.Object, _mockContextAccessor.Object);
        }

        [Fact]
        public async Task Index_ReturnsAViewResult_WithSchoolViewModelWithMessage()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.TeacherRepository.GetTeacherWithSchoolById(It.IsAny<int>()))
                .ReturnsAsync(SchoolResources.TeacherWithoutSchool);

            _mockContextAccessor.SetupGet(x => x.HttpContext).Returns(SchoolResources.MockHttpContext.Object);

            //Act
            var result = await _schoolController.Index();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<SchoolViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Index_ReturnsAViewResult_WithSchoolViewModelWithASchool()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.TeacherRepository.GetTeacherWithSchoolById(It.IsAny<int>()))
                .ReturnsAsync(SchoolResources.TeacherWithSchool);

            _mockContextAccessor.SetupGet(x => x.HttpContext).Returns(SchoolResources.MockHttpContext.Object);

            //Act
            var result = await _schoolController.Index();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<SchoolViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Index_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.TeacherRepository.GetTeacherWithSchoolById(It.IsAny<int>()))
                .ThrowsAsync(new Exception());

            //Act
            var result = await _schoolController.Index();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task Add_ReturnsAnErrorView_WhenThePrincipalHasASchool()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.TeacherRepository.GetTeacherWithSchoolById(It.IsAny<int>()))
                .ReturnsAsync(SchoolResources.TeacherWithSchool);

            _mockContextAccessor.SetupGet(x => x.HttpContext).Returns(SchoolResources.MockHttpContext.Object);

            //Act
            var result = await _schoolController.Add();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<ErrorViewModel>(viewResult.ViewData.Model);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task Add_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.TeacherRepository.GetTeacherWithSchoolById(It.IsAny<int>()))
                .ThrowsAsync(new Exception());

            //Act
            var result = await _schoolController.Add();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task Add_ReturnsAViewResult_WithASchool()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.TeacherRepository.GetTeacherWithSchoolById(It.IsAny<int>()))
                .ReturnsAsync(SchoolResources.TeacherWithoutSchool);

            _mockContextAccessor.SetupGet(x => x.HttpContext).Returns(SchoolResources.MockHttpContext.Object);

            //Act
            var result = await _schoolController.Add();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<School>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Add_ReturnsTheSameView_WhenTheModelStateIsNotValid()
        {
            //Arrange
            _schoolController.ModelState.AddModelError("Error", "Error");

            //Act
            var result = await _schoolController.Add(SchoolResources.School);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<School>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Add_ReturnsARedirect_AndAddsASchool()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.TeacherRepository.GetTeacherWithSchoolById(It.IsAny<int>()))
                .ReturnsAsync(SchoolResources.TeacherWithoutSchool);
            _mockUnitOfWork.Setup(x => x.TeacherRepository.UpdateAsync(It.IsAny<Teacher>()))
                .Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(x => x.SchoolRepository.AddAsync(It.IsAny<School>())).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(x => x.CompleteAsync()).Returns(Task.CompletedTask);

            _mockContextAccessor.SetupGet(x => x.HttpContext).Returns(SchoolResources.MockHttpContext.Object);

            //Act
            var result = await _schoolController.Add(SchoolResources.School);

            //Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task AddPost_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.TeacherRepository.GetTeacherWithSchoolById(It.IsAny<int>()))
                .ThrowsAsync(new Exception());

            //Act
            var result = await _schoolController.Add(SchoolResources.School);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task Edit_ReturnsAViewResult_WithASchool()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.SchoolRepository.GetSchoolById(It.IsAny<int>()))
                .ReturnsAsync(SchoolResources.School);

            //Act
            var result = await _schoolController.Edit(It.IsAny<int>());

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<School>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Edit_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {

            //Arrange
            _mockUnitOfWork.Setup(x => x.SchoolRepository.GetSchoolById(It.IsAny<int>()))
                .ThrowsAsync(new Exception());

            //Act
            var result = await _schoolController.Edit(It.IsAny<int>());

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task Edit_ReturnsTheSameView_WhenTheModelStateIsNotValid()
        {
            //Arrange
            _schoolController.ModelState.AddModelError("Error", "Error");

            //Act
            var result = await _schoolController.Edit(SchoolResources.School);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<School>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Edit_ReturnsARedirect_AndUpdatesTheSchoolDetails()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.SchoolRepository.UpdateAsync(It.IsAny<School>())).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(x => x.CompleteAsync()).Returns(Task.CompletedTask);

            //Act
            var result = await _schoolController.Edit(SchoolResources.School);

            //Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task EditPost_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.SchoolRepository.UpdateAsync(It.IsAny<School>())).ThrowsAsync(new Exception());

            //Act
            var result = await _schoolController.Edit(SchoolResources.School);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);

        }
    }
}
