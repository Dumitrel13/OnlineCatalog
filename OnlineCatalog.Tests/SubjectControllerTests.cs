using Microsoft.AspNetCore.Mvc;
using Moq;
using OnlineCatalog.Controllers;
using OnlineCatalog.Models;
using OnlineCatalog.Repository.Interfaces;

namespace OnlineCatalog.Tests
{
    public class SubjectControllerTests
    {
        private readonly Subject _incorrectSubject = new() { Name = "" };
        private readonly Subject _correctSubject = new() { Name = "Matematica" };

        private const int IncorrectId = -1;
        private const int CorrectId = 1;
        

        [Fact]
        public async Task Index_ReturnsAViewResult_WithAListOfSubjects()
        {
            //Arrange
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SubjectRepository.GetAllAsync()).ReturnsAsync(new List<Subject>() {
                new Subject() { SubjectId = 1 },
                new Subject() { SubjectId = 2 }
            });
            var subjectController = new SubjectController(unitOfWork.Object);

            //Act
            var result = await subjectController.Index();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<ICollection<Subject>>(
                viewResult.ViewData.Model);
            Assert.Equal(2, model.Count);
        }

        [Fact]
        public async Task Index_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            var unitOfWork = new Mock<IUnitOfWork>();

            unitOfWork.Setup(x => x.SubjectRepository.GetAllAsync()).ThrowsAsync(new Exception());

            var controller = new SubjectController(unitOfWork.Object);

            //Act
            var result = await controller.Index();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public void Add_ReturnsAView()
        {
            //Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var subjectController = new SubjectController(mockUnitOfWork.Object);

            //Act
            var result = subjectController.Add();

            //Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Add_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();

            mockUnitOfWork.Setup(x => x.SubjectRepository.AddAsync(It.IsAny<Subject>())).ThrowsAsync(new Exception());

            var subjectController = new SubjectController(mockUnitOfWork.Object);

            //Act
            var result = await subjectController.Add(It.IsAny<Subject>());

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task Add_ReturnsViewWithSubject_IfModelStateISNotValid()
        {
            //Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var subjectController = new SubjectController(mockUnitOfWork.Object);
            subjectController.ModelState.AddModelError("Name", "Required");

            //Act
            var result = await subjectController.Add(_incorrectSubject);

            //AssertS
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<Subject>(
                viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Add_ReturnsRedirectToAction_WhenModelStateIsValid()
        {
            //Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(x => x.SubjectRepository.AddAsync(_correctSubject)).Returns(Task.CompletedTask).Verifiable();
            var subjectController = new SubjectController(mockUnitOfWork.Object);

            //Act
            var result = await subjectController.Add(_correctSubject);

            //Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            mockUnitOfWork.Verify();
        }

        [Fact]
        public async Task Edit_ReturnsViewWithSubject_WhenSubjectIsFound()
        {
            //Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(x => x.SubjectRepository.GetSubjectByIdAsync(CorrectId))
                .ReturnsAsync(new Subject() { SubjectId = CorrectId });
            var subjectController = new SubjectController(mockUnitOfWork.Object);

            //Act
            var result = await subjectController.Edit(CorrectId);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<Subject>(
                viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Edit_ReturnsAnErrorView_WhenExceptionIsCaught()
        {
            //Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(x => x.SubjectRepository.GetSubjectByIdAsync(CorrectId))
                .ThrowsAsync(new Exception());
            var subjectController = new SubjectController(mockUnitOfWork.Object);

            //Act
            var result = await subjectController.Edit(CorrectId);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task Edit_ReturnsViewWithSubject_WhenModelStateIsNotValid()
        {
            //Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var subjectController = new SubjectController(mockUnitOfWork.Object);
            subjectController.ModelState.AddModelError("Name", "Required");

            //Act
            var result = await subjectController.Edit(_incorrectSubject);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<Subject>(
                viewResult.ViewData.Model);
        }
        [Fact]
        public async Task Edit_ReturnsRedirectToAction_WhenModelStateIsValid()
        {
            //Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(x => x.SubjectRepository.UpdateAsync(_correctSubject)).Returns(Task.CompletedTask)
                .Verifiable();
            var subjectController = new SubjectController(mockUnitOfWork.Object);

            //Act
            var result = await subjectController.Edit(_correctSubject);

            //Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            mockUnitOfWork.Verify();
        }

        [Fact]
        public async Task EditPost_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(x => x.SubjectRepository.UpdateAsync(_correctSubject)).Throws(new Exception());
            var subjectController = new SubjectController(mockUnitOfWork.Object);

            //Act
            var result = await subjectController.Edit(_correctSubject);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task Delete_ReturnsAnErrorView_WhenIdIsNotFound()
        {
            //Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(x => x.SubjectRepository.GetSubjectByIdAsync(IncorrectId))
                .ThrowsAsync(new InvalidOperationException());
            var subjectController = new SubjectController(mockUnitOfWork.Object);

            //Act
            var result = await subjectController.Delete(IncorrectId);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task Delete_ReturnsViewWithRole_WhenIdIsFound()
        {
            //Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(x => x.SubjectRepository.GetSubjectByIdAsync(CorrectId))
                .ReturnsAsync(new Subject() { SubjectId = CorrectId });
            var subjectController = new SubjectController(mockUnitOfWork.Object);

            //Act
            var result = await subjectController.Delete(CorrectId);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Subject>(
                viewResult.ViewData.Model);
        }

        [Fact]
        public async Task DeleteConfirm_ReturnsAnErrorView_WhenIdIsNotFound()
        {
            //Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(x => x.SubjectRepository.GetSubjectByIdAsync(IncorrectId))
                .ThrowsAsync(new InvalidOperationException());
            var subjectController = new SubjectController(mockUnitOfWork.Object);

            //Act
            var result = await subjectController.DeleteConfirm(IncorrectId);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task DeleteConfirm_ReturnsARedirectActionAndDeletesRole_WhenIdIsFound()
        {
            //Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(x => x.SubjectRepository.GetSubjectByIdAsync(CorrectId)).ReturnsAsync(_correctSubject);
            mockUnitOfWork.Setup(x => x.SubjectRepository.DeleteAsync(_correctSubject))
                .Returns(Task.CompletedTask)
                .Verifiable();
            var subjectController = new SubjectController(mockUnitOfWork.Object);

            //Act
            var result = await subjectController.DeleteConfirm(CorrectId);

            //Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            mockUnitOfWork.Verify();
        }
    }
}
