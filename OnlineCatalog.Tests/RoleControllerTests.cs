using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using OnlineCatalog.Controllers;
using OnlineCatalog.Data;
using OnlineCatalog.Models;
using OnlineCatalog.Repository.Interfaces;

namespace OnlineCatalog.Tests
{
    public class RoleControllerTests
    {
        private readonly RoleController _roleController;
        public RoleControllerTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                    .UseInMemoryDatabase(databaseName: "OnlineCatalog")
                    .Options;
            var context = new AppDbContext(options);
            //IUnitOfWork unitOfWork = new UnitOfWork(context);
            //_roleController = new RoleController(unitOfWork);
        }

        [Fact]//integration test
        public async Task Index_ReturnsAViewResult_WithAListOfRoles()
        {
            //Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(x => x.RoleRepository.GetAllAsync()).ReturnsAsync(new List<ApplicationRole>()
            {
                new ApplicationRole()
                {
                    Id = 1,
                    Name = "Profesor"
                },
                new ApplicationRole()
                {
                    Id = 2,
                    Name = "Diriginte"
                }
            });
            var roleController = new RoleController(mockUnitOfWork.Object);

            //Act
            var result = await roleController.Index();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<ICollection<ApplicationRole>>(
                viewResult.ViewData.Model);
            Assert.Equal(2, model.Count);
        }

        [Fact]
        public async Task Index_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(x => x.RoleRepository.GetAllAsync()).ThrowsAsync(new Exception());

            var roleController = new RoleController(mockUnitOfWork.Object);

            //Act
            var result = await roleController.Index();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task AddAndEdit_ReturnsView_WhenIdIs0()
        {
            //Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var roleController = new RoleController(mockUnitOfWork.Object);
            const int id = 0;

            //Act
            var result = await roleController.AddAndEdit(id);

            //Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task AddAndEdit_ReturnsViewWithRole_WhenIdIsNot0()
        {
            //Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            const int id = 1;
            mockUnitOfWork.Setup(x => x.RoleRepository.GetRoleByIdAsync(id)).ReturnsAsync(new ApplicationRole() { Id = id });
            var roleController = new RoleController(mockUnitOfWork.Object);


            //Act
            var result = await roleController.AddAndEdit(id);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<ApplicationRole>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task AddAndEdit_ReturnsARedirectAndAddsRole_WhenIdIs0()
        {
            //Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(x => x.RoleRepository.AddAsync(It.IsAny<ApplicationRole>()))
                .Returns(Task.CompletedTask)
                .Verifiable();
            var roleController = new RoleController(mockUnitOfWork.Object);
            ApplicationRole role = new() { Id = 0 };

            //Act
            var result = await roleController.AddAndEdit(role);

            //Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            mockUnitOfWork.Verify();
        }

        [Fact]
        public async Task AddAndEdit_ReturnsARedirectActionAndUpdatesRole_WhenIdIsNot0()
        {
            //Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            ApplicationRole role = new() { Id = 1, Name = "Test"};

            mockUnitOfWork.Setup(x => x.RoleRepository.GetRoleByIdAsync(It.IsAny<int>())).ReturnsAsync(role);
            mockUnitOfWork.Setup(x => x.RoleRepository.UpdateAsync(role))
                .Returns(Task.CompletedTask)
                .Verifiable();
            var roleController = new RoleController(mockUnitOfWork.Object);

            //Act
            var result = await roleController.AddAndEdit(role);

            //Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            mockUnitOfWork.Verify();
        }

        [Fact]
        public async Task AddAndEdit_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();

            mockUnitOfWork.Setup(x => x.RoleRepository.GetRoleByIdAsync(It.IsAny<int>())).ThrowsAsync(new Exception());

            var roleController = new RoleController(mockUnitOfWork.Object);

            //Act
            var result = await roleController.AddAndEdit(1);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);

        }

        [Fact]
        public async Task AddAndEditPost_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();

            mockUnitOfWork.Setup(x => x.RoleRepository.GetRoleByIdAsync(It.IsAny<int>())).ThrowsAsync(new Exception());

            var roleController = new RoleController(mockUnitOfWork.Object);

            //Act
            var result = await roleController.AddAndEdit(It.IsAny<ApplicationRole>());

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task Delete_ReturnsAnErrorView_WhenIdIsNotFound()
        {
            //Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            const int id = -1;
            mockUnitOfWork.Setup(x => x.RoleRepository.GetRoleByIdAsync(id)).ThrowsAsync(new InvalidOperationException());
            var roleController = new RoleController(mockUnitOfWork.Object);

            //Act
            var result = await roleController.Delete(id);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task Delete_ReturnsViewWithRole_WhenIdIsFound()
        {
            //Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            const int id = 2;
            mockUnitOfWork.Setup(x => x.RoleRepository.GetRoleByIdAsync(id)).ReturnsAsync(new ApplicationRole() { Id = id });
            var roleController = new RoleController(mockUnitOfWork.Object);

            //Act
            var result = await roleController.Delete(id);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<ApplicationRole>(
                viewResult.ViewData.Model);
        }

        [Fact]
        public async Task DeleteConfirm_ReturnsAnErrorView_WhenIdIsNotFound()
        {
            //Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            const int id = -1;
            mockUnitOfWork.Setup(x => x.RoleRepository.GetRoleByIdAsync(id)).ThrowsAsync(new InvalidOperationException());
            var roleController = new RoleController(mockUnitOfWork.Object);

            //Act
            var result = await roleController.DeleteConfirm(id);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task DeleteConfirm_ReturnsARedirectActionAndDeletesRole_WhenIdIsFound()
        {
            //Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            ApplicationRole role = new() { Id = 1 };
            const int id = 1;
            mockUnitOfWork.Setup(x => x.RoleRepository.GetRoleByIdAsync(id)).ReturnsAsync(role);
            mockUnitOfWork.Setup(x => x.RoleRepository.DeleteAsync(role))
                .Returns(Task.CompletedTask)
                .Verifiable();
            var roleController = new RoleController(mockUnitOfWork.Object);

            //Act
            var result = await roleController.DeleteConfirm(id);

            //Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            mockUnitOfWork.Verify();
        }
    }
}
