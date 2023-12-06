using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OnlineCatalog.Helpers;
using OnlineCatalog.Models;
using OnlineCatalog.Repository.Interfaces;
using OnlineCatalog.ViewModels;
using Microsoft.IdentityModel.Tokens;
using NuGet.Packaging;
using OnlineCatalog.Helpers.Interfaces;

namespace OnlineCatalog.Controllers
{
    [Authorize]
    public class ParentController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IEmailService _emailService;

        public ParentController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IHttpContextAccessor contextAccessor,
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
                var parents = (await _unitOfWork.ParentRepository.GetAllAsync()).Where(p => p.IsActive);

                return View(parents.ToList());
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
                var parentViewModel = new ParentViewModel()
                {
                    Parent = new Parent(),
                    Title = "New parent account",
                    ButtonText = "CreateAnAccount",
                    ActionName = "Add"
                };
                return View(parentViewModel);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add(ParentViewModel parentViewModel)
        {
            try
            {
                var emailAlreadyUsed = _userManager.Users.Any(x => x.Email == parentViewModel.Parent.Email);
                if (emailAlreadyUsed)
                {
                    ModelState.AddModelError("Parent.Email", "Email already used!");
                    return View(parentViewModel);
                }
                if (!ModelState.IsValid)
                {
                    return View(parentViewModel);
                }
                if (parentViewModel.SelectedGender.IsNullOrEmpty())
                {
                    ModelState.AddModelError("SelectedGender", "No gender selected!");
                    return View(parentViewModel);
                }

                parentViewModel.Parent.UserName = parentViewModel.Parent.Email;
                parentViewModel.Parent.Gender = parentViewModel.SelectedGender;

                var generatedPassword = PasswordGenerator.GeneratePassword();

                var sendResult = await _emailService.AccountCreationEmail(parentViewModel.Parent.Email, generatedPassword);
                if (!sendResult)
                {
                    return View("Error");
                }

                var result = await _userManager.CreateAsync(parentViewModel.Parent, generatedPassword);
                if (!result.Succeeded) return View(parentViewModel);
                await _userManager.AddToRoleAsync(parentViewModel.Parent, "Parinte");

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        //[HttpGet("Edit/{parentId}")]
        public async Task<IActionResult> Edit(int parentId)
        {
            try
            {
                var parent = await _unitOfWork.ParentRepository.GetParentById(parentId);

                var parentViewModel = new ParentViewModel()
                {
                    Parent = parent,
                    Title = "Edit",
                    ButtonText = "SaveChanges",
                    ActionName = "Edit"
                };

                return View("Add", parentViewModel);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ParentViewModel parentViewModel)
        {
            try
            {
                var actualParent = await _unitOfWork.ParentRepository.GetParentById(parentViewModel.Parent.Id);

                var emailAlreadyUsed = _userManager.Users.Where(u => u.Email != actualParent.Email)
                    .Any(x => x.Email == parentViewModel.Parent.Email);

                if (emailAlreadyUsed)
                {
                    ModelState.AddModelError("Parent.Email", "Email already used!");
                    return View("Add", parentViewModel);
                }
                if (!ModelState.IsValid)
                {
                    return View("Add", parentViewModel);
                }

                parentViewModel.Parent.Id = -1;

                actualParent.UserName = parentViewModel.Parent.Email;
                actualParent.NormalizedUserName = parentViewModel.Parent.Email.ToUpperInvariant();
                actualParent.Email = parentViewModel.Parent.Email;
                actualParent.NormalizedEmail = parentViewModel.Parent.Email.ToUpperInvariant();
                actualParent.FirstName = parentViewModel.Parent.FirstName;
                actualParent.LastName = parentViewModel.Parent.LastName;
                actualParent.Address = parentViewModel.Parent.Address;
                actualParent.IsActive = parentViewModel.Parent.IsActive;

                if (actualParent.Gender != parentViewModel.SelectedGender && !parentViewModel.SelectedGender.IsNullOrEmpty())
                {
                    actualParent.Gender = parentViewModel.SelectedGender;
                }

                //email schimbare mail eventual

                await _unitOfWork.ParentRepository.UpdateAsync(actualParent);
                await _unitOfWork.CompleteAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        public async Task<IActionResult> Details(int parentId)
        {
            try
            {
                var parent = await _unitOfWork.ParentRepository.GetParentById(parentId);

                var parentViewModel = new ParentViewModel()
                {
                    Parent = parent,
                    IsVisible = "d-none",
                    Title = "Details"
                };

                ViewData["IsReadonly"] = "readonly";

                return View("Add", parentViewModel);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        public async Task<IActionResult> Delete(int parentId)
        {
            try
            {
                var parent = await _unitOfWork.ParentRepository.GetParentById(parentId);

                var deleteUserVm = new DeleteUserViewModel()
                {
                    User = parent,
                    ActionControllerName = "Parent"
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
                var parent = await _unitOfWork.ParentRepository.GetParentById(model.User.Id);

                parent.IsActive = false;

                await _userManager.SetTwoFactorEnabledAsync(parent, true);
                await _userManager.ResetAuthenticatorKeyAsync(parent);

                await _unitOfWork.ParentRepository.UpdateAsync(parent);
                await _unitOfWork.CompleteAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        public async Task<IActionResult> AssignKids(int parentId)
        {
            try
            {
                var parent = await _unitOfWork.ParentRepository.GetParentById(parentId);
                var pupils = (await _unitOfWork.PupilRepository.GetAllAsync()).Where(p => p.IsActive).ToList();

                if (!parent.Pupils.IsNullOrEmpty())
                {
                    pupils = pupils.Where(p => !parent.Pupils.Contains(p)).ToList();
                }

                var assignPupilsViewModel = new AssignPupilsViewModel()
                {
                    Parent = parent,
                    Pupils = pupils,
                };

                return View(assignPupilsViewModel);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AssignKids(AssignPupilsViewModel assignPupilsViewModel)
        {
            try
            {
                var pupilsId = JsonConvert.DeserializeObject<List<int>>(assignPupilsViewModel.PupilsIds);
                var selectedPupils = await _unitOfWork.PupilRepository.GetPupilsByIds(pupilsId);

                assignPupilsViewModel.Pupils = (await _unitOfWork.PupilRepository.GetAllAsync()).Where(p=>p.IsActive).ToList();
                assignPupilsViewModel.Parent =
                    await _unitOfWork.ParentRepository.GetParentById(assignPupilsViewModel.Parent.Id);

                if (!assignPupilsViewModel.Pupils.IsNullOrEmpty())
                {
                    assignPupilsViewModel.Pupils = assignPupilsViewModel.Pupils.Where(p => !assignPupilsViewModel.Parent.Pupils.Contains(p)).ToList();
                }

                ModelState.Clear();
                if (selectedPupils.IsNullOrEmpty())
                {
                    ModelState.AddModelError("Pupils", "No pupils selected!");
                    return View(assignPupilsViewModel);
                }

                assignPupilsViewModel.Parent.Pupils.AddRange(selectedPupils);
                assignPupilsViewModel.Pupils = assignPupilsViewModel.Pupils.Where(p => !assignPupilsViewModel.Parent.Pupils.Contains(p)).ToList();
                await _unitOfWork.CompleteAsync();

                return View(assignPupilsViewModel);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        public async Task<IActionResult> RemoveFromList(int parentId, int pupilId)
        {
            try
            {
                var parent = await _unitOfWork.ParentRepository.GetParentById(parentId);
                var pupil = await _unitOfWork.PupilRepository.GetPupilWithClassroomById(pupilId);

                parent.Pupils.Remove(pupil);

                await _unitOfWork.CompleteAsync();

                return RedirectToAction("AssignKids", new { parentId = parent.Id });
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        [Authorize(Roles = "Parinte")]
        public async Task<IActionResult> DisplayMyKids()
        {
            try
            {
                var parent = (Parent)await _userManager.GetUserAsync(_contextAccessor.HttpContext.User);

                var pupils = await _unitOfWork.PupilRepository.GetPupilsByParent(parent);

                ViewData["GradeAction"] = "DisplayGradesForSpecificPupil";
                ViewData["AbsenceAction"] = "DisplayAbsencesForSpecificPupil";
                ViewData["MessageAction"] = "IndexParent";
                ViewData["MessageText"] = "Messages";
                return View("~/Views/Pupil/DisplaySpecificPupils.cshtml", pupils);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }
    }
}
