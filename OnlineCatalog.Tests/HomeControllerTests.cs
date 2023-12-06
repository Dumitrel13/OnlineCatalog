using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using OnlineCatalog.Controllers;

namespace OnlineCatalog.Tests
{
    public class HomeControllerTests
    {
        private readonly Mock<ILogger<HomeController>> _mockLogger;
        private readonly HomeController _homeController;

        public HomeControllerTests()
        {
            _mockLogger = new Mock<ILogger<HomeController>>();
            _homeController = new HomeController(_mockLogger.Object);
        }

        [Fact]
        public void Index_ReturnsAViewResult()
        {
            //Act
            var result = _homeController.Index();

            //Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Privacy_ReturnsAViewResult()
        {
            //Act
            var result = _homeController.Privacy();

            //Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Error_ReturnsAViewResult()
        {
            //Act
            var result = _homeController.Error();

            //Assert
            Assert.IsType<ViewResult>(result);
        }
    }
}
