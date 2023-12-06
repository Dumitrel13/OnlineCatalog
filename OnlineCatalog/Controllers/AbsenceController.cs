using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OnlineCatalog.Models;
using OnlineCatalog.Repository.Interfaces;
using OnlineCatalog.ViewModels;

namespace OnlineCatalog.Controllers
{
    public class AbsenceController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;

        public AbsenceController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _contextAccessor = contextAccessor;
        }
        public async Task<IActionResult> Index(int pupilId)
        {
            try
            {
                var pupil = await _unitOfWork.PupilRepository.GetPupilWithClassroomById(pupilId);
                var user = await _userManager.GetUserAsync(_contextAccessor.HttpContext.User);
                var teacherAssignments =
                    await _unitOfWork.TeacherAssignmentRepository
                        .GetCurrentTeacherAssignmentsWithDataForSpecificTeacherAndClass(user.Id,
                            pupil.Classroom.ClassId);

                if (teacherAssignments.IsNullOrEmpty())
                {
                    throw new Exception();
                }

                var absenceViewModel = new AbsenceViewModel()
                {
                    PupilFullName = pupil.FirstName + " " + pupil.LastName,
                    PupilId = pupilId,
                };

                if (teacherAssignments.All(ta => ta.ApplicationRole.Name == "Diriginte"))
                {
                    absenceViewModel.ClassMasterSubjects = teacherAssignments.Where(ta => ta.Subject.Name != "Purtare")
                        .Select(ta => ta.Subject.Name).ToList();
                    absenceViewModel.ClassMasterVisibility = "visible";
                    var classroom =
                        await _unitOfWork.ClassroomRepository.GetClassroomByIdAsync(pupil.Classroom.ClassId);
                    var subjects = classroom.Subjects;
                    absenceViewModel.Subjects = subjects.Where(s => s.Name != "Purtare").ToList();
                }
                else
                {
                    absenceViewModel.ClassMasterVisibility = "invisible";
                    absenceViewModel.Subjects = teacherAssignments.Where(ta => ta.Subject.Name != "Purtare")
                        .Select(ta => ta.Subject).ToList();
                }

                foreach (var subject in absenceViewModel.Subjects)
                {
                    absenceViewModel.Absences.AddRange(await _unitOfWork.AbsenceRepository
                        .GetAbsencesByPupilIdAndSubjectId(pupilId, subject.SubjectId));
                }

                ViewData["IsVisible"] = "visible";
                return View(absenceViewModel);
            }
            catch (Exception)
            {
                var errorViewModel = new ErrorViewModel();
                return View("Error", errorViewModel);
            }
        }

        public async Task<IActionResult> DisplayAbsencesForSpecificPupil(int pupilId)
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
                var absences = await _unitOfWork.AbsenceRepository.GetAbsencesForTheCurrentYearByPupilId(pupilId,
                    currentYear.StartingYear, currentYear.EndingYear);
                var pupilsClassroom =
                    await _unitOfWork.ClassroomRepository.GetClassroomByIdAsync(pupil.Classroom.ClassId);

                var absenceViewModel = new AbsenceViewModel()
                {
                    PupilId = pupilId,
                    PupilFullName = pupil.FirstName + " " + pupil.LastName,
                    Subjects = pupilsClassroom.Subjects.Where(s => s.Name != "Purtare").ToList(),
                    Absences = absences,
                    ClassMasterVisibility = "invisible"
                };

                ViewData["IsVisible"] = "invisible";
                return View("Index", absenceViewModel);
            }
            catch (Exception)
            {
                var errorViewModel = new ErrorViewModel();
                return View("Error", errorViewModel);
            }
        }

        public async Task<IActionResult> Add(int pupilId, int subjectId)
        {
            try
            {

                var pupil = await _unitOfWork.PupilRepository.GetPupilWithClassroomById(pupilId);
                var subject = await _unitOfWork.SubjectRepository.GetSubjectByIdAsync(subjectId);
                var user = (Teacher)await _userManager.GetUserAsync(_contextAccessor.HttpContext.User);

                var addAbsenceViewModel = new AddAbsenceViewModel()
                {
                    Absence = new Absence()
                    {
                        Date = DateTime.Now,
                        Pupil = pupil,
                        Subject = subject,
                        Teacher = user
                    },
                    FormattedDate = DateTime.Now.Day + " " +
                                    DateTimeFormatInfo.CurrentInfo.GetMonthName(DateTime.Now.Month) + " " +
                                    DateTime.Now.Year,
                    PupilFullName = pupil.FirstName + " " + pupil.LastName,
                    TeacherFullName = user.FirstName + " " + user.LastName,
                };

                return View(addAbsenceViewModel);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddAbsenceViewModel addAbsenceViewModel)
        {
            try
            {
                var teacher =
                    await _unitOfWork.TeacherRepository.GetTeacherByIdAsync(addAbsenceViewModel.Absence.Teacher.Id);
                var pupil = await _unitOfWork.PupilRepository.GetPupilWithClassroomById(addAbsenceViewModel.Absence
                    .Pupil.Id);
                var subject =
                    await _unitOfWork.SubjectRepository.GetSubjectByIdAsync(addAbsenceViewModel.Absence.Subject
                        .SubjectId);

                addAbsenceViewModel.Absence.Teacher = teacher;
                addAbsenceViewModel.Absence.Pupil = pupil;
                addAbsenceViewModel.Absence.Subject = subject;

                var absences =
                    await _unitOfWork.AbsenceRepository.GetAbsencesByPupilIdAndSubjectId(pupil.Id, subject.SubjectId);

                if (absences.Any(absence => absence.Date.Date == DateTime.Now.Date &&
                                            (absence.StartingHour == addAbsenceViewModel.Absence.StartingHour ||
                                             absence.EndingHour == addAbsenceViewModel.Absence.EndingHour)))
                {
                    ModelState.Clear();
                    ModelState.AddModelError("FormattedDate",
                        "An absence already exists for this pupil and on this subject in this time interval!");
                    return View(addAbsenceViewModel);
                }

                await _unitOfWork.AbsenceRepository.AddAsync(addAbsenceViewModel.Absence);
                await _unitOfWork.CompleteAsync();

                return RedirectToAction(nameof(Index), new { pupilId = pupil.Id });
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int pupilId, int subjectId, string isClassMaster)
        {
            try
            {
                var currentYearStructure = await _unitOfWork.YearStructureRepository.GetCurrentYearStructure();
                var absences =
                    (await _unitOfWork.AbsenceRepository.GetAbsencesByPupilIdAndSubjectId(pupilId, subjectId))
                    .Where(a => a.Date.Date >= currentYearStructure.StartingYear.Date &&
                                a.Date.Date <= currentYearStructure.EndingYear.Date).ToList();
                var pupil = await _unitOfWork.PupilRepository.GetPupilWithClassroomById(pupilId);

                var absenceViewModel = new AbsenceViewModel()
                {
                    PupilId = pupilId,
                    PupilFullName = pupil.FirstName + " " + pupil.LastName,
                    Absences = absences,
                    ClassMasterVisibility = isClassMaster
                };

                return View(absenceViewModel);
            }
            catch (Exception)
            {
                return View("Error");
            }

        }

        public async Task<IActionResult> Justify(int absenceId, int pupilId)
        {
            try
            {
                var absence = await _unitOfWork.AbsenceRepository.GetAbsenceById(absenceId);

                absence.IsJustified = true;

                await _unitOfWork.AbsenceRepository.UpdateAsync(absence);
                await _unitOfWork.CompleteAsync();

                return RedirectToAction(nameof(Index), new { pupilId = pupilId });
            }
            catch (Exception)
            {
                return View("Error");
            }
        }
    }
}
