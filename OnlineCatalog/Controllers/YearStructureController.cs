using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NuGet.Packaging;
using OnlineCatalog.Models;
using OnlineCatalog.Repository.Interfaces;

namespace OnlineCatalog.Controllers
{
    public class YearStructureController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public YearStructureController(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor,
            IStringLocalizer<SharedResources> localizer)
        {
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
            _localizer = localizer;
        }

        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var yearStructures = await _unitOfWork.YearStructureRepository.GetAllAsync();
                return View(yearStructures.ToList());
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        [Authorize(Roles = "Director")]
        [HttpGet]
        public IActionResult Create()
        {
            try
            {
                var yearStructure = new YearStructure()
                {
                    StartingYear = new DateTime(DateTime.Now.Year, 1, 1),
                    EndingYear = new DateTime(DateTime.Now.Year, 12, 1),
                    Periods = new HashSet<Period>()
                };

                ViewData["ActionName"] = "Create";

                return View(yearStructure);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(YearStructure yearStructure)
        {
            try
            {
                //provides access to the form data submitted in an HTTP POST request
                var periodsJson = _contextAccessor.HttpContext.Request.Form["periods"];
                if (JArray.Parse(periodsJson).Count == 0 || yearStructure.StartingYear == DateTime.MinValue ||
                    yearStructure.EndingYear == DateTime.MinValue)
                {
                    ModelState.AddModelError("Period", _localizer["No values were added!"]);

                    return View(yearStructure);
                }

                var periods = JsonConvert.DeserializeObject<List<Period>>(periodsJson);

                yearStructure.Periods.AddRange(periods);

                await _unitOfWork.YearStructureRepository.AddAsync(yearStructure);
                await _unitOfWork.CompleteAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        [HttpGet("[controller]/Edit/{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var yearStructure = await _unitOfWork.YearStructureRepository.GetYearStructureById(id);

                ViewData["ActionName"] = "Edit";

                return View("Create", yearStructure);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(YearStructure yearStructure)
        {
            try
            {
                var periodsJson = _contextAccessor.HttpContext.Request.Form["periods"];
                if (JArray.Parse(periodsJson).Count == 0 || yearStructure.StartingYear == DateTime.MinValue ||
                    yearStructure.EndingYear == DateTime.MinValue)
                {
                    ModelState.AddModelError("Period", _localizer["No values were added!"]);

                    return View("Create", yearStructure);
                }

                var periods = JsonConvert.DeserializeObject<List<Period>>(periodsJson);

                var actualYearStructure =
                    await _unitOfWork.YearStructureRepository.GetYearStructureById(yearStructure.YearStructureId);

                actualYearStructure.StartingYear = yearStructure.StartingYear;
                actualYearStructure.EndingYear = yearStructure.EndingYear;
                actualYearStructure.AllowExam = yearStructure.AllowExam;

                actualYearStructure.Periods.Clear();
                actualYearStructure.Periods.AddRange(periods);

                await _unitOfWork.YearStructureRepository.UpdateAsync(actualYearStructure);
                await _unitOfWork.CompleteAsync();

                return RedirectToAction(nameof(Index));
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
                var yearStructure = await _unitOfWork.YearStructureRepository.GetYearStructureById(id);

                return View("ConfirmDelete", yearStructure);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirm(YearStructure yearStructure)
        {
            try
            {
                var actualYearStructure =
                    await _unitOfWork.YearStructureRepository.GetYearStructureById(yearStructure.YearStructureId);

                await _unitOfWork.YearStructureRepository.DeleteAsync(actualYearStructure);
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
