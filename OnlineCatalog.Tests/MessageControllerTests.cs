using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Moq;
using OnlineCatalog.Controllers;
using OnlineCatalog.Models;
using OnlineCatalog.Repository.Interfaces;
using OnlineCatalog.Tests.Resources;
using OnlineCatalog.ViewModels;

namespace OnlineCatalog.Tests
{
    public class MessageControllerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly Mock<IHttpContextAccessor> _mockContextAccessor;
        private readonly Mock<IStringLocalizer<SharedResources>> _mockStringLocalizer;
        private MessageController _messageController;

        public MessageControllerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockUserManager = MessageResources.MockUserManager();
            _mockContextAccessor = new Mock<IHttpContextAccessor>();
            _mockStringLocalizer = new Mock<IStringLocalizer<SharedResources>>();

            _messageController = new MessageController(_mockUnitOfWork.Object, _mockUserManager.Object, _mockContextAccessor.Object,
                _mockStringLocalizer.Object);
        }

        [Fact]
        public async Task IndexParent_ReturnsAViewResult_WithATuple()
        {
            //Act
            var result = await _messageController.IndexParent(It.IsAny<int>());

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<Tuple<int, bool>>(viewResult.ViewData.Model);
            Assert.Equal("Index", viewResult.ViewName);
        }

        [Fact]
        public async Task IndexClassMaster_ReturnsAViewResult_WithATuple()
        {
            //Arrange
            _mockContextAccessor.SetupGet(x => x.HttpContext).Returns(MessageResources.MockHttpContext.Object);

            //Act
            var result = await _messageController.IndexClassMaster();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<Tuple<int, bool>>(viewResult.ViewData.Model);
            Assert.Equal("Index", viewResult.ViewName);

        }

        [Fact]
        public async Task IndexClassMaster_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            _mockContextAccessor.SetupGet(x => x.HttpContext).Throws(new Exception());

            //Act
            var result = await _messageController.IndexClassMaster();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Theory]
        [InlineData("")]
        [InlineData("path")]
        public async Task Details_ReturnsAViewComponent_WithAddMessageViewModel(string imagePath)
        {
            //Arrange
            MessageResources.Message.ImagePath = imagePath;

            _mockUnitOfWork.Setup(x => x.MessageRepository.GetMessageById(It.IsAny<int>()))
                .ReturnsAsync(MessageResources.Message);
            _mockUnitOfWork.Setup(x => x.MessageRepository.UpdateAsync(It.IsAny<Message>()))
                .Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(x => x.CompleteAsync()).Returns(Task.CompletedTask);

            //Act
            var result = await _messageController.Details(It.IsAny<int>());

            //Assert
            var viewResult = Assert.IsType<ViewComponentResult>(result);
            Assert.IsAssignableFrom<AddMessageViewModel>(viewResult.Arguments);
        }

        [Fact]
        public async Task Details_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.MessageRepository.GetMessageById(It.IsAny<int>()))
                .ThrowsAsync(new Exception());

            //Act
            var result = await _messageController.Details(It.IsAny<int>());

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<ErrorViewModel>(viewResult.ViewData.Model);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task Add_ReturnsAnErrorView_WhenThereAreNoTeacherAssignments()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.PupilRepository.GetPupilWithClassroomById(It.IsAny<int>()))
                .ReturnsAsync(MessageResources.Pupil);
            _mockUnitOfWork
                .Setup(x =>
                    x.TeacherAssignmentRepository.GetCurrentTeacherAssignmentsWithDataForSpecificTeacherAndClass(
                        It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new List<TeacherAssignment>());

            _mockContextAccessor.SetupGet(x => x.HttpContext).Returns(MessageResources.MockHttpContext.Object);

            //Act
            var result = await _messageController.Add(It.IsAny<int>());

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<ErrorViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Add_ReturnsAViewResult_WithAddMessageViewModel()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.PupilRepository.GetPupilWithClassroomById(It.IsAny<int>()))
                .ReturnsAsync(MessageResources.Pupil);
            _mockUnitOfWork
                .Setup(x =>
                    x.TeacherAssignmentRepository.GetCurrentTeacherAssignmentsWithDataForSpecificTeacherAndClass(
                        It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(MessageResources.TeacherAssignments);

            _mockContextAccessor.SetupGet(x => x.HttpContext).Returns(MessageResources.MockHttpContext.Object);

            //Act
            var result = await _messageController.Add(It.IsAny<int>());

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<AddMessageViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Add_ReturnsTheSameView_IfTheModelStateIsNotValid()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.PupilRepository.GetPupilWithClassroomById(It.IsAny<int>()))
                .ReturnsAsync(MessageResources.Pupil);
            _mockUnitOfWork.Setup(x => x.TeacherRepository.GetTeacherByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(MessageResources.Teacher);

            _messageController.ModelState.AddModelError("Error", "Error");

            //Act
            var result = await _messageController.Add(MessageResources.AddMessageViewModel);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<AddMessageViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Add_ReturnsARedirect_AndAddsANewMessage()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.PupilRepository.GetPupilWithClassroomById(It.IsAny<int>()))
                .ReturnsAsync(MessageResources.Pupil);
            _mockUnitOfWork.Setup(x => x.TeacherRepository.GetTeacherByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(MessageResources.Teacher);
            _mockUnitOfWork.Setup(x => x.MessageRepository.AddAsync(It.IsAny<Message>())).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(x => x.CompleteAsync()).Returns(Task.CompletedTask);

            //Act
            var result = await _messageController.Add(MessageResources.AddMessageViewModel);

            //Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Home", redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Add_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.PupilRepository.GetPupilWithClassroomById(It.IsAny<int>()))
                .ThrowsAsync(new Exception());
           
            //Act
            var result = await _messageController.Add(MessageResources.AddMessageViewModel);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<ErrorViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Send_ReturnsTheMainMessageViewWithATuple_WhenTheModelStateIsNotValid()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.PupilRepository.GetPupilWithClassroomById(It.IsAny<int>()))
                .ReturnsAsync(MessageResources.Pupil);
            _mockUnitOfWork.Setup(x => x.TeacherRepository.GetTeacherByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(MessageResources.Teacher);

            _messageController.ModelState.AddModelError("Error", "Error");

            //Act
            var result = await _messageController.Send(MessageResources.AddMessageViewModel);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<Tuple<int, bool>>(viewResult.ViewData.Model);
            Assert.Equal("Index", viewResult.ViewName);
        }

        [Fact]
        public async Task Send_ReturnsTheMainMessageViewWithATuple_WhenTheUploadFileResponseIsFalse()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.PupilRepository.GetPupilWithClassroomById(It.IsAny<int>()))
                .ReturnsAsync(MessageResources.Pupil);
            _mockUnitOfWork.Setup(x => x.TeacherRepository.GetTeacherByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(MessageResources.Teacher);

            _mockUnitOfWork
                .Setup(x => x.ManageFilesRepository.UploadFileAsync(It.IsAny<IFormFile>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            //Act
            var result = await _messageController.Send(MessageResources.AddMessageViewModel);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<Tuple<int, bool>>(viewResult.ViewData.Model);
            Assert.Equal("Index", viewResult.ViewName);
        }

        [Fact]
        public async Task Send_ReturnsTheMainMessageViewWithATuple_AndAddsANewMessage()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.PupilRepository.GetPupilWithClassroomById(It.IsAny<int>()))
                .ReturnsAsync(MessageResources.Pupil);
            _mockUnitOfWork.Setup(x => x.TeacherRepository.GetTeacherByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(MessageResources.Teacher);
            _mockUnitOfWork
                .Setup(x => x.ManageFilesRepository.UploadFileAsync(It.IsAny<IFormFile>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);
            _mockUnitOfWork.Setup(x => x.MessageRepository.AddAsync(It.IsAny<Message>())).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(x => x.CompleteAsync()).Returns(Task.CompletedTask);

            //Act
            var result = await _messageController.Send(MessageResources.AddMessageViewModel);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<Tuple<int, bool>>(viewResult.ViewData.Model);
            Assert.Equal("Index", viewResult.ViewName);
        }

        [Fact]
        public async Task Send_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.PupilRepository.GetPupilWithClassroomById(It.IsAny<int>()))
                .ThrowsAsync(new Exception());

            //Act
            var result = await _messageController.Send(MessageResources.AddMessageViewModel);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<ErrorViewModel>(viewResult.ViewData.Model);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task DownloadFile_ReturnsAnErrorView_WhenTheKeyIsNullOrEmpty()
        {
            //Act
            var result = await _messageController.DownloadFile(It.IsAny<string>(), null);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<ErrorViewModel>(viewResult.ViewData.Model);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task DownloadFile_ReturnsAFile_WhenTheKeyIsCorrect()
        {
            //Arrange
            _mockUnitOfWork
                .Setup(x => x.ManageFilesRepository.GetFileByKeyAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(MessageResources.GetObjectResponse);

            //Act
            var result = await _messageController.DownloadFile(It.IsAny<string>(), "key");

            //Assert
           Assert.IsType<FileStreamResult>(result);
        }

        [Fact]
        public async Task DownloadFile_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            _mockUnitOfWork
                .Setup(x => x.ManageFilesRepository.GetFileByKeyAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception());

            //Act
            var result = await _messageController.DownloadFile(It.IsAny<string>(), "key");

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }
    }
}
