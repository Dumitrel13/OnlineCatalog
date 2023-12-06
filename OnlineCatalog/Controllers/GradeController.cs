using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using OnlineCatalog.Helpers;
using OnlineCatalog.Models;
using OnlineCatalog.Repository.Interfaces;
using OnlineCatalog.ViewModels;

namespace OnlineCatalog.Controllers
{
    public class GradeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public GradeController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IHttpContextAccessor contextAccessor,
            IStringLocalizer<SharedResources> localizer)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _contextAccessor = contextAccessor;
            _localizer = localizer;
        }

        [HttpGet("[controller]/Index/{pupilId:int}")]
        public async Task<IActionResult> Index(int pupilId)
        {
            try
            {
                var pupil = await _unitOfWork.PupilRepository.GetPupilWithClassroomById(pupilId);

                //Check if the classroom of the pupil is found in the user assignments
                var user = await _userManager.GetUserAsync(_contextAccessor.HttpContext.User);
                var teacherAssignments =
                    await _unitOfWork.TeacherAssignmentRepository.GetCurrentTeacherAssignmentsForSpecificTeacher(
                        user.Id);

                if (pupil.Classroom == null || teacherAssignments.All(ta => ta.Classroom.ClassId != pupil.Classroom.ClassId))
                {
                    throw new Exception();
                }

                //Get the pupil's grades for the current year
                var currentYear = await _unitOfWork.YearStructureRepository.GetCurrentYearStructure();
                var grades =
                    await _unitOfWork.GradeRepository.GetPupilGradesForThisYear(pupilId, currentYear.StartingYear,
                        currentYear.EndingYear);

                var gradeViewModel = new GradeViewModel
                {
                    PupilFullName = pupil.FirstName + " " + pupil.LastName,
                    PupilId = pupilId,
                    TeacherId = user.Id
                };

                //Get the subjects for the logged in teacher
                var currentTeacherAssignment =
                    await _unitOfWork.TeacherAssignmentRepository
                        .GetCurrentTeacherAssignmentsWithDataForSpecificTeacherAndClass(user.Id,
                            pupil.Classroom.ClassId);

                gradeViewModel.AvailableSubjects.AddRange(currentTeacherAssignment.Select(ta => ta.Subject.Name)
                    .ToList());

                gradeViewModel.Grades = grades.Where(g => gradeViewModel.AvailableSubjects.Contains(g.Subject.Name))
                    .ToList();
                ViewData["IsVisible"] = "visible";

                return View(gradeViewModel);
            }
            catch (Exception)
            {
                var errorViewModel = new ErrorViewModel();
                return View("Error", errorViewModel);
            }
        }

        public async Task<IActionResult> DisplayGradesForSpecificPupil(int pupilId)
        {
            try
            {
                var user = await _userManager.GetUserAsync(_contextAccessor.HttpContext.User);
                Pupil pupil = null;

                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles.Contains("Parinte"))
                    {
                        pupil = await _unitOfWork.PupilRepository.GetPupilWithClassroomByIdForParent(pupilId, user.Id);
                    }

                    if (roles.Contains("Elev"))
                    {
                        if (user.Id != pupilId)
                        {
                            throw new Exception();
                        }
                        pupil = await _unitOfWork.PupilRepository.GetPupilWithClassroomById(pupilId);
                    }

                }

                if (pupil == null)
                {
                    throw new Exception();
                }

                var currentYear = await _unitOfWork.YearStructureRepository.GetCurrentYearStructure();
                var grades =
                    await _unitOfWork.GradeRepository.GetPupilGradesForThisYear(pupilId, currentYear.StartingYear,
                        currentYear.EndingYear);

                var classroom = await _unitOfWork.ClassroomRepository.GetClassroomByIdAsync(pupil.Classroom.ClassId);
                var available = classroom.Subjects.OrderBy(s => s.Name).Select(s => s.Name).ToList();

                var gradeViewModel = new GradeViewModel
                {
                    PupilFullName = pupil.FirstName + " " + pupil.LastName,
                    PupilId = pupilId,
                    Grades = grades,
                    AvailableSubjects = available
                };
                ViewData["IsVisible"] = "invisible";
                return View("Index", gradeViewModel);
            }
            catch (Exception)
            {
                var errorViewModel = new ErrorViewModel();
                return View("Error", errorViewModel);
            }
        }

        public async Task<IActionResult> AddGrade(string subject, int pupilId)
        {
            try
            {
                var addGradeViewModel = new AddGradeViewModel();
                var currentTeacher = (Teacher)await _userManager.GetUserAsync(_contextAccessor.HttpContext.User);
                var selectedPupil = await _unitOfWork.PupilRepository.GetPupilWithClassroomById(pupilId);

                var grade = new Grade
                {
                    Date = DateTime.Now,
                    Subject = await _unitOfWork.SubjectRepository.GetSubjectByName(subject),
                    Teacher = currentTeacher,
                    Pupil = selectedPupil
                };

                addGradeViewModel.PupilFullName = selectedPupil.FirstName + " " + selectedPupil.LastName;
                addGradeViewModel.TeacherFullName = currentTeacher.FirstName + " " + currentTeacher.LastName;
                addGradeViewModel.FormattedDate = DateTime.Now.Day + " " +
                                                 DateTime.Now.ToString("MMMM", CultureInfo.InvariantCulture) + " " +
                                                 DateTime.Now.Year;
                addGradeViewModel.Grade = grade;

                var yearStructure = await _unitOfWork.YearStructureRepository.GetCurrentYearStructure();
                var gradeTypes = (await _unitOfWork.GradeTypeRepository.GetAllAsync()).Where(x => x.Type != "Media").ToList();

                if (!yearStructure.AllowExam)
                {
                    gradeTypes.Remove(gradeTypes.First(x => x.Type == "Teza"));
                }

                addGradeViewModel.Types = gradeTypes.Select(x =>
                    new SelectListItem
                    {
                        Value = x.GradeTypeId.ToString(),
                        Text = x.Type

                    }).ToList();

                return View(addGradeViewModel);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddGrade(AddGradeViewModel addGradeViewModel)
        {
            try
            {
                var pupil = await _unitOfWork.PupilRepository.GetPupilWithClassroomById(addGradeViewModel.Grade.Pupil.Id);
                var teacher = await _unitOfWork.TeacherRepository.GetTeacherByIdAsync(addGradeViewModel.Grade.Teacher.Id);
                var subject = await _unitOfWork.SubjectRepository.GetSubjectByIdAsync(addGradeViewModel.Grade.Subject.SubjectId);
                var gradeType = await _unitOfWork.GradeTypeRepository.GetGradeTypeById(Convert.ToInt32(addGradeViewModel.SelectedType));

                var yearStructure = await _unitOfWork.YearStructureRepository.GetCurrentYearStructure();

                var subjectGrades = await _unitOfWork.GradeRepository.GetGradesForSpecificSubject(pupil.Id, subject.SubjectId,
                    yearStructure.StartingYear, yearStructure.EndingYear);

                var containsNumbersOnly = subjectGrades.Select(g => g.Score).All(g => int.TryParse(g, out _));
                var isSelectedScoreNumber = int.TryParse(addGradeViewModel.SelectedScore, out _);

                //A rating already exists
                if (!containsNumbersOnly && !isSelectedScoreNumber)
                {
                    ModelState.Clear();
                    ModelState.AddModelError("Score", _localizer["A rating was already added, you cannot add another one!"]);
                    return await AddGrade(subject.Name, pupil.Id);
                }

                //Trying to add rating on grade and vice versa
                if ((containsNumbersOnly && !isSelectedScoreNumber) || (!containsNumbersOnly && isSelectedScoreNumber))
                {
                    ModelState.Clear();
                    ModelState.AddModelError("Score", _localizer["The type of the selected score does not match the type of the existing grades!"]);
                    return await AddGrade(subject.Name, pupil.Id);
                }

                //Checking if the pupil has an exam grade
                if (gradeType.Type == "Teza")
                {
                    if (subjectGrades.Any(x => x.GradeType.Type == gradeType.Type))
                    {
                        ModelState.Clear();
                        ModelState.AddModelError("Score", _localizer["A final grade already exists for this subject!"]);
                        return await AddGrade(subject.Name, pupil.Id);
                    }
                }

                addGradeViewModel.Grade.Score = addGradeViewModel.SelectedScore;
                addGradeViewModel.Grade.GradeType = gradeType;
                addGradeViewModel.Grade.Subject = subject;
                addGradeViewModel.Grade.Teacher = teacher;
                addGradeViewModel.Grade.Pupil = pupil;

                await _unitOfWork.GradeRepository.AddAsync(addGradeViewModel.Grade);
                await _unitOfWork.CompleteAsync();

                return RedirectToAction(nameof(Index), new { pupilId = pupil.Id });
            }
            catch (Exception e)
            {
                var errorViewModel = new ErrorViewModel();
                return View("Error", errorViewModel);
            }

        }

        public async Task<IActionResult> Edit(int pupilId, string subject)
        {
            try
            {
                var addGradeViewModel = new AddGradeViewModel();
                var currentTeacher = (Teacher)await _userManager.GetUserAsync(_contextAccessor.HttpContext.User);
                var selectedPupil = await _unitOfWork.PupilRepository.GetPupilWithClassroomById(pupilId);

                var grade = new Grade
                {
                    Date = DateTime.Now,
                    Subject = await _unitOfWork.SubjectRepository.GetSubjectByName(subject),
                    Teacher = currentTeacher,
                    Pupil = selectedPupil
                };

                addGradeViewModel.PupilFullName = selectedPupil.FirstName + " " + selectedPupil.LastName;
                addGradeViewModel.TeacherFullName = currentTeacher.FirstName + " " + currentTeacher.LastName;
                addGradeViewModel.FormattedDate = DateTime.Now.Day + " " +
                                                  DateTime.Now.ToString("MMMM", CultureInfo.InvariantCulture) + " " +
                                                  DateTime.Now.Year;
                addGradeViewModel.Grade = grade;

                return View(addGradeViewModel);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AddGradeViewModel addGradeViewModel)
        {
            try
            {
                var pupil = await _unitOfWork.PupilRepository.GetPupilWithClassroomById(addGradeViewModel.Grade.Pupil.Id);
                var teacher = await _unitOfWork.TeacherRepository.GetTeacherByIdAsync(addGradeViewModel.Grade.Teacher.Id);
                var subject = await _unitOfWork.SubjectRepository.GetSubjectByIdAsync(addGradeViewModel.Grade.Subject.SubjectId);
                var gradeType = await _unitOfWork.GradeTypeRepository.GetGradeTypeByName("Nota");

                addGradeViewModel.Grade.Score = addGradeViewModel.SelectedScore;
                addGradeViewModel.Grade.GradeType = gradeType;
                addGradeViewModel.Grade.Subject = subject;
                addGradeViewModel.Grade.Teacher = teacher;
                addGradeViewModel.Grade.Pupil = pupil;

                await _unitOfWork.GradeRepository.UpdateAsync(addGradeViewModel.Grade);
                await _unitOfWork.CompleteAsync();

                return RedirectToAction(nameof(Index), new { pupilId = pupil.Id });
            }
            catch (Exception)
            {
                var errorViewModel = new ErrorViewModel();
                return View("Error", errorViewModel);
            }
        }

        [HttpGet]
        public async Task<IActionResult> CalculateFinalGrade(int pupilId, string subjectName, int teacherId)
        {
            try
            {
                var pupil = await _unitOfWork.PupilRepository.GetPupilWithClassroomById(pupilId);
                var subject = await _unitOfWork.SubjectRepository.GetSubjectByName(subjectName);

                var currentYearStructure = await _unitOfWork.YearStructureRepository.GetCurrentYearStructure();
                var grades = (await _unitOfWork.GradeRepository.GetPupilGradesForThisYear(pupilId,
                    currentYearStructure.StartingYear, currentYearStructure.EndingYear)).Where(g => g.Subject.Name == subjectName).ToList();

                var calculateFinalGradeViewModel = new CalculateFinalGradeViewModel()
                {
                    Grades = grades,
                    Subject = subject,
                    PupilFullName = pupil.FirstName + " " + pupil.LastName,
                    ExamGrade = grades.SingleOrDefault(g => g.GradeType.Type == "Teza"),
                    FinalGrade = grades.SingleOrDefault(g => g.GradeType.Type == "Media")?.Score,
                    PupilId = pupilId,
                    TeacherId = teacherId
                };

                return View(calculateFinalGradeViewModel);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CalculateFinalGrade(CalculateFinalGradeViewModel calculateFinalGradeViewModel)
        {
            try
            {
                var subject = await _unitOfWork.SubjectRepository.GetSubjectByIdAsync(calculateFinalGradeViewModel.Subject.SubjectId);
                var yearStructure = await _unitOfWork.YearStructureRepository.GetCurrentYearStructure();

                var subjectGrades = await _unitOfWork.GradeRepository.GetGradesForSpecificSubject(calculateFinalGradeViewModel.PupilId,
                    calculateFinalGradeViewModel.Subject.SubjectId, yearStructure.StartingYear, yearStructure.EndingYear);

                var containsNumbersOnly = subjectGrades.Select(g => g.Score).All(g => int.TryParse(g, out _));

                //If the grade is a rating
                if (!containsNumbersOnly)
                {
                    ModelState.Clear();
                    ModelState.AddModelError("Grades", _localizer["You cannot calculate the final grade because this subject has no grades!"]);
                    return await CalculateFinalGrade(calculateFinalGradeViewModel.PupilId, subject.Name,
                        calculateFinalGradeViewModel.TeacherId);
                }

                var pupil = await _unitOfWork.PupilRepository.GetPupilWithClassroomById(calculateFinalGradeViewModel.PupilId);


                var currentYearStructure = await _unitOfWork.YearStructureRepository.GetCurrentYearStructure();
                var grades = (await _unitOfWork.GradeRepository.GetPupilGradesForThisYear(calculateFinalGradeViewModel.PupilId,
                    currentYearStructure.StartingYear, currentYearStructure.EndingYear)).Where(g => g.Subject.Name == subject.Name).ToList();

                var finalGradeCalculator = new FinalGradeCalculator();

                var examGrade = grades.SingleOrDefault(g => g.GradeType.Type == "Teza");

                //Check if the pupil has an exam grade for this subject
                string finalGrade;
                if (examGrade != null)
                {
                    finalGradeCalculator.SetStrategy(new FinalGradeWithExam());
                    finalGrade = finalGradeCalculator.GetFinalGrade(
                        grades.Where(g => g.GradeType.Type != "Teza" && g.GradeType.Type != "Media").ToList(), examGrade);
                }
                else
                {
                    finalGradeCalculator.SetStrategy(new FinalGradeWithoutExam());
                    finalGrade = finalGradeCalculator.GetFinalGrade(grades.Where(g => g.GradeType.Type != "Media").ToList(), null);
                }

                calculateFinalGradeViewModel.FinalGrade = finalGrade;
                calculateFinalGradeViewModel.Grades = grades;
                calculateFinalGradeViewModel.Subject = subject;
                calculateFinalGradeViewModel.PupilFullName = pupil.FirstName + " " + pupil.LastName;
                calculateFinalGradeViewModel.ExamGrade = grades.SingleOrDefault(g => g.GradeType.Type == "Teza");

                ModelState.Clear();
                if (Convert.ToInt32(finalGrade) == 0)
                {
                    ModelState.AddModelError("Grade", _localizer["This pupil has no grades!"]);
                    return View(calculateFinalGradeViewModel);
                }

                var grade = grades.SingleOrDefault(g => g.GradeType.Type == "Media");

                if (grade != null)
                {
                    grade.Score = finalGrade;
                    grade.Date = DateTime.Now;

                    await _unitOfWork.GradeRepository.UpdateAsync(grade);
                    await _unitOfWork.CompleteAsync();
                }
                else
                {
                    var type = await _unitOfWork.GradeTypeRepository.GetGradeTypeByName("Media");
                    var teacher = await _unitOfWork.TeacherRepository.GetTeacherByIdAsync(calculateFinalGradeViewModel.TeacherId);

                    grade = new Grade
                    {
                        Date = DateTime.Now,
                        GradeType = type,
                        Pupil = pupil,
                        Score = finalGrade,
                        Subject = subject,
                        Teacher = teacher
                    };
                    await _unitOfWork.GradeRepository.AddAsync(grade);
                    await _unitOfWork.CompleteAsync();
                }

                return View(calculateFinalGradeViewModel);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }
    }
}
