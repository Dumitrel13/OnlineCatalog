using Microsoft.AspNetCore.Identity;
using Moq;
using OnlineCatalog.Controllers;
using OnlineCatalog.Models;
using OnlineCatalog.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using OnlineCatalog.Tests.Resources;
using Microsoft.AspNetCore.Mvc;
using OnlineCatalog.ViewModels;

namespace OnlineCatalog.Tests
{
    public class AbsenceControllerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly Mock<IHttpContextAccessor> _mockContextAccessor;
        private AbsenceController _absenceController;

        public AbsenceControllerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockUserManager = AbsenceResources.MockUserManager();
            _mockContextAccessor = new Mock<IHttpContextAccessor>();

            _absenceController = new AbsenceController(_mockUnitOfWork.Object, _mockUserManager.Object, _mockContextAccessor.Object);
        }

        [Fact]
        public async Task Index_ReturnsAViewResult_WithAbsenceViewModel()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.PupilRepository.GetPupilWithClassroomById(It.IsAny<int>())).ReturnsAsync(AbsenceResources.Pupil);
            _mockUnitOfWork.Setup(x =>
                    x.TeacherAssignmentRepository.GetCurrentTeacherAssignmentsWithDataForSpecificTeacherAndClass(
                        It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(AbsenceResources.TeacherAssignments);
            _mockUnitOfWork.Setup(x => x.ClassroomRepository.GetClassroomByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(AbsenceResources.Classroom);
            _mockUnitOfWork
                .Setup(x => x.AbsenceRepository.GetAbsencesByPupilIdAndSubjectId(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(AbsenceResources.Absences);

            _mockContextAccessor.SetupGet(x => x.HttpContext).Returns(AbsenceResources.MockHttpContext.Object);

            //Act
            var result = await _absenceController.Index(It.IsAny<int>());

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<AbsenceViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Index_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.PupilRepository.GetPupilWithClassroomById(It.IsAny<int>())).ReturnsAsync(AbsenceResources.Pupil);
            _mockUnitOfWork.Setup(x =>
                x.TeacherAssignmentRepository.GetCurrentTeacherAssignmentsWithDataForSpecificTeacherAndClass(
                    It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new List<TeacherAssignment>());

            _mockContextAccessor.SetupGet(x => x.HttpContext).Returns(AbsenceResources.MockHttpContext.Object);

            //Act
            var result = await _absenceController.Index(It.IsAny<int>());

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<ErrorViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Index_ReturnsAViewResult_WithDifferentSubjectsInViewModel()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.PupilRepository.GetPupilWithClassroomById(It.IsAny<int>())).ReturnsAsync(AbsenceResources.Pupil);
            _mockUnitOfWork.Setup(x =>
                x.TeacherAssignmentRepository.GetCurrentTeacherAssignmentsWithDataForSpecificTeacherAndClass(
                    It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(AbsenceResources.DifferentTeacherAssignments);
            _mockUnitOfWork.Setup(x => x.ClassroomRepository.GetClassroomByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(AbsenceResources.Classroom);
            _mockUnitOfWork
                .Setup(x => x.AbsenceRepository.GetAbsencesByPupilIdAndSubjectId(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(AbsenceResources.Absences);

            _mockContextAccessor.SetupGet(x => x.HttpContext).Returns(AbsenceResources.MockHttpContext.Object);

            //Act
            var result = await _absenceController.Index(It.IsAny<int>());

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<AbsenceViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task DisplayAbsencesForSpecificPupil_ReturnsAViewResult_WithAbsenceViewModel()
        {
            //Arrange
            _mockUnitOfWork
                .Setup(x => x.PupilRepository.GetPupilWithClassroomByIdForParent(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(AbsenceResources.Pupil);
            _mockUnitOfWork.Setup(x => x.PupilRepository.GetPupilWithClassroomById(It.IsAny<int>()))
                .ReturnsAsync(AbsenceResources.Pupil);
            _mockUnitOfWork.Setup(x => x.YearStructureRepository.GetCurrentYearStructure())
                .ReturnsAsync(AbsenceResources.YearStructure);
            _mockUnitOfWork
                .Setup(x => x.AbsenceRepository.GetAbsencesForTheCurrentYearByPupilId(It.IsAny<int>(),
                    It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(AbsenceResources.Absences);
            _mockUnitOfWork.Setup(x => x.ClassroomRepository.GetClassroomByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(AbsenceResources.Classroom);

            _mockContextAccessor.SetupGet(x => x.HttpContext).Returns(AbsenceResources.MockHttpContext.Object);

            //Act
            var result = await _absenceController.DisplayAbsencesForSpecificPupil(1);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<AbsenceViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task DisplayAbsencesForSpecificPupil_ReturnsAnErrorView_WhenTheUserIdAndTheParameterIdDoesNotMatch()
        {
            //Arrange
            _mockUnitOfWork
                .Setup(x => x.PupilRepository.GetPupilWithClassroomByIdForParent(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(AbsenceResources.Pupil);
            _mockUnitOfWork.Setup(x => x.PupilRepository.GetPupilWithClassroomById(It.IsAny<int>()))
                .ReturnsAsync(AbsenceResources.Pupil);
            _mockUnitOfWork.Setup(x => x.YearStructureRepository.GetCurrentYearStructure())
                .ReturnsAsync(AbsenceResources.YearStructure);
            _mockUnitOfWork
                .Setup(x => x.AbsenceRepository.GetAbsencesForTheCurrentYearByPupilId(It.IsAny<int>(),
                    It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(AbsenceResources.Absences);
            _mockUnitOfWork.Setup(x => x.ClassroomRepository.GetClassroomByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(AbsenceResources.Classroom);

            _mockContextAccessor.SetupGet(x => x.HttpContext).Returns(AbsenceResources.MockHttpContext.Object);

            //Act
            var result = await _absenceController.DisplayAbsencesForSpecificPupil(It.IsAny<int>());

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<ErrorViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task DisplayAbsencesForSpecificPupil_ReturnsAnErrorView_WhenThePupilIsNull()
        {
            //Arrange
            _mockUnitOfWork
                .Setup(x => x.PupilRepository.GetPupilWithClassroomByIdForParent(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(null as Pupil);
            _mockUnitOfWork.Setup(x => x.PupilRepository.GetPupilWithClassroomById(It.IsAny<int>()))
                .ReturnsAsync(null as Pupil);

            _mockContextAccessor.SetupGet(x => x.HttpContext).Returns(AbsenceResources.MockHttpContext.Object);

            //Act
            var result = await _absenceController.DisplayAbsencesForSpecificPupil(1);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<ErrorViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Add_ReturnsAViewResult_WithAddAbsenceViewModel()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.PupilRepository.GetPupilWithClassroomById(It.IsAny<int>()))
                .ReturnsAsync(AbsenceResources.Pupil);
            _mockUnitOfWork.Setup(x => x.SubjectRepository.GetSubjectByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(AbsenceResources.Subject);

            _mockContextAccessor.SetupGet(x => x.HttpContext).Returns(AbsenceResources.MockHttpContext.Object);

            //Act
            var result = await _absenceController.Add(It.IsAny<int>(), It.IsAny<int>());

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<AddAbsenceViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Add_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.PupilRepository.GetPupilWithClassroomById(It.IsAny<int>()))
                .ThrowsAsync(new Exception());

            //Act
            var result = await _absenceController.Add(It.IsAny<int>(), It.IsAny<int>());

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task Add_ReturnsARedirect_AndAddsAnAbsence()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.TeacherRepository.GetTeacherByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(AbsenceResources.Teacher);
            _mockUnitOfWork.Setup(x => x.PupilRepository.GetPupilWithClassroomById(It.IsAny<int>()))
                .ReturnsAsync(AbsenceResources.Pupil);
            _mockUnitOfWork.Setup(x => x.SubjectRepository.GetSubjectByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(AbsenceResources.Subject);
            _mockUnitOfWork
                .Setup(x => x.AbsenceRepository.GetAbsencesByPupilIdAndSubjectId(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(AbsenceResources.Absences);
            _mockUnitOfWork.Setup(x => x.AbsenceRepository.AddAsync(It.IsAny<Absence>())).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(x => x.CompleteAsync()).Returns(Task.CompletedTask);

            _mockContextAccessor.SetupGet(x => x.HttpContext).Returns(AbsenceResources.MockHttpContext.Object);

            //Act
            var result = await _absenceController.Add(AbsenceResources.AddAbsenceViewModel);

            //Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Add_ReturnsViewResult_WithAbsenceViewModel_WhenTheInputIsNotCorrect()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.TeacherRepository.GetTeacherByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(AbsenceResources.Teacher);
            _mockUnitOfWork.Setup(x => x.PupilRepository.GetPupilWithClassroomById(It.IsAny<int>()))
                .ReturnsAsync(AbsenceResources.Pupil);
            _mockUnitOfWork.Setup(x => x.SubjectRepository.GetSubjectByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(AbsenceResources.Subject);
            _mockUnitOfWork
                .Setup(x => x.AbsenceRepository.GetAbsencesByPupilIdAndSubjectId(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(AbsenceResources.DuplicateAbsences);
            _mockUnitOfWork.Setup(x => x.AbsenceRepository.AddAsync(It.IsAny<Absence>())).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(x => x.CompleteAsync()).Returns(Task.CompletedTask);

            _mockContextAccessor.SetupGet(x => x.HttpContext).Returns(AbsenceResources.MockHttpContext.Object);

            //Act
            var result = await _absenceController.Add(AbsenceResources.AddAbsenceViewModel);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<AddAbsenceViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task AddPost_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.TeacherRepository.GetTeacherByIdAsync(It.IsAny<int>()))
                .ThrowsAsync(new Exception());

            //Act
            var result = await _absenceController.Add(AbsenceResources.AddAbsenceViewModel);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task Details_ReturnsAViewResult_WithAbsenceViewModel()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.YearStructureRepository.GetCurrentYearStructure())
                .ReturnsAsync(AbsenceResources.YearStructure);
            _mockUnitOfWork
                .Setup(x => x.AbsenceRepository.GetAbsencesByPupilIdAndSubjectId(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(AbsenceResources.Absences);
            _mockUnitOfWork.Setup(x => x.PupilRepository.GetPupilWithClassroomById(It.IsAny<int>()))
                .ReturnsAsync(AbsenceResources.Pupil);

            _mockContextAccessor.SetupGet(x => x.HttpContext).Returns(AbsenceResources.MockHttpContext.Object);

            //Act
            var result = await _absenceController.Details(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>());

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<AbsenceViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Details_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.YearStructureRepository.GetCurrentYearStructure())
                .ThrowsAsync(new Exception());

            //Act
            var result = await _absenceController.Details(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>());

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task Justify_ReturnsARedirect_AndUpdatesAnAbsence()
        {
            //Arrange

            _mockUnitOfWork.Setup(x => x.AbsenceRepository.GetAbsenceById(It.IsAny<int>()))
                .ReturnsAsync(AbsenceResources.DuplicateAbsences.First());
            _mockUnitOfWork.Setup(x => x.AbsenceRepository.UpdateAsync(It.IsAny<Absence>()))
                .Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(x => x.CompleteAsync()).Returns(Task.CompletedTask);

            //Act
            var result = await _absenceController.Justify(It.IsAny<int>(), It.IsAny<int>());

            //Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Justify_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange

            _mockUnitOfWork.Setup(x => x.AbsenceRepository.GetAbsenceById(It.IsAny<int>()))
                .ThrowsAsync(new Exception());

            //Act
            var result = await _absenceController.Justify(It.IsAny<int>(), It.IsAny<int>());

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }
    }
}
