using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OnlineCatalog.Controllers;
using OnlineCatalog.Helpers.Interfaces;
using OnlineCatalog.Models;
using OnlineCatalog.Repository.Interfaces;
using OnlineCatalog.Tests.Resources;
using OnlineCatalog.ViewModels;

namespace OnlineCatalog.Tests
{
    public class PupilControllerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly Mock<IHttpContextAccessor> _mockContextAccessor;
        private readonly Mock<IEmailService> _mockEmailService;
        private PupilController _pupilController;


        public PupilControllerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockUserManager = PupilResources.MockUserManager();
            _mockContextAccessor = new Mock<IHttpContextAccessor>();
            _mockEmailService = new Mock<IEmailService>();

            _pupilController = new PupilController(_mockUnitOfWork.Object, _mockUserManager.Object, _mockContextAccessor.Object, _mockEmailService.Object);
        }


        [Fact]
        public async Task Index_ReturnsAViewResult_WithAListOfPupils()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.PupilRepository.GetAllAsync()).ReturnsAsync(PupilResources.Pupils);

            //Act
            var result = await _pupilController.Index();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<List<Pupil>>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Index_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.PupilRepository.GetAllAsync()).ThrowsAsync(new Exception());

            //Act
            var result = await _pupilController.Index();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task DisplaySpecificPupils_ReturnsAView_WithAClassroom()
        {
           //Arrange
           _mockUnitOfWork.Setup(x => x.ClassroomRepository.GetClassroomByIdAsync(It.IsAny<int>()))
               .ReturnsAsync(PupilResources.Classroom);
           _mockUnitOfWork
               .Setup(
                   x => x.TeacherAssignmentRepository.GetCurrentTeacherAssignmentsForSpecificTeacher(It.IsAny<int>()))
               .ReturnsAsync(PupilResources.TeacherAssignments);

           _mockContextAccessor.SetupGet(x => x.HttpContext).Returns(PupilResources.MockHttpContext.Object);

            //Act
            var result = await _pupilController.DisplaySpecificPupils(1);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<List<Pupil>>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task DisplaySpecificPupils_ReturnsAnErrorView_WhenThereAreNoTeacherAssignmentsForTheClass()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.ClassroomRepository.GetClassroomByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(PupilResources.Classroom);
            _mockUnitOfWork
                .Setup(
                    x => x.TeacherAssignmentRepository.GetCurrentTeacherAssignmentsForSpecificTeacher(It.IsAny<int>()))
                .ReturnsAsync(PupilResources.TeacherAssignments);

            _mockContextAccessor.SetupGet(x => x.HttpContext).Returns(PupilResources.MockHttpContext.Object);

            //Act
            var result = await _pupilController.DisplaySpecificPupils(It.IsAny<int>());

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<ErrorViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task DisplayMyData_ReturnsAViewWithAListOfPupils()
        {
            //Arrange
            _mockContextAccessor.SetupGet(x => x.HttpContext).Returns(PupilResources.MockHttpContext.Object);

            //Act
            var result = await _pupilController.DisplayMyData();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<List<Pupil>>(viewResult.ViewData.Model);
            Assert.Equal("DisplaySpecificPupils", viewResult.ViewName);
        }

        [Fact]
        public async Task DisplayMyData_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            _mockContextAccessor.SetupGet(x => x.HttpContext).Throws(new Exception());

            //Act
            var result = await _pupilController.DisplayMyData();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task Add_ReturnsAViewResult_WithPupilViewModel()
        {
            //Act
            var result = _pupilController.Add();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<PupilViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Add_ReturnsTheSameView_WhenTheEmailIsAlreadyUsed()
        {
            //Arrange
            var userList = new List<ApplicationUser> { new() { Email = "Test" } };
            _mockUserManager.Setup(x => x.Users).Returns(userList.AsQueryable());

            //Act
            var result = await _pupilController.Add(PupilResources.PupilViewModel);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<PupilViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Add_ReturnsTheSameView_WhenTheModelStateIsNotValid()
        {
            //Arrange
            var userList = new List<ApplicationUser> { new() { Email = "test" } };
            _mockUserManager.Setup(x => x.Users).Returns(userList.AsQueryable());

            _pupilController.ModelState.AddModelError("Error", "Error");

            //Act
            var result = await _pupilController.Add(PupilResources.PupilViewModel);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<PupilViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Add_ReturnsTheSameView_WhenNoGenderWasSelected()
        {
            //Arrange
            var userList = new List<ApplicationUser> { new() { Email = "test" } };
            _mockUserManager.Setup(x => x.Users).Returns(userList.AsQueryable());

            //Act
            var result = await _pupilController.Add(PupilResources.PupilViewModelWithoutGender);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<PupilViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Add_ReturnsARedirect_AndCreatesAnUser()
        {
            //Arrange

            var userList = new List<ApplicationUser> { new() { Email = "test" } };
            _mockUserManager.Setup(x => x.Users).Returns(userList.AsQueryable());
            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            _mockEmailService.Setup(x => x.AccountCreationEmail(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            //Act
            var result = await _pupilController.Add(PupilResources.PupilViewModel);

            //Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Add_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            var userList = new List<ApplicationUser> { new() { Email = "test" } };
            _mockUserManager.Setup(x => x.Users).Throws(new Exception());

            //Act
            var result = await _pupilController.Add(PupilResources.PupilViewModel);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task Add_ReturnsAnErrorView_WhenTheEmailFailedToSent()
        {
            //Arrange

            var userList = new List<ApplicationUser> { new() { Email = "test" } };
            _mockUserManager.Setup(x => x.Users).Returns(userList.AsQueryable());
            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            _mockEmailService.Setup(x => x.AccountCreationEmail(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            //Act
            var result = await _pupilController.Add(PupilResources.PupilViewModel);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task Edit_ReturnsAViewResult_WithPupilViewModel()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.PupilRepository.GetPupilWithClassroomById(It.IsAny<int>()))
                .ReturnsAsync(PupilResources.Pupil);

            //Act
            var result = await _pupilController.Edit(It.IsAny<int>());

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<PupilViewModel>(viewResult.ViewData.Model);
            Assert.Equal("Add", viewResult.ViewName);
        }

        [Fact]
        public async Task Edit_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {

            //Arrange
            _mockUnitOfWork.Setup(x => x.PupilRepository.GetPupilWithClassroomById(It.IsAny<int>()))
                .ThrowsAsync(new Exception());

            //Act
            var result = await _pupilController.Edit(It.IsAny<int>());

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task Edit_ReturnsTheSameView_WhenTheEmailIsAlreadyUsed()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.PupilRepository.GetPupilWithClassroomById(It.IsAny<int>()))
                .ReturnsAsync(PupilResources.Pupil);

            var userList = new List<ApplicationUser> { new() { Email = "Test" } };
            _mockUserManager.Setup(x => x.Users).Returns(userList.AsQueryable());

            //Act
            var result = await _pupilController.Edit(PupilResources.PupilViewModel);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<PupilViewModel>(viewResult.ViewData.Model);
            Assert.Equal("Add", viewResult.ViewName);
        }

        [Fact]
        public async Task Edit_ReturnsTheSameView_WhenTheModelStateIsNotValid()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.PupilRepository.GetPupilWithClassroomById(It.IsAny<int>()))
                .ReturnsAsync(PupilResources.Pupil);

            var userList = new List<ApplicationUser> { new() { Email = "test" } };
            _mockUserManager.Setup(x => x.Users).Returns(userList.AsQueryable());

            _pupilController.ModelState.AddModelError("Error", "Error");

            //Act
            var result = await _pupilController.Edit(PupilResources.PupilViewModel);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<PupilViewModel>(viewResult.ViewData.Model);
            Assert.Equal("Add", viewResult.ViewName);
        }

        [Fact]
        public async Task Edit_ReturnsARedirect_AndUpdatesThePupilInformation()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.PupilRepository.GetPupilWithClassroomById(It.IsAny<int>()))
                .ReturnsAsync(PupilResources.Pupil);
            _mockUnitOfWork.Setup(x => x.PupilRepository.UpdateAsync(It.IsAny<Pupil>())).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(x => x.CompleteAsync()).Returns(Task.CompletedTask);

            var userList = new List<ApplicationUser> { new() { Email = "test" } };
            _mockUserManager.Setup(x => x.Users).Returns(userList.AsQueryable());

            //Act
            var result = await _pupilController.Edit(PupilResources.PupilViewModel);

            //Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task EditPost_ReturnsAnErrorViewModel_WhenAnExceptionIsCaught()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.PupilRepository.GetPupilWithClassroomById(It.IsAny<int>()))
                .ThrowsAsync(new Exception());

            //Act
            var result = await _pupilController.Edit(PupilResources.PupilViewModel);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task Details_ReturnsAViewResult_WithPupilViewModel()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.PupilRepository.GetPupilWithClassroomById(It.IsAny<int>()))
                .ReturnsAsync(PupilResources.Pupil);

            //Act
            var result = await _pupilController.Details(It.IsAny<int>());

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<PupilViewModel>(viewResult.ViewData.Model);
            Assert.Equal("Add", viewResult.ViewName);
        }

        [Fact]
        public async Task Details_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.PupilRepository.GetPupilWithClassroomById(It.IsAny<int>()))
                .ThrowsAsync(new Exception());

            //Act
            var result = await _pupilController.Details(It.IsAny<int>());

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task Delete_ReturnsAViewResult_WithDeleteUserViewModel()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.PupilRepository.GetPupilWithClassroomById(It.IsAny<int>()))
                .ReturnsAsync(PupilResources.Pupil);

            //Act
            var result = await _pupilController.Delete(It.IsAny<int>());

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<DeleteUserViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Delete_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.PupilRepository.GetPupilWithClassroomById(It.IsAny<int>()))
                .ThrowsAsync(new Exception());

            //Act
            var result = await _pupilController.Delete(It.IsAny<int>());

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task DeleteConfirm_ReturnsARedirect_AndDeletesAnUser()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.PupilRepository.GetPupilWithClassroomById(It.IsAny<int>()))
                .ReturnsAsync(PupilResources.Pupil);
            _mockUnitOfWork.Setup(x => x.PupilRepository.DeleteAsync(It.IsAny<Pupil>())).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(x => x.CompleteAsync()).Returns(Task.CompletedTask);

            //Act
            var result = await _pupilController.DeleteConfirm(PupilResources.DeleteUserViewModel);

            //Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task DeleteConfirm_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.PupilRepository.GetPupilWithClassroomById(It.IsAny<int>()))
                .ThrowsAsync(new Exception());

            //Act
            var result = await _pupilController.DeleteConfirm(PupilResources.DeleteUserViewModel);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }
    }
}
