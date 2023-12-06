using Microsoft.AspNetCore.Mvc;
using OnlineCatalog.Models;
using OnlineCatalog.Repository.Interfaces;
using OnlineCatalog.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using OnlineCatalog.Helpers;
using OnlineCatalog.Helpers.Interfaces;

namespace OnlineCatalog.Controllers
{
    [Authorize]
    public class TeacherController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly IHttpContextAccessor _contextAccessor;

        public TeacherController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IEmailService emailService,
            IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _emailService = emailService;
            _contextAccessor = contextAccessor;
        }

        [Authorize(Roles = "Director, Admin")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var user = await _userManager.GetUserAsync(_contextAccessor.HttpContext.User);

                var roles = await _userManager.GetRolesAsync(user);

                if (roles.Contains("Director"))
                {
                    ViewData["PrincipalVisibility"] = "visible";
                }
                else
                {
                    ViewData["PrincipalVisibility"] = "invisible";
                }

                return View((await _unitOfWork.TeacherRepository.GetAllAsync()).Where(t => t.IsActive).OrderBy(t => t.LastName).ToList());
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Add()
        {
            try
            {
                var teacherModel = new TeacherModel()
                {
                    Teacher = new Teacher(),
                    Title = "New teacher account",
                    ButtonText = "CreateAnAccount",
                    ActionName = "Add"
                };
                return View(teacherModel);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add(TeacherModel teacherModel)
        {
            try
            {
                var emailAlreadyUsed = _userManager.Users.Any(x => x.Email == teacherModel.Teacher.Email);
                if (emailAlreadyUsed)
                {
                    ModelState.AddModelError("Teacher.Email", "Email already used!");
                    return View(teacherModel);
                }
                if (!ModelState.IsValid)
                {
                    return View(teacherModel);
                }

                if (teacherModel.SelectedGender.IsNullOrEmpty())
                {
                    ModelState.AddModelError("SelectedGender", "No gender selected!");
                    return View(teacherModel);
                }

                //Get the principal of the school
                var principalAccount = await _userManager.GetUsersInRoleAsync("Director");
                if (!principalAccount.IsNullOrEmpty())
                {
                    var actualPrincipal =
                        await _unitOfWork.TeacherRepository.GetTeacherWithSchoolById(principalAccount.First().Id);

                    teacherModel.Teacher.School = actualPrincipal.School ?? new School();
                }

                teacherModel.Teacher.UserName = teacherModel.Teacher.Email;
                teacherModel.Teacher.Gender = teacherModel.SelectedGender;

                var generatedPassword = PasswordGenerator.GeneratePassword();

                var response = await _emailService.AccountCreationEmail(teacherModel.Teacher.Email, generatedPassword);

                if (!response)
                {
                    return View("Error");
                }

                var result = await _userManager.CreateAsync(teacherModel.Teacher, generatedPassword);
                if (!result.Succeeded) return View(teacherModel);

                if (teacherModel.IsPrincipal)
                {

                    if (!principalAccount.IsNullOrEmpty())
                    {
                        var principalAssignments =
                            await _unitOfWork.TeacherAssignmentRepository.GetCurrentTeacherAssignmentsForSpecificTeacher(
                                principalAccount.First().Id);

                        foreach (var assignment in principalAssignments.Where(assignment => assignment.ApplicationRole.Name == "Director"))
                        {
                            assignment.EndingDate = DateTime.Now;
                        }

                        await _userManager.RemoveFromRoleAsync(principalAccount.First(), "Director");
                    }

                    var currentYearStructure = await _unitOfWork.YearStructureRepository.GetCurrentYearStructure();
                    var teacherAssignment = new TeacherAssignment
                    {
                        ApplicationRole = await _unitOfWork.RoleRepository.GetRoleByNameAsync("Director"),
                        StartingDate = currentYearStructure.StartingYear,
                        EndingDate = currentYearStructure.EndingYear,
                        Teacher = teacherModel.Teacher,
                        Classroom = (await _unitOfWork.ClassroomRepository.GetAllAsync()).First(),
                        Subject = (await _unitOfWork.SubjectRepository.GetAllAsync()).First()
                    };

                    await _unitOfWork.TeacherAssignmentRepository.AddAsync(teacherAssignment);
                    await _unitOfWork.CompleteAsync();

                    await _userManager.AddToRoleAsync(teacherModel.Teacher, "Director");
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int teacherId)
        {
            try
            {
                var teacher = await _unitOfWork.TeacherRepository.GetTeacherByIdAsync(teacherId);
                var model = new TeacherModel()
                {
                    Teacher = teacher,
                    Title = "Edit",
                    ButtonText = "SaveChanges",
                    ActionName = "Edit"
                };

                return View("Add", model);
            }
            catch (Exception)
            {
                var errorViewModel = new ErrorViewModel();
                return View("Error", errorViewModel);
            }

        }

        [HttpPost]
        public async Task<IActionResult> Edit(TeacherModel model)
        {
            try
            {
                var actualTeacher = await _unitOfWork.TeacherRepository.GetTeacherByIdAsync(model.Teacher.Id);

                var emailAlreadyUsed = _userManager.Users.Where(u => u.Email != actualTeacher.Email)
                    .Any(x => x.Email == model.Teacher.Email);

                if (emailAlreadyUsed)
                {
                    ModelState.AddModelError("Teacher.Email", "Email already used!");
                    return View("Add", model);
                }
                if (!ModelState.IsValid)
                {
                    return View("Add", model);
                }

                model.Teacher.Id = -1;

                actualTeacher.UserName = model.Teacher.Email;
                actualTeacher.NormalizedUserName = model.Teacher.Email.ToUpperInvariant();
                actualTeacher.Email = model.Teacher.Email;
                actualTeacher.NormalizedEmail = model.Teacher.Email.ToUpperInvariant();
                actualTeacher.FirstName = model.Teacher.FirstName;
                actualTeacher.LastName = model.Teacher.LastName;
                actualTeacher.Address = model.Teacher.Address;
                actualTeacher.IsActive = model.Teacher.IsActive;

                if (actualTeacher.Gender != model.SelectedGender && !model.SelectedGender.IsNullOrEmpty())
                {
                    actualTeacher.Gender = model.SelectedGender;
                }

                //email schimbare mail eventual

                await _unitOfWork.TeacherRepository.UpdateAsync(actualTeacher);
                await _unitOfWork.CompleteAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        public async Task<IActionResult> Details(int teacherId)
        {
            try
            {
                var teacher = await _unitOfWork.TeacherRepository.GetTeacherByIdAsync(teacherId);

                var teacherModel = new TeacherModel()
                {
                    Teacher = teacher,
                    IsVisible = "d-none",
                    Title = "Details"
                };

                ViewData["IsReadonly"] = "readonly";

                return View("Add", teacherModel);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        public async Task<IActionResult> Delete(int teacherId)
        {
            try
            {
                var teacher = await _unitOfWork.TeacherRepository.GetTeacherByIdAsync(teacherId);

                var deleteUserVm = new DeleteUserViewModel()
                {
                    User = teacher,
                    ActionControllerName = "Teacher"
                };

                return View("~/Views/Shared/ConfirmDelete.cshtml", deleteUserVm);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirm(DeleteUserViewModel model)
        {
            try
            {
                var teacher = await _unitOfWork.TeacherRepository.GetTeacherByIdAsync(model.User.Id);

                teacher.IsActive = false;

                await _userManager.SetTwoFactorEnabledAsync(teacher, true);
                await _userManager.ResetAuthenticatorKeyAsync(teacher);

                await _unitOfWork.TeacherRepository.UpdateAsync(teacher);
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

