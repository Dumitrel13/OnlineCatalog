using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using OnlineCatalog.Models;
using OnlineCatalog.Repository.Interfaces;
using OnlineCatalog.ViewModels;

namespace OnlineCatalog.Controllers
{
    [Authorize]
    public class ClassroomController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;


        public ClassroomController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _contextAccessor = contextAccessor;
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                return View(await _unitOfWork.ClassroomRepository.GetAllAsync());
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        public IActionResult Add()
        {
            try
            {
                var model = new ClassroomViewModel{ActionName = "Add"};

                return View(model);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add(ClassroomViewModel classroomViewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(classroomViewModel);
                }

                var principalAccount = await _userManager.GetUsersInRoleAsync("Director");
                var actualPrincipal =
                    await _unitOfWork.TeacherRepository.GetTeacherWithSchoolById(principalAccount.First().Id);

                classroomViewModel.Classroom.School = actualPrincipal.School ?? new School();

                classroomViewModel.Classroom.Grade = Convert.ToInt32(classroomViewModel.Grades
                    .First(g => g.Value == classroomViewModel.SelectedGrade).Text);

                classroomViewModel.Classroom.Group = classroomViewModel.Groups
                    .First(g => g.Value == classroomViewModel.SelectedGroup).Text;

                await _unitOfWork.ClassroomRepository.AddAsync(classroomViewModel.Classroom);
                await _unitOfWork.CompleteAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        [Authorize(Roles = "Profesor, Director")]
        [HttpGet]
        public async Task<IActionResult> MyClassrooms()
        {
            try
            {
                var user = await _userManager.GetUserAsync(_contextAccessor.HttpContext.User);
                var currentTeacherAssignments =
                    await _unitOfWork.TeacherAssignmentRepository.GetCurrentTeacherAssignmentsForSpecificTeacher(user.Id);
                var classes = currentTeacherAssignments.DistinctBy(ta => ta.Classroom)
                    .Select(ta => ta.Classroom).OrderBy(c => c.Grade).ThenBy(c => c.Group).ToList();

                return View(classes);
            }
            catch (Exception)
            {
                var errorViewModel = new ErrorViewModel();
                return View("Error", errorViewModel);
            }
        }

        [Authorize(Roles = "Director")]
        public IActionResult PromoteAllClassroomsDisplay()
        {
            return View("PromoteAllClassrooms");
        }

        [HttpPost]
        public async Task<IActionResult> PromoteAllClassrooms()
        {
            try
            {

                var classrooms = await _unitOfWork.ClassroomRepository.GetAllClassroomsWithPupilsAsync();

                var groupedClassrooms = classrooms.GroupBy(c => c.Group).ToDictionary(g => g.Key, g => g.ToList());


                foreach (var groupClassrooms in groupedClassrooms.Values)
                {
                    await PromoteClassrooms(groupClassrooms);
                }

                return RedirectToAction("Index", "Home");
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        private async Task PromoteClassrooms(IReadOnlyList<Classroom> classrooms)
        {
            try
            {
                var pupils = new List<Pupil>(classrooms.First().Pupils);
                classrooms.First().Pupils.Clear();
                await _unitOfWork.ClassroomRepository.UpdateAsync(classrooms.First());

                for (var i = 1; i < classrooms.Count; i++)
                {
                    var tempPupils = new List<Pupil>(classrooms[i].Pupils);
                    classrooms[i].Pupils = pupils;
                    await _unitOfWork.ClassroomRepository.UpdateAsync(classrooms[i]);

                    pupils = tempPupils;
                }

                foreach (var pupil in pupils)
                {
                    pupil.Classroom = null;
                    await _unitOfWork.PupilRepository.UpdateAsync(pupil);
                }

                await _unitOfWork.CompleteAsync();
            }
            catch (Exception)
            {
                throw new Exception();
            }
        }

        [Authorize(Roles = "Director")]
        [HttpGet("[controller]/AssignSubjects/{classroomId:int}")]
        public async Task<IActionResult> AssignSubjects(int classroomId)
        {
            try
            {
                var classroom = await _unitOfWork.ClassroomRepository.GetClassroomByIdAsync(classroomId);
                var subjects = await _unitOfWork.SubjectRepository.GetAllAsync();

                var assignSubjectsViewModel = new AssignSubjectsViewModel()
                {
                    Classroom = classroom,
                    Subjects = subjects.Except(classroom.Subjects).OrderBy(s => s.Name).ToList(),

                };

                return View(assignSubjectsViewModel);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AssignSubjects(AssignSubjectsViewModel assignSubjectsViewModel)
        {
            try
            {
                var subjectsId = JsonConvert.DeserializeObject<List<int>>(assignSubjectsViewModel.SubjectsId);

                ModelState.Clear();
                if (subjectsId.IsNullOrEmpty())
                {
                    ModelState.AddModelError("Subjects", "No subjects were selected!");

                    assignSubjectsViewModel.Classroom = await _unitOfWork.ClassroomRepository.GetClassroomByIdAsync(
                        assignSubjectsViewModel.Classroom
                            .ClassId);
                    assignSubjectsViewModel.Subjects = (await _unitOfWork.SubjectRepository.GetAllAsync()).ToList();

                    return View(assignSubjectsViewModel);
                }

                var subjects = await _unitOfWork.SubjectRepository.GetSubjectsByIds(subjectsId);

                var classroom =
                    await _unitOfWork.ClassroomRepository.GetClassroomByIdAsync(assignSubjectsViewModel.Classroom
                        .ClassId);

                classroom.Subjects.AddRange(subjects);

                await _unitOfWork.ClassroomRepository.UpdateAsync(classroom);
                await _unitOfWork.CompleteAsync();

                assignSubjectsViewModel.Subjects = (await _unitOfWork.SubjectRepository.GetAllAsync())
                    .Except(classroom.Subjects).ToList();
                assignSubjectsViewModel.Classroom = classroom;

                return View(assignSubjectsViewModel);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        public async Task<IActionResult> RemoveSubjectFromList(int classroomId, int subjectId)
        {
            try
            {
                var classroom = await _unitOfWork.ClassroomRepository.GetClassroomByIdAsync(classroomId);
                var subject = await _unitOfWork.SubjectRepository.GetSubjectByIdAsync(subjectId);

                classroom.Subjects.Remove(subject);

                await _unitOfWork.ClassroomRepository.UpdateAsync(classroom);
                await _unitOfWork.CompleteAsync();

                return RedirectToAction("AssignSubjects", new { classroomId = classroomId });
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        [Authorize(Roles = "Director")]
        public async Task<IActionResult> AssignPupils(int classroomId)
        {
            try
            {
                var pupils = (await _unitOfWork.PupilRepository.GetAllPupilsWithClassroom()).ToList();

                var assignPupilsViewModel = new AssignPupilsViewModel()
                {
                    Classroom = await _unitOfWork.ClassroomRepository.GetClassroomByIdAsync(classroomId),
                    Pupils = pupils.Where(p => p.Classroom == null).ToList()
                };

                return View(assignPupilsViewModel);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AssignPupils(AssignPupilsViewModel assignPupilsViewModel)
        {
            try
            {
                var pupilIds = JsonConvert.DeserializeObject<List<int>>(assignPupilsViewModel.PupilsIds);

                ModelState.Clear();
                if (pupilIds.IsNullOrEmpty())
                {
                    ModelState.AddModelError("Pupils", "No pupils were selected!");

                    assignPupilsViewModel.Classroom = await _unitOfWork.ClassroomRepository.GetClassroomByIdAsync(
                        assignPupilsViewModel.Classroom
                            .ClassId);
                    assignPupilsViewModel.Pupils = (await _unitOfWork.PupilRepository.GetAllPupilsWithClassroom())
                        .Where(p => p.Classroom == null).ToList();

                    return View(assignPupilsViewModel);
                }

                var pupils = await _unitOfWork.PupilRepository.GetPupilsByIds(pupilIds);

                var classroom =
                    await _unitOfWork.ClassroomRepository.GetClassroomByIdAsync(assignPupilsViewModel.Classroom
                        .ClassId);

                classroom.Pupils.AddRange(pupils);

                await _unitOfWork.ClassroomRepository.UpdateAsync(classroom);
                await _unitOfWork.CompleteAsync();

                assignPupilsViewModel.Classroom = classroom;

                return View(assignPupilsViewModel);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        public async Task<IActionResult> RemovePupilFromList(int classroomId, int pupilId)
        {
            try
            {
                var classroom = await _unitOfWork.ClassroomRepository.GetClassroomByIdAsync(classroomId);
                var pupil = await _unitOfWork.PupilRepository.GetPupilWithClassroomById(pupilId);

                classroom.Pupils.Remove(pupil);

                await _unitOfWork.ClassroomRepository.UpdateAsync(classroom);
                await _unitOfWork.CompleteAsync();

                return RedirectToAction("AssignPupils", new { classroomId = classroomId });
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        public async Task<IActionResult> Edit(int classroomId)
        {
            try
            {
                var classroom = await _unitOfWork.ClassroomRepository.GetClassroomByIdAsync(classroomId);

                var model = new ClassroomViewModel { Classroom = classroom, ActionName = "Edit" };

                return View("Add", model);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ClassroomViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("Add", model);
                }

                var actualClassroom =
                    await _unitOfWork.ClassroomRepository.GetClassroomByIdAsync(model.Classroom.ClassId);

                actualClassroom.Grade = Convert.ToInt32(model.Grades
                    .First(g => g.Value == model.SelectedGrade).Text);

                actualClassroom.Group = model.Groups
                    .First(g => g.Value == model.SelectedGroup).Text;

                await _unitOfWork.ClassroomRepository.UpdateAsync(actualClassroom);
                await _unitOfWork.CompleteAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                return View("Error");
            }
        }
    }
}
