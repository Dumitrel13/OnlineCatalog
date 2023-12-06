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
    public class ClassroomControllerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly Mock<IHttpContextAccessor> _mockContextAccessor;
        private ClassroomController _classroomController;

        public ClassroomControllerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockUserManager = ClassroomResources.MockUserManager();
            _mockContextAccessor = new Mock<IHttpContextAccessor>();

            _classroomController = new ClassroomController(_mockUnitOfWork.Object, _mockUserManager.Object, _mockContextAccessor.Object);
        }

        [Fact]
        public async Task Index_ReturnsAViewResult_WithACollectionOfClassrooms()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.ClassroomRepository.GetAllAsync()).ReturnsAsync(ClassroomResources.Classrooms);

            //Act
            var result = await _classroomController.Index();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<ICollection<Classroom>>(viewResult.ViewData.Model);

        }

        [Fact]
        public async Task Index_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.ClassroomRepository.GetAllAsync()).ThrowsAsync(new Exception());

            //Act
            var result = await _classroomController.Index();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public void Add_ReturnsAViewResult_WithClassroomViewModel()
        {
            //Arrange

            //Act
            var result = _classroomController.Add();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<ClassroomViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Add_ReturnsAViewResult_WhenModelStateIsNotValid()
        {
            //Arrange
            _classroomController.ModelState.AddModelError("Name", "Error");

            //Act
            var result = await _classroomController.Add(It.IsAny<ClassroomViewModel>());

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Add_ReturnsARedirect_AndAddsANewClassroom()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.SchoolRepository.GetSchoolByName(It.IsAny<string>()))
                .ReturnsAsync(ClassroomResources.School);
            _mockUnitOfWork.Setup(x => x.TeacherRepository.GetTeacherWithSchoolById(It.IsAny<int>()))
                .ReturnsAsync(ClassroomResources.Teacher);
            _mockUnitOfWork.Setup(x => x.ClassroomRepository.AddAsync(It.IsAny<Classroom>()))
                .Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(x => x.CompleteAsync()).Returns(Task.CompletedTask);

            //Act
            var result = await _classroomController.Add(ClassroomResources.ClassroomViewModel);

            //Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Add_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.TeacherRepository.GetTeacherWithSchoolById(It.IsAny<int>()))
                .ThrowsAsync(new Exception());

            //Act
            var result = await _classroomController.Add(ClassroomResources.ClassroomViewModel);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task MyClassrooms_ReturnsAViewResult_WithAListOfClassrooms()
        {
            //Arrange
            _mockUnitOfWork
                .Setup(x =>
                    x.TeacherAssignmentRepository.GetCurrentTeacherAssignmentsForSpecificTeacher(It.IsAny<int>()))
                .ReturnsAsync(ClassroomResources.TeacherAssignments);

            _mockContextAccessor.SetupGet(x => x.HttpContext).Returns(ClassroomResources.MockHttpContext.Object);

            //Act
            var result = await _classroomController.MyClassrooms();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<List<Classroom>>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task MyClassrooms_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            _mockUnitOfWork
                .Setup(x =>
                    x.TeacherAssignmentRepository.GetCurrentTeacherAssignmentsForSpecificTeacher(It.IsAny<int>()))
                .ThrowsAsync(new Exception());

            _mockContextAccessor.SetupGet(x => x.HttpContext).Returns(ClassroomResources.MockHttpContext.Object);

            //Act
            var result = await _classroomController.MyClassrooms();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<ErrorViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task PromoteAllClassroomsDisplay_ReturnsAViewResult()
        {
            //Act
            var result = _classroomController.PromoteAllClassroomsDisplay();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("PromoteAllClassrooms", viewResult.ViewName);
        }

        [Fact]
        public async Task PromoteAllClassrooms_ReturnsARedirect_AndPromotesTheClassrooms()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.PupilRepository.UpdateAsync(It.IsAny<Pupil>())).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(x => x.ClassroomRepository.GetAllClassroomsWithPupilsAsync())
                .ReturnsAsync(ClassroomResources.ClassroomsWithPupils);

            //Act
            var result = await _classroomController.PromoteAllClassrooms();

            //Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Home", redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task PromoteAllClassrooms_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.ClassroomRepository.GetAllClassroomsWithPupilsAsync())
                .ThrowsAsync(new Exception());

            //Act
            var result = await _classroomController.PromoteAllClassrooms();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task PromoteClassroom_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.PupilRepository.UpdateAsync(It.IsAny<Pupil>())).ThrowsAsync(new Exception());
            _mockUnitOfWork.Setup(x => x.ClassroomRepository.GetAllClassroomsWithPupilsAsync())
                .ReturnsAsync(ClassroomResources.ClassroomsWithPupils);

            //Act
            var result = await _classroomController.PromoteAllClassrooms();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task AssignSubjects_ReturnsAViewResult_WithAViewModel()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.ClassroomRepository.GetClassroomByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(ClassroomResources.Classroom);
            _mockUnitOfWork.Setup(x => x.SubjectRepository.GetAllAsync()).ReturnsAsync(ClassroomResources.Subjects);

            //Act
            var result = await _classroomController.AssignSubjects(It.IsAny<int>());

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<AssignSubjectsViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task AssignSubjects_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.ClassroomRepository.GetClassroomByIdAsync(It.IsAny<int>()))
                .ThrowsAsync(new Exception());

            //Act
            var result = await _classroomController.AssignSubjects(It.IsAny<int>());

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task AssignSubjects_ReturnsTheSameView_WhenThereAreNoSubjectIds()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.ClassroomRepository.GetClassroomByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(ClassroomResources.Classroom);
            _mockUnitOfWork.Setup(x => x.SubjectRepository.GetAllAsync()).ReturnsAsync(ClassroomResources.Subjects);

            //Act
            var result = await _classroomController.AssignSubjects(ClassroomResources.AssignSubjectsViewModel);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<AssignSubjectsViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task AssignSubjects_ReturnsAViewResult_WhenThereAreSubjectIds()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.SubjectRepository.GetSubjectsByIds(It.IsAny<List<int>>()))
                .ReturnsAsync(ClassroomResources.Subjects);
            _mockUnitOfWork.Setup(x => x.ClassroomRepository.GetClassroomByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(ClassroomResources.Classroom);
            _mockUnitOfWork.Setup(x => x.SubjectRepository.GetAllAsync()).ReturnsAsync(ClassroomResources.Subjects);
            _mockUnitOfWork.Setup(x => x.ClassroomRepository.UpdateAsync(It.IsAny<Classroom>()))
                .Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(x => x.CompleteAsync()).Returns(Task.CompletedTask);

            //Act
            var result =
                await _classroomController.AssignSubjects(ClassroomResources.AssignSubjectsViewModelWithSubjectIds);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<AssignSubjectsViewModel>(viewResult.Model);
        }

        [Fact]
        public async Task AssignSubjectsPost_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.SubjectRepository.GetSubjectsByIds(It.IsAny<List<int>>()))
                .ThrowsAsync(new Exception());

            //Act
            var result =
                await _classroomController.AssignSubjects(ClassroomResources.AssignSubjectsViewModelWithSubjectIds);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task RemoveSubjectFromList_ReturnsARedirect_AndRemovesASubjectFromTheList()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.ClassroomRepository.GetClassroomByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(ClassroomResources.Classroom);
            _mockUnitOfWork.Setup(x => x.SubjectRepository.GetSubjectByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(ClassroomResources.Subjects.First());
            _mockUnitOfWork.Setup(x => x.ClassroomRepository.UpdateAsync(It.IsAny<Classroom>()))
                .Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(x => x.CompleteAsync()).Returns(Task.CompletedTask);

            //Act
            var result = await _classroomController.RemoveSubjectFromList(It.IsAny<int>(), It.IsAny<int>());

            //Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("AssignSubjects", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task RemoveSubjectFromList_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.ClassroomRepository.GetClassroomByIdAsync(It.IsAny<int>()))
                .ThrowsAsync(new Exception());

            //Act
            var result = await _classroomController.RemoveSubjectFromList(It.IsAny<int>(), It.IsAny<int>());

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task AssignPupils_ReturnsAViewResult_WithAViewModel()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.PupilRepository.GetAllPupilsWithClassroom())
                .ReturnsAsync(ClassroomResources.Pupils);
            _mockUnitOfWork.Setup(x => x.ClassroomRepository.GetClassroomByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(ClassroomResources.Classroom);

            //Act
            var result = await _classroomController.AssignPupils(It.IsAny<int>());

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<AssignPupilsViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task AssignPupils_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.PupilRepository.GetAllPupilsWithClassroom()).ThrowsAsync(new Exception());

            //Act
            var result = await _classroomController.AssignPupils(It.IsAny<int>());

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task AssignPupils_ReturnsTheSameView_WhenThereAreNoPupilIds()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.ClassroomRepository.GetClassroomByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(ClassroomResources.Classroom);
            _mockUnitOfWork.Setup(x => x.PupilRepository.GetAllPupilsWithClassroom())
                .ReturnsAsync(ClassroomResources.Pupils);

            //Act
            var result = await _classroomController.AssignPupils(ClassroomResources.AssignPupilsViewModel);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<AssignPupilsViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task AssignPupils_ReturnsAViewResult_WhenThereArePupilIds()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.PupilRepository.GetPupilsByIds(It.IsAny<List<int>>()))
                .ReturnsAsync(ClassroomResources.Pupils);
            _mockUnitOfWork.Setup(x => x.ClassroomRepository.GetClassroomByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(ClassroomResources.Classroom);
            _mockUnitOfWork.Setup(x => x.PupilRepository.UpdateAsync(It.IsAny<Pupil>())).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(x => x.CompleteAsync()).Returns(Task.CompletedTask);

            //Act
            var result = await _classroomController.AssignPupils(ClassroomResources.AssignPupilsViewModelWithPupilIds);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<AssignPupilsViewModel>(viewResult.Model);
        }

        [Fact]
        public async Task AssignPupilsPost_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.PupilRepository.GetPupilsByIds(It.IsAny<List<int>>()))
                .ThrowsAsync(new Exception());

            //Act
            var result = await _classroomController.AssignPupils(ClassroomResources.AssignPupilsViewModelWithPupilIds);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task RemovePupilFromList_ReturnsARedirect_AndRemovesAPupilFromTheList()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.ClassroomRepository.GetClassroomByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(ClassroomResources.Classroom);
            _mockUnitOfWork.Setup(x => x.PupilRepository.GetPupilWithClassroomById(It.IsAny<int>()))
                .ReturnsAsync(ClassroomResources.Pupils.First());
            _mockUnitOfWork.Setup(x => x.ClassroomRepository.UpdateAsync(It.IsAny<Classroom>()))
                .Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(x => x.CompleteAsync()).Returns(Task.CompletedTask);

            //Act
            var result = await _classroomController.RemovePupilFromList(It.IsAny<int>(), It.IsAny<int>());

            //Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("AssignPupils", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task RemovePupilFromList_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.ClassroomRepository.GetClassroomByIdAsync(It.IsAny<int>()))
                .ThrowsAsync(new Exception());

            //Act
            var result = await _classroomController.RemovePupilFromList(It.IsAny<int>(), It.IsAny<int>());

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }
    }
}
