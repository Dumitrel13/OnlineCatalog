using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineCatalog.Models;
using OnlineCatalog.Repository.Interfaces;
using OnlineCatalog.ViewModels;

namespace OnlineCatalog.Controllers
{
    public class SchoolController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;

        public SchoolController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _contextAccessor = contextAccessor;
        }

        [HttpGet("[controller]/Index")]
        [Authorize(Roles = "Director")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var user = await _userManager.GetUserAsync(_contextAccessor.HttpContext.User);

                var principal = await _unitOfWork.TeacherRepository.GetTeacherWithSchoolById(user.Id);

                var model = new SchoolViewModel();
                if (principal.School == null)
                {
                    model.Message = "No school registered yet!";
                }
                else
                {
                    model.School = principal.School;
                }

                return View(model);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        [HttpGet("[controller]/Add")]
        [Authorize(Roles = "Director")]
        public async Task<IActionResult> Add()
        {
            try
            {
                var user = await _userManager.GetUserAsync(_contextAccessor.HttpContext.User);
                var principal = await _unitOfWork.TeacherRepository.GetTeacherWithSchoolById(user.Id);

                if (principal.School != null)
                {
                    var errorViewModel = new ErrorViewModel()
                    {
                    };
                    return View("Error", errorViewModel);
                }

                var school = new School();

                ViewData["ActionName"] = "Add";

                return View(school);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add(School school)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewData["ActionName"] = "Add";

                    return View(school);
                }

                var user = await _userManager.GetUserAsync(_contextAccessor.HttpContext.User);

                var principal = await _unitOfWork.TeacherRepository.GetTeacherWithSchoolById(user.Id);

                principal.School = school;

                await _unitOfWork.TeacherRepository.UpdateAsync(principal);
                await _unitOfWork.SchoolRepository.AddAsync(school);
                await _unitOfWork.CompleteAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        [HttpGet("[controller]/Edit/{id:int}")]
        [Authorize(Roles = "Director")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var school = await _unitOfWork.SchoolRepository.GetSchoolById(id);

                ViewData["ActionName"] = "Edit";

                return View("Add", school);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(School school)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewData["ActionName"] = "Edit";

                    return View("Add", school);
                }

                await _unitOfWork.SchoolRepository.UpdateAsync(school);
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
