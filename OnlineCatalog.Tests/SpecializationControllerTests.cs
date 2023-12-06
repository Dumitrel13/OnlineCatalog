using Microsoft.AspNetCore.Mvc;
using Moq;
using OnlineCatalog.Controllers;
using OnlineCatalog.Models;
using OnlineCatalog.Repository.Interfaces;
using OnlineCatalog.Tests.Resources;

namespace OnlineCatalog.Tests
{
    public class SpecializationControllerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly SpecializationController _controller;

        public SpecializationControllerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _controller = new SpecializationController(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Index_ReturnsAViewResult_WithAListOfSpecializations()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.SpecializationRepository.GetAllAsync())
                .ReturnsAsync(SpecializationResources.Specializations);

            //Act
            var result = await _controller.Index();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<ICollection<Specialization>>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Index_ReturnAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.SpecializationRepository.GetAllAsync())
                .ThrowsAsync(new Exception());

            //Act
            var result = await _controller.Index();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task AddAndEdit_ReturnsAnEmptyViewResult_WhenTheIdIs0()
        {
            //Act
            var result = await _controller.AddAndEdit(0);

            //Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task AddAndEdit_ReturnsAViewResult_WithASpecialization()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.SpecializationRepository.GetSpecializationByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(SpecializationResources.Specialization);

            //Act
            var result = await _controller.AddAndEdit(1);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<Specialization>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task AddAndEdit_ReturnsARedirectAndUpdatesASpecialization_WhenTheIdIsNot0()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.SpecializationRepository.UpdateAsync(It.IsAny<Specialization>()))
                .Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(x => x.CompleteAsync()).Returns(Task.CompletedTask);

            //Act
            var result = await _controller.AddAndEdit(new Specialization { SpecializationId = 1 });

            //Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task AddAndEdit_ReturnsARedirectAndAddsASpecialization_WhenTheIdIs0()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.SpecializationRepository.AddAsync(It.IsAny<Specialization>()))
                .Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(x => x.CompleteAsync()).Returns(Task.CompletedTask);

            //Act
            var result = await _controller.AddAndEdit(new Specialization { SpecializationId = 0 });

            //Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task AddAndEdit_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.SpecializationRepository.GetSpecializationByIdAsync(It.IsAny<int>()))
                .ThrowsAsync(new Exception());

            //Act
            var result = await _controller.AddAndEdit(1);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task AddAndEditPost_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.SpecializationRepository.UpdateAsync(It.IsAny<Specialization>()))
                .ThrowsAsync(new Exception());

            //Act
            var result = await _controller.AddAndEdit(new Specialization { SpecializationId = 1 });

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task Delete_ReturnsAViewResult_WithASpecialization()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.SpecializationRepository.GetSpecializationByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(SpecializationResources.Specialization);

            //Act
            var result = await _controller.Delete(It.IsAny<int>());

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<Specialization>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Delete_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.SpecializationRepository.GetSpecializationByIdAsync(It.IsAny<int>()))
                .ThrowsAsync(new Exception());

            //Act
            var result = await _controller.Delete(It.IsAny<int>());

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<ErrorViewModel>(viewResult.ViewData.Model);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task DeleteConfirm_ReturnsARedirect_AndDeletesASpecialization()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.SpecializationRepository.GetSpecializationByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(SpecializationResources.Specialization);
            _mockUnitOfWork.Setup(x => x.SpecializationRepository.DeleteAsync(It.IsAny<Specialization>()))
                .Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(x => x.CompleteAsync()).Returns(Task.CompletedTask);

            //Act
            var result = await _controller.DeleteConfirm(It.IsAny<int>());

            //Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task DeleteConfirm_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.SpecializationRepository.GetSpecializationByIdAsync(It.IsAny<int>()))
                .ThrowsAsync(new Exception());

            //Act
            var result = await _controller.DeleteConfirm(It.IsAny<int>());

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<ErrorViewModel>(viewResult.ViewData.Model);
            Assert.Equal("Error", viewResult.ViewName);
        }
    }
}
