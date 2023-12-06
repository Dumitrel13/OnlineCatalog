using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using OnlineCatalog.Models;
using OnlineCatalog.Repository.Interfaces;
using OnlineCatalog.ViewModels;

namespace OnlineCatalog.Controllers
{
    public class TeacherAssignmentController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        public TeacherAssignmentController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        [Authorize(Roles = "Director")]
        public async Task<IActionResult> Index(int teacherId, int year)
        {
            try
            {
                var teacherAssignments =
                    await _unitOfWork.TeacherAssignmentRepository.GetAllTeacherAssignmentsWithDataByDate(teacherId, year);
                var teacher = await _unitOfWork.TeacherRepository.GetTeacherByIdAsync(teacherId);

                var displayAssignments = new DisplayAssignmentsViewModel()
                {
                    TeacherAssignments = teacherAssignments,
                    TeacherFullName = teacher.FirstName + " " + teacher.LastName,
                    TeacherId = teacherId,
                };

                return View(displayAssignments);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        public async Task<IActionResult> Add(int teacherId)
        {
            try
            {
                var teacher = await _unitOfWork.TeacherRepository.GetTeacherByIdAsync(teacherId);
                var classrooms = await _unitOfWork.ClassroomRepository.GetAllAsync();
                var roles = await _unitOfWork.RoleRepository.GetAllAsync();
                var subjects = await _unitOfWork.SubjectRepository.GetAllAsync();
                var yearStructure = await _unitOfWork.YearStructureRepository.GetCurrentYearStructure();

                var teacherAssignmentViewModel = new TeacherAssignmentViewModel()
                {
                    TeacherAssignment = new TeacherAssignment()
                    {
                        StartingDate = yearStructure.StartingYear,
                        EndingDate = yearStructure.EndingYear,
                        Teacher = teacher,
                    },
                    YearStructure = yearStructure,
                    TeachersFullName = teacher.FirstName + " " + teacher.LastName,
                    Classrooms = classrooms.Select(c => new SelectListItem()
                    {
                        Text = c.Grade + " " + c.Group,
                        Value = c.ClassId.ToString()
                    }).ToList(),
                    Roles = roles.Where(r => !new[] { "Admin", "Parinte", "Elev" }.Contains(r.Name))
                    .Select(r => new SelectListItem()
                    {
                        Text = r.Name,
                        Value = r.Id.ToString()
                    }).ToList(),
                    Subjects = subjects.Select(s => new SelectListItem()
                    {
                        Text = s.Name,
                        Value = s.SubjectId.ToString()
                    }).ToList(),
                };

                return View(teacherAssignmentViewModel);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add(TeacherAssignmentViewModel assignmentViewModel)
        {
            try
            {
                var teacher = await _unitOfWork.TeacherRepository.GetTeacherByIdAsync(
                    assignmentViewModel.TeacherAssignment.Teacher.Id);

                if (assignmentViewModel.SelectedClassroom.IsNullOrEmpty() ||
                    assignmentViewModel.SelectedSubject.IsNullOrEmpty() ||
                    assignmentViewModel.SelectedRole.IsNullOrEmpty() ||
                    assignmentViewModel.TeacherAssignment.StartingDate > assignmentViewModel.TeacherAssignment.EndingDate ||
                    assignmentViewModel.TeacherAssignment.StartingDate == DateTime.MinValue ||
                    assignmentViewModel.TeacherAssignment.EndingDate == DateTime.MinValue ||
                    assignmentViewModel.TeacherAssignment.SubjectHours == 0)
                {
                    var classrooms = await _unitOfWork.ClassroomRepository.GetAllAsync();
                    var roles = await _unitOfWork.RoleRepository.GetAllAsync();
                    var subjects = await _unitOfWork.SubjectRepository.GetAllAsync();
                    var yearStructure = await _unitOfWork.YearStructureRepository.GetCurrentYearStructure();

                    var teacherAssignmentViewModel = new TeacherAssignmentViewModel()
                    {
                        TeacherAssignment = new TeacherAssignment()
                        {
                            StartingDate = yearStructure.StartingYear,
                            EndingDate = yearStructure.EndingYear,
                            Teacher = teacher,
                        },
                        YearStructure = yearStructure,
                        TeachersFullName = teacher.FirstName + " " + teacher.LastName,
                        Classrooms = classrooms.Select(c => new SelectListItem()
                        {
                            Text = c.Grade + " " + c.Group,
                            Value = c.ClassId.ToString()
                        }).ToList(),
                        Roles = roles.Where(r => !new[] { "Admin", "Parinte", "Elev" }.Contains(r.Name)).Select(r => new SelectListItem()
                        {
                            Text = r.Name,
                            Value = r.Id.ToString()
                        }).ToList(),
                        Subjects = subjects.Select(s => new SelectListItem()
                        {
                            Text = s.Name,
                            Value = s.SubjectId.ToString()
                        }).ToList(),
                    };

                    ModelState.Clear();
                    ModelState.AddModelError("InvalidInput", "Please enter valid data for all fields!");

                    return View(teacherAssignmentViewModel);
                }

                var classroom = await _unitOfWork.ClassroomRepository.GetClassroomByIdAsync(
                    Convert.ToInt32(assignmentViewModel.SelectedClassroom));
                var role = await _unitOfWork.RoleRepository.GetRoleByIdAsync(
                    Convert.ToInt32(assignmentViewModel.SelectedRole));
                var subject = await _unitOfWork.SubjectRepository.GetSubjectByIdAsync(
                        Convert.ToInt32(assignmentViewModel.SelectedSubject));

                assignmentViewModel.TeacherAssignment.Teacher = teacher;
                assignmentViewModel.TeacherAssignment.Classroom = classroom;
                assignmentViewModel.TeacherAssignment.ApplicationRole = role;
                assignmentViewModel.TeacherAssignment.Subject = subject;

                await _userManager.AddToRoleAsync(teacher, role.Name);

                await _unitOfWork.TeacherAssignmentRepository.AddAsync(assignmentViewModel.TeacherAssignment);
                await _unitOfWork.CompleteAsync();

                return RedirectToAction("Index", new { teacherId = assignmentViewModel.TeacherAssignment.Teacher.Id });
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        public async Task<IActionResult> Edit(int assignmentId)
        {
            try
            {
                var assignment = await _unitOfWork.TeacherAssignmentRepository.GetTeacherAssignmentById(assignmentId);
                var classroom = await _unitOfWork.ClassroomRepository.GetClassroomByIdAsync(assignment.Classroom.ClassId);
                var roles = await _unitOfWork.RoleRepository.GetAllAsync();
                var yearStructure = await _unitOfWork.YearStructureRepository.GetCurrentYearStructure();

                var teacherAssignmentViewModel = new TeacherAssignmentViewModel()
                {
                    TeacherAssignment = assignment,
                    YearStructure = yearStructure,
                    TeachersFullName = assignment.Teacher.FirstName + " " + assignment.Teacher.LastName,
                    ClassroomName = assignment.Classroom.Grade + " " + assignment.Classroom.Group,
                    Roles = roles.Where(r => !new[] { "Admin", "Parinte", "Elev" }.Contains(r.Name)).Select(r => new SelectListItem
                    {
                        Text = r.Name,
                        Value = r.Id.ToString()

                    }).ToList(),
                    Subjects = classroom.Subjects.Select(s => new SelectListItem
                    {
                        Text = s.Name,
                        Value = s.SubjectId.ToString()
                    }).ToList(),
                };

                return View(teacherAssignmentViewModel);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(TeacherAssignmentViewModel assignmentViewModel)
        {
            try
            {
                var teacher = await _unitOfWork.TeacherRepository.GetTeacherByIdAsync(
                    assignmentViewModel.TeacherAssignment.Teacher.Id);
                var currentClassroom = await _unitOfWork.ClassroomRepository.GetClassroomByIdAsync(assignmentViewModel.TeacherAssignment
                        .Classroom.ClassId);
                assignmentViewModel.TeacherAssignment.Teacher = teacher;
                assignmentViewModel.TeacherAssignment.Classroom = currentClassroom;

                if (assignmentViewModel.SelectedRole.IsNullOrEmpty())
                {
                    var role = await _unitOfWork.RoleRepository.GetRoleByIdAsync(assignmentViewModel.TeacherAssignment
                        .ApplicationRole.Id);
                    assignmentViewModel.TeacherAssignment.ApplicationRole = role;
                }
                else
                {
                    var role = await _unitOfWork.RoleRepository.GetRoleByIdAsync(
                        Convert.ToInt32(assignmentViewModel.SelectedRole));
                    var user = await _userManager.FindByIdAsync(teacher.Id.ToString());

                    if (assignmentViewModel.TeacherAssignment.ApplicationRole.Name != "Profesor")
                    {
                        await _userManager.RemoveFromRoleAsync(user, assignmentViewModel.TeacherAssignment.ApplicationRole.Name);
                    }

                    await _userManager.AddToRoleAsync(teacher, role.Name);

                    assignmentViewModel.TeacherAssignment.ApplicationRole = role;
                }

                if (assignmentViewModel.SelectedSubject.IsNullOrEmpty())
                {
                    var subject = await _unitOfWork.SubjectRepository.GetSubjectByIdAsync(assignmentViewModel.TeacherAssignment
                            .Subject.SubjectId);
                    assignmentViewModel.TeacherAssignment.Subject = subject;
                }
                else
                {
                    var subject = await _unitOfWork.SubjectRepository.GetSubjectByIdAsync(
                            Convert.ToInt32(assignmentViewModel.SelectedSubject));
                    assignmentViewModel.TeacherAssignment.Subject = subject;
                }

                //Validation checks
                if (assignmentViewModel.TeacherAssignment.StartingDate > assignmentViewModel.TeacherAssignment.EndingDate ||
                    assignmentViewModel.TeacherAssignment.StartingDate == DateTime.MinValue ||
                    assignmentViewModel.TeacherAssignment.EndingDate == DateTime.MinValue ||
                    assignmentViewModel.TeacherAssignment.SubjectHours == 0)
                {
                    var assignment = await _unitOfWork.TeacherAssignmentRepository
                        .GetTeacherAssignmentById(assignmentViewModel.TeacherAssignment.TeacherAssignmentId);
                    var classroom = await _unitOfWork.ClassroomRepository.GetClassroomByIdAsync(assignment.Classroom.ClassId);
                    var roles = await _unitOfWork.RoleRepository.GetAllAsync();
                    var yearStructure = await _unitOfWork.YearStructureRepository.GetCurrentYearStructure();

                    assignmentViewModel.TeacherAssignment = assignment;
                    assignmentViewModel.YearStructure = yearStructure;
                    assignmentViewModel.Roles = roles.Where(r => !new[] { "Admin", "Parinte", "Elev" }.Contains(r.Name)).Select(r =>
                        new SelectListItem
                        {
                            Text = r.Name,
                            Value = r.Id.ToString()

                        }).ToList();
                    assignmentViewModel.Subjects = classroom.Subjects.Select(s => new SelectListItem
                    {
                        Text = s.Name,
                        Value = s.SubjectId.ToString()
                    }).ToList();

                    return View(assignmentViewModel);
                }

                await _unitOfWork.TeacherAssignmentRepository.UpdateAsync(assignmentViewModel.TeacherAssignment);
                await _unitOfWork.CompleteAsync();

                return RedirectToAction("Index", new { teacherId = assignmentViewModel.TeacherAssignment.Teacher.Id });
            }
            catch (Exception)
            {
                return View("Error");
            }
        }
    }
}
