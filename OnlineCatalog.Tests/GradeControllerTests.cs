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
    public class GradeControllerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly Mock<IHttpContextAccessor> _mockContextAccessor;
        private readonly Mock<IStringLocalizer<SharedResources>> _mockStringLocalizer;
        private GradeController _gradeController;

        public GradeControllerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockUserManager = GradeResources.MockUserManager();
            _mockContextAccessor = new Mock<IHttpContextAccessor>();
            _mockStringLocalizer = new Mock<IStringLocalizer<SharedResources>>();

            _gradeController = new GradeController(_mockUnitOfWork.Object, _mockUserManager.Object, _mockContextAccessor.Object, _mockStringLocalizer.Object);
        }

        [Fact]
        public async Task Index_ReturnsAnErrorView_IfThePupilHasNoClassroom()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.PupilRepository.GetPupilWithClassroomById(It.IsAny<int>()))
                .ReturnsAsync(GradeResources.PupilWithoutClass);
            _mockUnitOfWork
                .Setup(x =>
                    x.TeacherAssignmentRepository.GetCurrentTeacherAssignmentsForSpecificTeacher(It.IsAny<int>()))
                .ReturnsAsync(GradeResources.TeacherAssignments);

            _mockContextAccessor.SetupGet(x => x.HttpContext).Returns(GradeResources.MockHttpContext.Object);

            //Act
            var result = await _gradeController.Index(It.IsAny<int>());

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<ErrorViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Index_ReturnsAViewResult_WhenThePupilHasAClassroom()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.PupilRepository.GetPupilWithClassroomById(It.IsAny<int>()))
                .ReturnsAsync(GradeResources.PupilWithClass);
            _mockUnitOfWork
                .Setup(x =>
                    x.TeacherAssignmentRepository.GetCurrentTeacherAssignmentsForSpecificTeacher(It.IsAny<int>()))
                .ReturnsAsync(GradeResources.TeacherAssignments);
            _mockUnitOfWork.Setup(x => x.YearStructureRepository.GetCurrentYearStructure())
                .ReturnsAsync(GradeResources.YearStructure);
            _mockUnitOfWork
                .Setup(x => x.GradeRepository.GetPupilGradesForThisYear(It.IsAny<int>(), It.IsAny<DateTime>(),
                    It.IsAny<DateTime>())).ReturnsAsync(GradeResources.Grades);
            _mockUnitOfWork
                .Setup(x =>
                    x.TeacherAssignmentRepository.GetCurrentTeacherAssignmentsWithDataForSpecificTeacherAndClass(
                        It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(GradeResources.TeacherAssignments);

            _mockContextAccessor.SetupGet(x => x.HttpContext).Returns(GradeResources.MockHttpContext.Object);

            //Act
            var result = await _gradeController.Index(It.IsAny<int>());

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<GradeViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task DisplayGradesForSpecificPupil_ReturnsAnErrorViewIfTheUserIdISDifferentFromTheParameterId()
        {
            //Arrange
            _mockUnitOfWork
                .Setup(x => x.PupilRepository.GetPupilWithClassroomByIdForParent(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(GradeResources.PupilWithClass);
            _mockUnitOfWork.Setup(x => x.PupilRepository.GetPupilWithClassroomById(It.IsAny<int>()))
                .ReturnsAsync(GradeResources.PupilWithClass);

            _mockContextAccessor.SetupGet(x => x.HttpContext).Returns(GradeResources.MockHttpContext.Object);

            //Act
            var result = await _gradeController.DisplayGradesForSpecificPupil(It.IsAny<int>());

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<ErrorViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task DisplayGradesForSpecificPupil_ReturnsAViewResult_WithGradeViewModel()
        {
            //Arrange
            _mockUnitOfWork
                .Setup(x => x.PupilRepository.GetPupilWithClassroomByIdForParent(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(GradeResources.PupilWithClass);
            _mockUnitOfWork.Setup(x => x.PupilRepository.GetPupilWithClassroomById(It.IsAny<int>()))
                .ReturnsAsync(GradeResources.PupilWithClass);
            _mockUnitOfWork.Setup(x => x.YearStructureRepository.GetCurrentYearStructure())
                .ReturnsAsync(GradeResources.YearStructure);
            _mockUnitOfWork
                .Setup(x => x.GradeRepository.GetPupilGradesForThisYear(It.IsAny<int>(), It.IsAny<DateTime>(),
                    It.IsAny<DateTime>())).ReturnsAsync(GradeResources.Grades);
            _mockUnitOfWork.Setup(x => x.SubjectRepository.GetAllAsync()).ReturnsAsync(GradeResources.Subjects);
            _mockUnitOfWork.Setup(x => x.ClassroomRepository.GetClassroomByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(GradeResources.Classroom);

            _mockContextAccessor.SetupGet(x => x.HttpContext).Returns(GradeResources.MockHttpContext.Object);

            //Act
            var result = await _gradeController.DisplayGradesForSpecificPupil(1);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<GradeViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task DisplayGradesForSpecificPupil_ReturnsAnErrorView_WhenThePupilDoesNotExist()
        {
            //Arrange
            _mockUnitOfWork
                .Setup(x => x.PupilRepository.GetPupilWithClassroomByIdForParent(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(null as Pupil);
            _mockUnitOfWork.Setup(x => x.PupilRepository.GetPupilWithClassroomById(It.IsAny<int>()))
                .ReturnsAsync(null as Pupil);

            _mockContextAccessor.SetupGet(x => x.HttpContext).Returns(GradeResources.MockHttpContext.Object);

            //Act
            var result = await _gradeController.DisplayGradesForSpecificPupil(1);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<ErrorViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task AddGrade_ReturnsAViewResult_WithAddGradeViewModel()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.PupilRepository.GetPupilWithClassroomById(It.IsAny<int>()))
                .ReturnsAsync(GradeResources.PupilWithClass);
            _mockUnitOfWork.Setup(x => x.SubjectRepository.GetSubjectByName(It.IsAny<string>()))
                .ReturnsAsync(GradeResources.Subjects.First());
            _mockUnitOfWork.Setup(x => x.GradeTypeRepository.GetAllAsync()).ReturnsAsync(GradeResources.GradeTypes);
            _mockUnitOfWork.Setup(x => x.YearStructureRepository.GetCurrentYearStructure())
                .ReturnsAsync(GradeResources.YearStructure);

            _mockContextAccessor.SetupGet(x => x.HttpContext).Returns(GradeResources.MockHttpContext.Object);

            //Act
            var result = await _gradeController.AddGrade(It.IsAny<string>(), It.IsAny<int>());

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<AddGradeViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task AddGrade_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.PupilRepository.GetPupilWithClassroomById(It.IsAny<int>()))
                .ThrowsAsync(new Exception());

            //Act
            var result = await _gradeController.AddGrade(It.IsAny<string>(), It.IsAny<int>());

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);

        }

        [Fact]
        public async Task AddGrade_ReturnsTheSameView_IfARatingExistsAndANewRatingIsAdded()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.PupilRepository.GetPupilWithClassroomById(It.IsAny<int>()))
                .ReturnsAsync(GradeResources.PupilWithClass);
            _mockUnitOfWork.Setup(x => x.TeacherRepository.GetTeacherByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(GradeResources.Teacher);
            _mockUnitOfWork.Setup(x => x.SubjectRepository.GetSubjectByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(GradeResources.Subjects.First());
            _mockUnitOfWork.Setup(x => x.GradeTypeRepository.GetGradeTypeById(It.IsAny<int>()))
                .ReturnsAsync(GradeResources.GradeTypes.First());
            _mockUnitOfWork.Setup(x => x.GradeTypeRepository.GetAllAsync()).ReturnsAsync(GradeResources.GradeTypes);
            _mockUnitOfWork.Setup(x => x.YearStructureRepository.GetCurrentYearStructure())
                .ReturnsAsync(GradeResources.YearStructure);
            _mockUnitOfWork
                .Setup(x => x.GradeRepository.GetGradesForSpecificSubject(It.IsAny<int>(), It.IsAny<int>(),
                    It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(GradeResources.RatingGrade);
            _mockUnitOfWork.Setup(x => x.GradeRepository.AddAsync(It.IsAny<Grade>())).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(x => x.CompleteAsync()).Returns(Task.CompletedTask);

            _mockStringLocalizer.Setup(x => x[It.IsAny<string>()])
                .Returns(new LocalizedString("", ""));

            _mockContextAccessor.SetupGet(x => x.HttpContext).Returns(GradeResources.MockHttpContext.Object);

            //Act
            var result = await _gradeController.AddGrade(GradeResources.AddGradeViewModelWithRating);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<AddGradeViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task AddGrade_ReturnsTheSameView_WhenTryingToAddARatingOverGradeAndViceVersa()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.PupilRepository.GetPupilWithClassroomById(It.IsAny<int>()))
                .ReturnsAsync(GradeResources.PupilWithClass);
            _mockUnitOfWork.Setup(x => x.TeacherRepository.GetTeacherByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(GradeResources.Teacher);
            _mockUnitOfWork.Setup(x => x.SubjectRepository.GetSubjectByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(GradeResources.Subjects.First());
            _mockUnitOfWork.Setup(x => x.GradeTypeRepository.GetGradeTypeById(It.IsAny<int>()))
                .ReturnsAsync(GradeResources.GradeTypes.First());
            _mockUnitOfWork.Setup(x => x.GradeTypeRepository.GetAllAsync()).ReturnsAsync(GradeResources.GradeTypes);
            _mockUnitOfWork.Setup(x => x.YearStructureRepository.GetCurrentYearStructure())
                .ReturnsAsync(GradeResources.YearStructure);
            _mockUnitOfWork
                .Setup(x => x.GradeRepository.GetGradesForSpecificSubject(It.IsAny<int>(), It.IsAny<int>(),
                    It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(GradeResources.RatingGrade);
            _mockUnitOfWork.Setup(x => x.GradeRepository.AddAsync(It.IsAny<Grade>())).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(x => x.CompleteAsync()).Returns(Task.CompletedTask);

            _mockStringLocalizer.Setup(x => x[It.IsAny<string>()])
                .Returns(new LocalizedString("", ""));

            _mockContextAccessor.SetupGet(x => x.HttpContext).Returns(GradeResources.MockHttpContext.Object);

            //Act
            var result = await _gradeController.AddGrade(GradeResources.AddGradeViewModel);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<AddGradeViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task AddGrade_ReturnsTheSameView_WhenTryingToAddAnotherExamGrade()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.PupilRepository.GetPupilWithClassroomById(It.IsAny<int>()))
                .ReturnsAsync(GradeResources.PupilWithClass);
            _mockUnitOfWork.Setup(x => x.TeacherRepository.GetTeacherByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(GradeResources.Teacher);
            _mockUnitOfWork.Setup(x => x.SubjectRepository.GetSubjectByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(GradeResources.Subjects.First());
            _mockUnitOfWork.Setup(x => x.GradeTypeRepository.GetGradeTypeById(It.IsAny<int>()))
                .ReturnsAsync(GradeResources.GradeType);
            _mockUnitOfWork.Setup(x => x.GradeTypeRepository.GetAllAsync()).ReturnsAsync(GradeResources.GradeTypes);
            _mockUnitOfWork.Setup(x => x.YearStructureRepository.GetCurrentYearStructure())
                .ReturnsAsync(GradeResources.YearStructure);
            _mockUnitOfWork
                .Setup(x => x.GradeRepository.GetGradesForSpecificSubject(It.IsAny<int>(), It.IsAny<int>(),
                    It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(GradeResources.Grades);
            _mockUnitOfWork.Setup(x => x.GradeRepository.AddAsync(It.IsAny<Grade>())).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(x => x.CompleteAsync()).Returns(Task.CompletedTask);

            _mockStringLocalizer.Setup(x => x[It.IsAny<string>()])
                .Returns(new LocalizedString("", ""));

            _mockContextAccessor.SetupGet(x => x.HttpContext).Returns(GradeResources.MockHttpContext.Object);

            //Act
            var result = await _gradeController.AddGrade(GradeResources.AddGradeViewModel);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<AddGradeViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task AddGrade_ReturnsARedirect_AndAddsANewGrade()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.PupilRepository.GetPupilWithClassroomById(It.IsAny<int>()))
                .ReturnsAsync(GradeResources.PupilWithClass);
            _mockUnitOfWork.Setup(x => x.TeacherRepository.GetTeacherByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(GradeResources.Teacher);
            _mockUnitOfWork.Setup(x => x.SubjectRepository.GetSubjectByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(GradeResources.Subjects.First());
            _mockUnitOfWork.Setup(x => x.GradeTypeRepository.GetGradeTypeById(It.IsAny<int>()))
                .ReturnsAsync(GradeResources.GradeTypes.First());
            _mockUnitOfWork.Setup(x => x.YearStructureRepository.GetCurrentYearStructure())
                .ReturnsAsync(GradeResources.YearStructure);
            _mockUnitOfWork
                .Setup(x => x.GradeRepository.GetGradesForSpecificSubject(It.IsAny<int>(), It.IsAny<int>(),
                    It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(GradeResources.Grades);
            _mockUnitOfWork.Setup(x => x.GradeRepository.AddAsync(It.IsAny<Grade>())).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(x => x.CompleteAsync()).Returns(Task.CompletedTask);

            //Act
            var result = await _gradeController.AddGrade(GradeResources.AddGradeViewModel);

            //Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task AddGradePost_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.PupilRepository.GetPupilWithClassroomById(It.IsAny<int>()))
                .ThrowsAsync(new Exception());

            //Act

            var result = await _gradeController.AddGrade(GradeResources.AddGradeViewModel);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<ErrorViewModel>(viewResult.ViewData.Model);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task Edit_ReturnsAViewResult_WithAddViewModel()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.PupilRepository.GetPupilWithClassroomById(It.IsAny<int>()))
                .ReturnsAsync(GradeResources.PupilWithClass);
            _mockUnitOfWork.Setup(x => x.SubjectRepository.GetSubjectByName(It.IsAny<string>()))
                .ReturnsAsync(GradeResources.Subjects.First());

            _mockContextAccessor.SetupGet(x => x.HttpContext).Returns(GradeResources.MockHttpContext.Object);

            //Act
            var result = await _gradeController.Edit(It.IsAny<int>(), It.IsAny<string>());

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<AddGradeViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Edit_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.PupilRepository.GetPupilWithClassroomById(It.IsAny<int>()))
                .ThrowsAsync(new Exception());

            //Act
            var result = await _gradeController.Edit(It.IsAny<int>(), It.IsAny<string>());

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task Edit_ReturnsARedirect_AndUpdatesAGrade()
        {
            //Arrange

            _mockUnitOfWork.Setup(x => x.PupilRepository.GetPupilWithClassroomById(It.IsAny<int>()))
                .ReturnsAsync(GradeResources.PupilWithClass);
            _mockUnitOfWork.Setup(x => x.TeacherRepository.GetTeacherByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(GradeResources.Teacher);
            _mockUnitOfWork.Setup(x => x.SubjectRepository.GetSubjectByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(GradeResources.Subjects.First());
            _mockUnitOfWork.Setup(x => x.GradeTypeRepository.GetGradeTypeById(It.IsAny<int>()))
                .ReturnsAsync(GradeResources.GradeTypes.First());
            _mockUnitOfWork.Setup(x => x.GradeRepository.UpdateAsync(It.IsAny<Grade>())).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(x => x.CompleteAsync()).Returns(Task.CompletedTask);

            //Act
            var result = await _gradeController.Edit(GradeResources.AddGradeViewModel);

            //Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task EditPost_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.PupilRepository.GetPupilWithClassroomById(It.IsAny<int>()))
                .ThrowsAsync(new Exception());

            //Act

            var result = await _gradeController.Edit(GradeResources.AddGradeViewModel);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<ErrorViewModel>(viewResult.ViewData.Model);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task CalculateFinalGrade_ReturnsAViewResult_WithCalculateFinalGradeViewModel()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.PupilRepository.GetPupilWithClassroomById(It.IsAny<int>()))
                .ReturnsAsync(GradeResources.PupilWithClass);
            _mockUnitOfWork.Setup(x => x.SubjectRepository.GetSubjectByName(It.IsAny<string>()))
                .ReturnsAsync(GradeResources.Subjects.First());
            _mockUnitOfWork.Setup(x => x.YearStructureRepository.GetCurrentYearStructure())
                .ReturnsAsync(GradeResources.YearStructure);
            _mockUnitOfWork
                .Setup(x => x.GradeRepository.GetPupilGradesForThisYear(It.IsAny<int>(), It.IsAny<DateTime>(),
                    It.IsAny<DateTime>())).ReturnsAsync(GradeResources.Grades);

            //Act
            var result =
                await _gradeController.CalculateFinalGrade(It.IsAny<int>(), "Test", It.IsAny<int>());

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<CalculateFinalGradeViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task CalculateFinalGrade_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.PupilRepository.GetPupilWithClassroomById(It.IsAny<int>()))
                .ThrowsAsync(new Exception());

            //Act
            var result =
                await _gradeController.CalculateFinalGrade(It.IsAny<int>(), "Test", It.IsAny<int>());

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task CalculateFinalGrade_ReturnsTheSameView_WhenTheGradeIsARating()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.PupilRepository.GetPupilWithClassroomById(It.IsAny<int>()))
                .ReturnsAsync(GradeResources.PupilWithClass);
            _mockUnitOfWork.Setup(x => x.SubjectRepository.GetSubjectByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(GradeResources.Subjects.First());
            _mockUnitOfWork.Setup(x => x.YearStructureRepository.GetCurrentYearStructure())
                .ReturnsAsync(GradeResources.YearStructure);
            _mockUnitOfWork
                .Setup(x => x.GradeRepository.GetGradesForSpecificSubject(It.IsAny<int>(), It.IsAny<int>(),
                    It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(new List<Grade>{new(){Score = "Admis"}});
            _mockUnitOfWork
                .Setup(x => x.GradeRepository.GetPupilGradesForThisYear(It.IsAny<int>(), It.IsAny<DateTime>(),
                    It.IsAny<DateTime>())).ReturnsAsync(GradeResources.Grades);

            _mockStringLocalizer.Setup(x => x[It.IsAny<string>()]).Returns(new LocalizedString("", ""));

            //Act
            var result = await _gradeController.CalculateFinalGrade(GradeResources.CalculateFinalGradeViewModel);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<CalculateFinalGradeViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task CalculateFinalGrade_ReturnsTheSameView_WhenTheFinalGradeIs0()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.PupilRepository.GetPupilWithClassroomById(It.IsAny<int>()))
                .ReturnsAsync(GradeResources.PupilWithClass);
            _mockUnitOfWork.Setup(x => x.SubjectRepository.GetSubjectByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(GradeResources.Subjects.First());
            _mockUnitOfWork.Setup(x => x.YearStructureRepository.GetCurrentYearStructure())
                .ReturnsAsync(GradeResources.YearStructure);
            _mockUnitOfWork
                .Setup(x => x.GradeRepository.GetGradesForSpecificSubject(It.IsAny<int>(), It.IsAny<int>(),
                    It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(GradeResources.Grades);
            _mockUnitOfWork
                .Setup(x => x.GradeRepository.GetPupilGradesForThisYear(It.IsAny<int>(), It.IsAny<DateTime>(),
                    It.IsAny<DateTime>())).ReturnsAsync(new List<Grade>());

            _mockStringLocalizer.Setup(x => x[It.IsAny<string>()]).Returns(new LocalizedString("", ""));

            //Act
            var result = await _gradeController.CalculateFinalGrade(GradeResources.CalculateFinalGradeViewModel);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<CalculateFinalGradeViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task CalculateFinalGrade_ReturnsAViewResult_WithCalculateFinalGradeViewModel_AfterRecalculatingTheGrade()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.PupilRepository.GetPupilWithClassroomById(It.IsAny<int>()))
                .ReturnsAsync(GradeResources.PupilWithClass);
            _mockUnitOfWork.Setup(x => x.SubjectRepository.GetSubjectByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(GradeResources.Subjects.First());
            _mockUnitOfWork.Setup(x => x.YearStructureRepository.GetCurrentYearStructure())
                .ReturnsAsync(GradeResources.YearStructure);
            _mockUnitOfWork
                .Setup(x => x.GradeRepository.GetPupilGradesForThisYear(It.IsAny<int>(), It.IsAny<DateTime>(),
                    It.IsAny<DateTime>())).ReturnsAsync(GradeResources.Grades);
            _mockUnitOfWork.Setup(x => x.GradeRepository.UpdateAsync(It.IsAny<Grade>())).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(x => x.CompleteAsync()).Returns(Task.CompletedTask);
            _mockUnitOfWork
                .Setup(x => x.GradeRepository.GetGradesForSpecificSubject(It.IsAny<int>(), It.IsAny<int>(),
                    It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(GradeResources.Grades);

            //Act
            var result = await _gradeController.CalculateFinalGrade(GradeResources.CalculateFinalGradeViewModel);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<CalculateFinalGradeViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task CalculateFinalGrade_ReturnsAViewResult_WithCalculateFinalGradeViewModel_AfterCalculatingTheGrade()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.PupilRepository.GetPupilWithClassroomById(It.IsAny<int>()))
                .ReturnsAsync(GradeResources.PupilWithClass);
            _mockUnitOfWork.Setup(x => x.SubjectRepository.GetSubjectByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(GradeResources.Subjects.First());
            _mockUnitOfWork.Setup(x => x.YearStructureRepository.GetCurrentYearStructure())
                .ReturnsAsync(GradeResources.YearStructure);
            _mockUnitOfWork
                .Setup(x => x.GradeRepository.GetPupilGradesForThisYear(It.IsAny<int>(), It.IsAny<DateTime>(),
                    It.IsAny<DateTime>())).ReturnsAsync(GradeResources.GradesWithoutFinalGrade);

            _mockUnitOfWork.Setup(x => x.GradeTypeRepository.GetGradeTypeByName(It.IsAny<string>()))
                .ReturnsAsync(GradeResources.GradeTypes.First());
            _mockUnitOfWork.Setup(x => x.TeacherRepository.GetTeacherByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(GradeResources.Teacher);
            _mockUnitOfWork.Setup(x => x.GradeRepository.AddAsync(It.IsAny<Grade>())).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(x => x.CompleteAsync()).Returns(Task.CompletedTask);
            _mockUnitOfWork
                .Setup(x => x.GradeRepository.GetGradesForSpecificSubject(It.IsAny<int>(), It.IsAny<int>(),
                    It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(GradeResources.Grades);

            //Act
            var result = await _gradeController.CalculateFinalGrade(GradeResources.CalculateFinalGradeViewModel);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<CalculateFinalGradeViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task CalculateFinalGradePost_ReturnsAnErrorView_WhenAnExceptionIsCaught()
        {
            //Arrange
            _mockUnitOfWork.Setup(x => x.SubjectRepository.GetSubjectByIdAsync(It.IsAny<int>()))
                .ThrowsAsync(new Exception());

            //Act
            var result = await _gradeController.CalculateFinalGrade(GradeResources.CalculateFinalGradeViewModel);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }
    }
}
