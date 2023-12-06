using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using OnlineCatalog.Controllers;
using OnlineCatalog.Models;
using OnlineCatalog.Repository.Interfaces;
using OnlineCatalog.Tests.Resources;
using Microsoft.AspNetCore.Mvc;
using OnlineCatalog.Helpers.Interfaces;
using OnlineCatalog.ViewModels;

namespace OnlineCatalog.Tests
{
    public class ParentControllerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly Mock<IHttpContextAccessor> _mockContextAccessor;
        private readonly Mock<IEmailService> _mockEmailService;
        private readonly ParentController _parentController;

        public ParentControllerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockUserManager = ParentResources.MockUserManager();
            _mockContextAccessor = new Mock<IHttpContextAccessor>();
            _mockEmailService = new Mock<IEmailService>();

            _parentController = new ParentController(_mockUnitOfWork.Object, _mockUserManager.Object, _mockContextAccessor.Object, _mockEmailService.Object);
        }

        [Fact]
        public async Task Index_ReturnsAViewResult_WithAListOfParents()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.ParentRepository.GetAllAsync()).ReturnsAsync(ParentResources.Parents);

            //Act
            var result = await _parentController.Index();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<List<Parent>>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Index_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.ParentRepository.GetAllAsync()).ThrowsAsync(new Exception());

            //Act
            var result = await _parentController.Index();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task Add_ReturnsAViewResult_WithParentViewModel()
        {
            //Act
            var result = _parentController.Add();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<ParentViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Add_ReturnsTheSameView_WhenTheEmailIsAlreadyUsed()
        {
            //Arrange
            var userList = new List<ApplicationUser> { new() { Email = "Test" } };
            _mockUserManager.Setup(x => x.Users).Returns(userList.AsQueryable());

            //Act
            var result = await _parentController.Add(ParentResources.ParentViewModel);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<ParentViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Add_ReturnsTheSameView_WhenTheModelStateIsNotValid()
        {
            //Arrange
            var userList = new List<ApplicationUser> { new() { Email = "test" } };
            _mockUserManager.Setup(x => x.Users).Returns(userList.AsQueryable());

            _parentController.ModelState.AddModelError("Error", "Error");

            //Act
            var result = await _parentController.Add(ParentResources.ParentViewModel);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<ParentViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Add_ReturnsTheSameView_WhenTheSelectedGenderIsNullOfEmpty()
        {
            //Arrange
            var userList = new List<ApplicationUser> { new() { Email = "test" } };
            _mockUserManager.Setup(x => x.Users).Returns(userList.AsQueryable());

            //Act
            var result = await _parentController.Add(ParentResources.ParentViewModelWithoutGender);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<ParentViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Add_ReturnsAnErrorView_IfTheEmailFailedToSent()
        {
            //Arrange
            var userList = new List<ApplicationUser> { new() { Email = "test" } };
            _mockUserManager.Setup(x => x.Users).Returns(userList.AsQueryable());
            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            _mockEmailService.Setup(x => x.AccountCreationEmail(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            //Act
            var result = await _parentController.Add(ParentResources.ParentViewModel);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task Add_ReturnsARedirect_AndCreatesAnUserAccount()
        {
            //Arrange
            var userList = new List<ApplicationUser> { new() { Email = "test" } };
            _mockUserManager.Setup(x => x.Users).Returns(userList.AsQueryable());
            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            _mockEmailService.Setup(x => x.AccountCreationEmail(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            //Act
            var result = await _parentController.Add(ParentResources.ParentViewModel);

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
            var result = await _parentController.Add(ParentResources.ParentViewModel);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task Edit_ReturnsAViewResult_WithParentViewModel()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.ParentRepository.GetParentById(It.IsAny<int>()))
                .ReturnsAsync(ParentResources.Parent);

            //Act
            var result = await _parentController.Edit(It.IsAny<int>());

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<ParentViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Edit_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.ParentRepository.GetParentById(It.IsAny<int>()))
                .ThrowsAsync(new Exception());

            //Act
            var result = await _parentController.Edit(It.IsAny<int>());

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task Edit_ReturnsTheSameView_IfTheEmailIsAlreadyUsed()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.ParentRepository.GetParentById(It.IsAny<int>()))
                .ReturnsAsync(new Parent(){Email = "email"});

            var userList = new List<ApplicationUser> { new() { Email = "Test" } };
            _mockUserManager.Setup(x => x.Users).Returns(userList.AsQueryable());

            //Act
            var result = await _parentController.Edit(ParentResources.ParentViewModel);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<ParentViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Edit_ReturnsTheSameView_IfTheModelStateIsNotValid()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.ParentRepository.GetParentById(It.IsAny<int>()))
                .ReturnsAsync(ParentResources.Parent);

            var userList = new List<ApplicationUser> { new() { Email = "test" } };
            _mockUserManager.Setup(x => x.Users).Returns(userList.AsQueryable());

            _parentController.ModelState.AddModelError("Error", "Error");

            //Act
            var result = await _parentController.Edit(ParentResources.ParentViewModel);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<ParentViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Edit_ReturnsARedirect_AndUpdatesParentInformation()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.ParentRepository.GetParentById(It.IsAny<int>()))
                .ReturnsAsync(ParentResources.Parent);
            _mockUnitOfWork.Setup(x => x.ParentRepository.UpdateAsync(It.IsAny<Parent>())).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(x => x.CompleteAsync()).Returns(Task.CompletedTask);

            var userList = new List<ApplicationUser> { new() { Email = "test" } };
            _mockUserManager.Setup(x => x.Users).Returns(userList.AsQueryable());

            //Act
            var result = await _parentController.Edit(ParentResources.ParentViewModel);

            //Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task EditPost_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.ParentRepository.GetParentById(It.IsAny<int>()))
                .ThrowsAsync(new Exception());

            //Act
            var result = await _parentController.Edit(ParentResources.ParentViewModel);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task Details_ReturnsAViewResult_WithParentViewModel()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.ParentRepository.GetParentById(It.IsAny<int>()))
                .ReturnsAsync(ParentResources.Parent);

            //Act
            var result = await _parentController.Details(It.IsAny<int>());

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<ParentViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Details_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.ParentRepository.GetParentById(It.IsAny<int>()))
                .ThrowsAsync(new Exception());

            //Act
            var result = await _parentController.Details(It.IsAny<int>());

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task Delete_ReturnsAViewResult_WithDeleteUserViewModel()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.ParentRepository.GetParentById(It.IsAny<int>()))
                .ReturnsAsync(ParentResources.Parent);

            //Act
            var result = await _parentController.Delete(It.IsAny<int>());

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<DeleteUserViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Delete_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.ParentRepository.GetParentById(It.IsAny<int>()))
                .ThrowsAsync(new Exception());

            //Act
            var result = await _parentController.Delete(It.IsAny<int>());

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task DeleteConfirm_ReturnsARedirect_AndDeletesAnUser()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.ParentRepository.GetParentById(It.IsAny<int>()))
                .ReturnsAsync(ParentResources.Parent);
            _mockUnitOfWork.Setup(x => x.ParentRepository.DeleteAsync(It.IsAny<Parent>())).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(x => x.CompleteAsync()).Returns(Task.CompletedTask);

            //Act
            var result = await _parentController.DeleteConfirm(ParentResources.DeleteUserViewModel);

            //Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task DeleteConfirm_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.ParentRepository.GetParentById(It.IsAny<int>()))
                .ThrowsAsync(new Exception());

            //Act
            var result = await _parentController.DeleteConfirm(ParentResources.DeleteUserViewModel);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task AssignKids_ReturnsAViewResult_WithAssignPupilsViewModel()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.ParentRepository.GetParentById(It.IsAny<int>()))
                .ReturnsAsync(ParentResources.Parent);
            _mockUnitOfWork.Setup(x => x.PupilRepository.GetAllAsync()).ReturnsAsync(ParentResources.Pupils);

            //Act
            var result = await _parentController.AssignKids(It.IsAny<int>());

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<AssignPupilsViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task AssignKids_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.ParentRepository.GetParentById(It.IsAny<int>()))
                .ThrowsAsync(new Exception());

            //Act
            var result = await _parentController.AssignKids(It.IsAny<int>());

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task AssignKids_ReturnsTheSameViewIfNoPupilsAreSelected()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.PupilRepository.GetPupilsByIds(It.IsAny<List<int>>()))
                .ReturnsAsync(new List<Pupil>());
            _mockUnitOfWork.Setup(x => x.PupilRepository.GetAllAsync()).ReturnsAsync(ParentResources.Pupils);
            _mockUnitOfWork.Setup(x => x.ParentRepository.GetParentById(It.IsAny<int>()))
                .ReturnsAsync(ParentResources.Parent);

            //Act
            var result = await _parentController.AssignKids(ParentResources.AssignPupils);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<AssignPupilsViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task AssignKids_ReturnsAView_WithAssignPupilsViewModel()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.PupilRepository.GetPupilsByIds(It.IsAny<List<int>>()))
                .ReturnsAsync(ParentResources.Pupils);
            _mockUnitOfWork.Setup(x => x.PupilRepository.GetAllAsync()).ReturnsAsync(ParentResources.Pupils);
            _mockUnitOfWork.Setup(x => x.ParentRepository.GetParentById(It.IsAny<int>()))
                .ReturnsAsync(ParentResources.Parent);

            //Act
            var result = await _parentController.AssignKids(ParentResources.AssignPupils);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<AssignPupilsViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task AssignKidsPost_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.PupilRepository.GetPupilsByIds(It.IsAny<List<int>>()))
                .ThrowsAsync(new Exception());
            
            //Act
            var result = await _parentController.AssignKids(ParentResources.AssignPupils);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task RemoveFromList_ReturnsARedirect()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.ParentRepository.GetParentById(It.IsAny<int>()))
                .ReturnsAsync(ParentResources.Parent);
            _mockUnitOfWork.Setup(x => x.PupilRepository.GetPupilWithClassroomById(It.IsAny<int>()))
                .ReturnsAsync(ParentResources.Pupils.First());
            _mockUnitOfWork.Setup(x => x.CompleteAsync()).Returns(Task.CompletedTask);

            //Act
            var result = await _parentController.RemoveFromList(It.IsAny<int>(), It.IsAny<int>());

            //Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("AssignKids", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task RemoveFromList_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.ParentRepository.GetParentById(It.IsAny<int>()))
                .ThrowsAsync(new Exception());

            //Act
            var result = await _parentController.RemoveFromList(It.IsAny<int>(), It.IsAny<int>());

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task DisplayMyKids_ReturnsAViewResult_WithAListOfPupils()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.PupilRepository.GetPupilsByParent(It.IsAny<Parent>()))
                .ReturnsAsync(ParentResources.Pupils);

            _mockContextAccessor.SetupGet(x => x.HttpContext).Returns(ParentResources.MockHttpContext.Object);

            //Act
            var result = await _parentController.DisplayMyKids();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<List<Pupil>>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task DisplayMyKids_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.PupilRepository.GetPupilsByParent(It.IsAny<Parent>()))
                .ThrowsAsync(new Exception());

            _mockContextAccessor.SetupGet(x => x.HttpContext).Returns(ParentResources.MockHttpContext.Object);

            //Act
            var result = await _parentController.DisplayMyKids();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);

        }

    }
}
