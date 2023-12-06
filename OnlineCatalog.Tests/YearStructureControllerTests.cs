using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Moq;
using OnlineCatalog.Controllers;
using OnlineCatalog.Models;
using OnlineCatalog.Repository.Interfaces;
using OnlineCatalog.Tests.Resources;

namespace OnlineCatalog.Tests
{
    public class YearStructureControllerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly YearStructureController _yearStructureController;
        private readonly Mock<IHttpContextAccessor> _mockContextAccessor;
        private readonly Mock<IStringLocalizer<SharedResources>> _mockLocalizer;

        public YearStructureControllerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockContextAccessor = new Mock<IHttpContextAccessor>();
            _mockLocalizer = new Mock<IStringLocalizer<SharedResources>>();

            _yearStructureController = new YearStructureController(_mockUnitOfWork.Object, _mockContextAccessor.Object, _mockLocalizer.Object);
        }

        [Fact]
        public async Task Index_ReturnsAViewResult_WithAListOfYearStructures()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.YearStructureRepository.GetAllAsync())
                .ReturnsAsync(YearStructureResources.YearStructures);

            //Act
            var result = await _yearStructureController.Index();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<ICollection<YearStructure>>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Index_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.YearStructureRepository.GetAllAsync())
                .ThrowsAsync(new Exception());

            //Act
            var result = await _yearStructureController.Index();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task Create_ReturnsAViewResult_WithAYearStructure()
        {

            //Act
            var result = _yearStructureController.Create();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<YearStructure>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Create_ReturnsTheSameView_WhenThereAreNoPeriods()
        {
            //Arrange
            var formCollectionMock = new Mock<IFormCollection>();
            formCollectionMock.Setup(x => x["periods"]).Returns("[]");

            var httpRequestMock = new Mock<HttpRequest>();
            httpRequestMock.Setup(x => x.Form).Returns(formCollectionMock.Object);


            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(x => x.Request).Returns(httpRequestMock.Object);

            _mockContextAccessor.Setup(x => x.HttpContext).Returns(httpContextMock.Object);

            _mockLocalizer.Setup(x => x[It.IsAny<string>()]).Returns(new LocalizedString("", ""));

            //Act
            var result = await _yearStructureController.Create(YearStructureResources.YearStructure);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<YearStructure>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Create_ReturnsARedirect_AndAddsANewYearStructure()
        {
            //Arrange
            var formCollectionMock = new Mock<IFormCollection>();
            formCollectionMock.Setup(x => x["periods"]).Returns(
                "[{\r\n    \"PeriodId\": 1,\r\n    \"Title\": \"First Period\",\r\n    \"Start\": \"2023-06-01T00:00:00\",\r\n    \"End\": \"2023-06-30T23:59:59\"\r\n  }]");

            var httpRequestMock = new Mock<HttpRequest>();
            httpRequestMock.Setup(x => x.Form).Returns(formCollectionMock.Object);


            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(x => x.Request).Returns(httpRequestMock.Object);

            _mockContextAccessor.Setup(x => x.HttpContext).Returns(httpContextMock.Object);

            _mockUnitOfWork.Setup(x => x.YearStructureRepository.AddAsync(It.IsAny<YearStructure>()))
                .Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(x => x.CompleteAsync()).Returns(Task.CompletedTask);

            //Act
            var result = await _yearStructureController.Create(YearStructureResources.YearStructure);

            //Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Create_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            _mockContextAccessor.Setup(x => x.HttpContext).Throws(new Exception());

            //Act
            var result = await _yearStructureController.Create(YearStructureResources.YearStructure);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task Edit_ReturnsAViewResult_WithAYearStructure()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.YearStructureRepository.GetYearStructureById(It.IsAny<int>()))
                .ReturnsAsync(YearStructureResources.YearStructure);

            //Act
            var result = await _yearStructureController.Edit(It.IsAny<int>());

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<YearStructure>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Edit_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.YearStructureRepository.GetYearStructureById(It.IsAny<int>()))
                .ThrowsAsync(new Exception());

            //Act
            var result = await _yearStructureController.Edit(It.IsAny<int>());

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task Edit_ReturnsTheSameView_WhenThereAreNoPeriods()
        {
            //Arrange
            var formCollectionMock = new Mock<IFormCollection>();
            formCollectionMock.Setup(x => x["periods"]).Returns("[]");

            var httpRequestMock = new Mock<HttpRequest>();
            httpRequestMock.Setup(x => x.Form).Returns(formCollectionMock.Object);


            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(x => x.Request).Returns(httpRequestMock.Object);

            _mockContextAccessor.Setup(x => x.HttpContext).Returns(httpContextMock.Object);

            _mockLocalizer.Setup(x => x[It.IsAny<string>()]).Returns(new LocalizedString("", ""));

            //Act
            var result = await _yearStructureController.Edit(YearStructureResources.YearStructure);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<YearStructure>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Edit_ReturnsARedirect_AndUpdatesTheYearStructure()
        {
            //Arrange
            var formCollectionMock = new Mock<IFormCollection>();
            formCollectionMock.Setup(x => x["periods"]).Returns(
                "[{\r\n    \"PeriodId\": 1,\r\n    \"Title\": \"First Period\",\r\n    \"Start\": \"2023-06-01T00:00:00\",\r\n    \"End\": \"2023-06-30T23:59:59\"\r\n  }]");

            var httpRequestMock = new Mock<HttpRequest>();
            httpRequestMock.Setup(x => x.Form).Returns(formCollectionMock.Object);


            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(x => x.Request).Returns(httpRequestMock.Object);

            _mockContextAccessor.Setup(x => x.HttpContext).Returns(httpContextMock.Object);

            _mockUnitOfWork.Setup(x => x.YearStructureRepository.GetYearStructureById(It.IsAny<int>()))
                .ReturnsAsync(YearStructureResources.YearStructure);
            _mockUnitOfWork.Setup(x => x.YearStructureRepository.UpdateAsync(It.IsAny<YearStructure>()))
                .Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(x => x.CompleteAsync()).Returns(Task.CompletedTask);

            //Act
            var result = await _yearStructureController.Edit(YearStructureResources.YearStructure);

            //Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task EditPost_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            _mockContextAccessor.Setup(x => x.HttpContext).Throws(new Exception());

            //Act
            var result = await _yearStructureController.Edit(YearStructureResources.YearStructure);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task Delete_ReturnsAViewResult_WithAYearStructure()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.YearStructureRepository.GetYearStructureById(It.IsAny<int>()))
                .ReturnsAsync(YearStructureResources.YearStructure);

            //Act
            var result = await _yearStructureController.Delete(It.IsAny<int>());

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<YearStructure>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Delete_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.YearStructureRepository.GetYearStructureById(It.IsAny<int>()))
                .ThrowsAsync(new Exception());

            //Act
            var result = await _yearStructureController.Delete(It.IsAny<int>());

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task DeleteConfirm_ReturnsARedirect_AndDeletesAYearStructure()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.YearStructureRepository.GetYearStructureById(It.IsAny<int>()))
                .ReturnsAsync(YearStructureResources.YearStructure);
            _mockUnitOfWork.Setup(x => x.YearStructureRepository.DeleteAsync(It.IsAny<YearStructure>()))
                .Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(x => x.CompleteAsync()).Returns(Task.CompletedTask);

            //Act
            var result = await _yearStructureController.DeleteConfirm(YearStructureResources.YearStructure);

            //Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task DeleteConfirm_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.YearStructureRepository.GetYearStructureById(It.IsAny<int>()))
                .ThrowsAsync(new Exception());

            //Act
            var result = await _yearStructureController.DeleteConfirm(YearStructureResources.YearStructure);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }
    }
}
