using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OnlineCatalog.Helpers;
using OnlineCatalog.Helpers.Interfaces;
using OnlineCatalog.Models;
using OnlineCatalog.Repository.Interfaces;
using OnlineCatalog.ViewModels;

namespace OnlineCatalog.Controllers
{
    [Authorize]
    public class PupilController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IEmailService _emailService;

        public PupilController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IHttpContextAccessor contextAccessor,
            IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _contextAccessor = contextAccessor;
            _emailService = emailService;
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                return View((await _unitOfWork.PupilRepository.GetAllAsync()).Where(p => p.IsActive).ToList());
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        //Display the pupils with the available actions from the teacher's point of view 
        [HttpGet("[controller]/DisplaySpecificPupils/{classroomId:int}")]
        public async Task<IActionResult> DisplaySpecificPupils(int classroomId)
        {
            try
            {
                var classroom = await _unitOfWork.ClassroomRepository.GetClassroomByIdAsync(classroomId);
                var user = await _userManager.GetUserAsync(_contextAccessor.HttpContext.User);
                var teacherAssignments = await _unitOfWork.TeacherAssignmentRepository
                    .GetCurrentTeacherAssignmentsForSpecificTeacher(user.Id);

                if (teacherAssignments.All(ta => ta.Classroom.ClassId != classroomId))
                {
                    throw new Exception();
                }

                ViewData["GradeAction"] = "Index";
                ViewData["AbsenceAction"] = "Index";
                ViewData["MessageAction"] = "Add";
                ViewData["MessageText"] = "SendMessage";

                return View(classroom.Pupils);
            }
            catch (Exception)
            {
                var errorViewModel = new ErrorViewModel();
                return View("Error", errorViewModel);
            }

        }

        //Display the pupil's own data
        [Authorize(Roles = "Elev")]
        [HttpGet]
        public async Task<IActionResult> DisplayMyData()
        {
            try
            {
                var pupil = (Pupil)await _userManager.GetUserAsync(_contextAccessor.HttpContext.User);

                ViewData["GradeAction"] = "DisplayGradesForSpecificPupil";
                ViewData["AbsenceAction"] = "DisplayAbsencesForSpecificPupil";

                ViewData["MessageText"] = "Messages";
                ViewData["IsMessagesVisible"] = "invisible";
                return View("DisplaySpecificPupils", new List<Pupil> { pupil });
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
                var pupilViewModel = new PupilViewModel
                {
                    Pupil = new Pupil { StartingDate = DateTime.Now },
                    Title = "Add a new pupil",
                    ButtonText = "AddPupil",
                    ActionName = "Add"
                };

                return View(pupilViewModel);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add(PupilViewModel pupilViewModel)
        {
            try
            {
                var emailAlreadyUsed = _userManager.Users.Any(x => x.Email == pupilViewModel.Pupil.Email);
                if (emailAlreadyUsed)
                {
                    ModelState.AddModelError("Pupil.Email", "Email already used!");
                    return View(pupilViewModel);
                }
                if (!ModelState.IsValid)
                {
                    return View(pupilViewModel);
                }
                if (pupilViewModel.SelectedGender.IsNullOrEmpty())
                {
                    ModelState.AddModelError("SelectedGender", "No gender selected!");
                    return View(pupilViewModel);
                }

                pupilViewModel.Pupil.UserName = pupilViewModel.Pupil.Email;
                pupilViewModel.Pupil.Gender = pupilViewModel.SelectedGender;

                var generatedPassword = PasswordGenerator.GeneratePassword();

                var response = await _emailService.AccountCreationEmail(pupilViewModel.Pupil.Email, generatedPassword);
                if (!response)
                {
                    return View("Error");
                }

                var result = await _userManager.CreateAsync(pupilViewModel.Pupil, generatedPassword);
                if (!result.Succeeded) return View(pupilViewModel);
                await _userManager.AddToRoleAsync(pupilViewModel.Pupil, "Elev");

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var model = new PupilViewModel
                {
                    Pupil = await _unitOfWork.PupilRepository.GetPupilWithClassroomById(id),
                    Title = "Edit",
                    ButtonText = "SaveChanges",
                    ActionName = "Edit"
                };
                return View("Add", model);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(PupilViewModel model)
        {
            try
            {
                var actualPupil = await _unitOfWork.PupilRepository.GetPupilWithClassroomById(model.Pupil.Id);

                var emailAlreadyUsed = _userManager.Users.Where(u => u.Email != actualPupil.Email)
                    .Any(x => x.Email == model.Pupil.Email);

                if (emailAlreadyUsed)
                {
                    ModelState.AddModelError("Pupil.Email", "Email already used!");
                    return View("Add", model);
                }

                if (!ModelState.IsValid)
                {
                    return View("Add", model);
                }

                model.Pupil.Id = -1;

                actualPupil.UserName = model.Pupil.Email;
                actualPupil.NormalizedUserName = model.Pupil.Email.ToUpperInvariant();
                actualPupil.Email = model.Pupil.Email;
                actualPupil.NormalizedEmail = model.Pupil.Email.ToUpperInvariant();
                actualPupil.FirstName = model.Pupil.FirstName;
                actualPupil.LastName = model.Pupil.LastName;
                actualPupil.Address = model.Pupil.Address;
                actualPupil.IsActive = model.Pupil.IsActive;
                actualPupil.StartingDate = model.Pupil.StartingDate;
                actualPupil.EndingDate = model.Pupil.EndingDate;

                if (actualPupil.Gender != model.SelectedGender && !model.SelectedGender.IsNullOrEmpty())
                {
                    actualPupil.Gender = model.SelectedGender;
                }

                //email schimbare mail eventual

                await _unitOfWork.PupilRepository.UpdateAsync(actualPupil);
                await _unitOfWork.CompleteAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var pupil = await _unitOfWork.PupilRepository.GetPupilWithClassroomById(id);
                var model = new PupilViewModel
                {
                    Pupil = pupil,
                    IsVisible = "d-none",
                    Title = "Details"
                };

                ViewData["IsReadonly"] = "readonly";

                return View("Add", model);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var pupil = await _unitOfWork.PupilRepository.GetPupilWithClassroomById(id);

                var deleteUserVm = new DeleteUserViewModel()
                {
                    User = pupil,
                    ActionControllerName = "Pupil"
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
                var pupil = await _unitOfWork.PupilRepository.GetPupilWithClassroomById(model.User.Id);

                pupil.IsActive = false;

                await _userManager.SetTwoFactorEnabledAsync(pupil, true);
                await _userManager.ResetAuthenticatorKeyAsync(pupil);

                await _unitOfWork.PupilRepository.UpdateAsync(pupil);
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
