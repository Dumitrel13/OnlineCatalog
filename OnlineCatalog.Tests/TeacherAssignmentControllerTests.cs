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
    public class TeacherAssignmentControllerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly TeacherAssignmentController _controller;

        public TeacherAssignmentControllerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockUserManager = ClassroomResources.MockUserManager();

            _controller = new TeacherAssignmentController(_mockUnitOfWork.Object, _mockUserManager.Object);
        }

        [Fact]
        public async Task Index_ReturnsAViewResult_WithDisplayAssignmentsViewModel()
        {
            //Arrange
            _mockUnitOfWork
                .Setup(x => x.TeacherAssignmentRepository.GetAllTeacherAssignmentsWithDataByDate(It.IsAny<int>(),
                    It.IsAny<int>())).ReturnsAsync(TeacherAssignmentResources.TeacherAssignments);
            _mockUnitOfWork.Setup(x => x.TeacherRepository.GetTeacherByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(TeacherAssignmentResources.Teacher);

            //Act
            var result = await _controller.Index(It.IsAny<int>(), It.IsAny<int>());

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<DisplayAssignmentsViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Index_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            _mockUnitOfWork
                .Setup(x => x.TeacherAssignmentRepository.GetAllTeacherAssignmentsWithDataByDate(It.IsAny<int>(),
                    It.IsAny<int>())).ThrowsAsync(new Exception());

            //Act
            var result = await _controller.Index(It.IsAny<int>(), It.IsAny<int>());

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task Add_ReturnsAViewResult_WithTeacherAssignmentViewModel()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.TeacherRepository.GetTeacherByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(TeacherAssignmentResources.Teacher);
            _mockUnitOfWork.Setup(x => x.ClassroomRepository.GetAllAsync())
                .ReturnsAsync(TeacherAssignmentResources.Classrooms);
            _mockUnitOfWork.Setup(x => x.RoleRepository.GetAllAsync()).ReturnsAsync(TeacherAssignmentResources.Roles);
            _mockUnitOfWork.Setup(x => x.SubjectRepository.GetAllAsync())
                .ReturnsAsync(TeacherAssignmentResources.Subjects);
            _mockUnitOfWork.Setup(x => x.YearStructureRepository.GetCurrentYearStructure())
                .ReturnsAsync(TeacherAssignmentResources.YearStructure);

            //Act
            var result = await _controller.Add(It.IsAny<int>());

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<TeacherAssignmentViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Add_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.TeacherRepository.GetTeacherByIdAsync(It.IsAny<int>()))
                .ThrowsAsync(new Exception());

            //Act
            var result = await _controller.Add(It.IsAny<int>());

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task Add_ReturnsTheSameView_WhenTheInputIsNotValid()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.TeacherRepository.GetTeacherByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(TeacherAssignmentResources.Teacher);
            _mockUnitOfWork.Setup(x => x.ClassroomRepository.GetAllAsync())
                .ReturnsAsync(TeacherAssignmentResources.Classrooms);
            _mockUnitOfWork.Setup(x => x.RoleRepository.GetAllAsync()).ReturnsAsync(TeacherAssignmentResources.Roles);
            _mockUnitOfWork.Setup(x => x.SubjectRepository.GetAllAsync())
                .ReturnsAsync(TeacherAssignmentResources.Subjects);
            _mockUnitOfWork.Setup(x => x.YearStructureRepository.GetCurrentYearStructure())
                .ReturnsAsync(TeacherAssignmentResources.YearStructure);

            //Act
            var result = await _controller.Add(TeacherAssignmentResources.TeacherAssignmentViewModelEmpty);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<TeacherAssignmentViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Add_ReturnsARedirect_AndAddsANewTeacherAssignment()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.TeacherRepository.GetTeacherByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(TeacherAssignmentResources.Teacher);
            _mockUnitOfWork.Setup(x => x.ClassroomRepository.GetClassroomByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(TeacherAssignmentResources.Classrooms.First);
            _mockUnitOfWork.Setup(x => x.RoleRepository.GetRoleByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(TeacherAssignmentResources.Roles.First);
            _mockUnitOfWork.Setup(x => x.SubjectRepository.GetSubjectByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(TeacherAssignmentResources.Subjects.First);
            _mockUnitOfWork.Setup(x => x.TeacherAssignmentRepository.AddAsync(It.IsAny<TeacherAssignment>()))
                .Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(x => x.CompleteAsync()).Returns(Task.CompletedTask);

            //Act
            var result = await _controller.Add(TeacherAssignmentResources.TeacherAssignmentViewModel);

            //Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task AddPost_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.TeacherRepository.GetTeacherByIdAsync(It.IsAny<int>()))
                .ThrowsAsync(new Exception());

            //Act
            var result = await _controller.Add(TeacherAssignmentResources.TeacherAssignmentViewModel);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }


        [Fact]
        public async Task Edit_ReturnsAViewResult_WithTeacherAssignmentViewModel()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.TeacherAssignmentRepository.GetTeacherAssignmentById(It.IsAny<int>()))
                .ReturnsAsync(TeacherAssignmentResources.TeacherAssignment);
            _mockUnitOfWork.Setup(x => x.ClassroomRepository.GetClassroomByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(TeacherAssignmentResources.ClassroomWithSubjects);
            _mockUnitOfWork.Setup(x => x.RoleRepository.GetAllAsync()).ReturnsAsync(TeacherAssignmentResources.Roles);
            _mockUnitOfWork.Setup(x => x.YearStructureRepository.GetCurrentYearStructure())
                .ReturnsAsync(TeacherAssignmentResources.YearStructure);

            //Act
            var result = await _controller.Edit(It.IsAny<int>());

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<TeacherAssignmentViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Edit_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.TeacherAssignmentRepository.GetTeacherAssignmentById(It.IsAny<int>()))
                .ThrowsAsync(new Exception());

            //Act
            var result = await _controller.Edit(It.IsAny<int>());

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task Edit_ReturnsTheSameView_WhenTheInputIsInvalid()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.TeacherRepository.GetTeacherByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(TeacherAssignmentResources.Teacher);
            _mockUnitOfWork.Setup(x => x.ClassroomRepository.GetClassroomByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(TeacherAssignmentResources.ClassroomWithSubjects);
            _mockUnitOfWork.Setup(x => x.RoleRepository.GetRoleByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(TeacherAssignmentResources.Roles.First());
            _mockUnitOfWork.Setup(x => x.SubjectRepository.GetSubjectByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(TeacherAssignmentResources.Subjects.First());
            _mockUnitOfWork.Setup(x => x.TeacherAssignmentRepository.GetTeacherAssignmentById(It.IsAny<int>()))
                .ReturnsAsync(TeacherAssignmentResources.TeacherAssignment);
            _mockUnitOfWork.Setup(x => x.RoleRepository.GetAllAsync()).ReturnsAsync(TeacherAssignmentResources.Roles);
            _mockUnitOfWork.Setup(x => x.YearStructureRepository.GetCurrentYearStructure())
                .ReturnsAsync(TeacherAssignmentResources.YearStructure);

            //Act
            var result = await _controller.Edit(TeacherAssignmentResources.TeacherAssignmentViewModelEmpty);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<TeacherAssignmentViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Edit_ReturnsARedirect_AndUpdatesTheTeacherAssignment()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.TeacherRepository.GetTeacherByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(TeacherAssignmentResources.Teacher);
            _mockUnitOfWork.Setup(x => x.ClassroomRepository.GetClassroomByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(TeacherAssignmentResources.ClassroomWithSubjects);
            _mockUnitOfWork.Setup(x => x.RoleRepository.GetRoleByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(TeacherAssignmentResources.Roles.First());
            _mockUnitOfWork.Setup(x => x.SubjectRepository.GetSubjectByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(TeacherAssignmentResources.Subjects.First());
            _mockUnitOfWork.Setup(x => x.TeacherAssignmentRepository.UpdateAsync(It.IsAny<TeacherAssignment>()))
                .Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(x => x.CompleteAsync()).Returns(Task.CompletedTask);

            _mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(TeacherAssignmentResources.Teacher);

            //Act
            var result = await _controller.Edit(TeacherAssignmentResources.TeacherAssignmentViewModel);

            //Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task EditPost_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.TeacherRepository.GetTeacherByIdAsync(It.IsAny<int>()))
                .ThrowsAsync(new Exception());

            //Act
            var result = await _controller.Edit(TeacherAssignmentResources.TeacherAssignmentViewModel);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }
    }
}
