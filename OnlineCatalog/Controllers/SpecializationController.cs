using Microsoft.AspNetCore.Mvc;
using OnlineCatalog.Models;
using OnlineCatalog.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace OnlineCatalog.Controllers
{
    [Authorize]
    public class SpecializationController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public SpecializationController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                return View(await _unitOfWork.SpecializationRepository.GetAllAsync());
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        public async Task<IActionResult> AddAndEdit(int id)
        {
            try
            {
                if (id == 0)
                {
                    ViewBag.BtnText = "Add specialization";
                    ViewBag.Title = "Add a new specialization";
                    return View();
                }
                else
                {
                    var specialization = await _unitOfWork.SpecializationRepository.GetSpecializationByIdAsync(id);
                    ViewBag.BtnText = "Save changes";
                    ViewBag.Title = "Edit specialization";
                    return View(specialization);
                }
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddAndEdit(Specialization specialization)
        {
            try
            {

                if (specialization.SpecializationId != 0)
                {
                    await _unitOfWork.SpecializationRepository.UpdateAsync(specialization);
                }
                else
                {
                    await _unitOfWork.SpecializationRepository.AddAsync(specialization);
                }
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
                var specialization = await _unitOfWork.SpecializationRepository.GetSpecializationByIdAsync(id);
                return View(specialization);
            }
            catch (Exception)
            {
                var errorViewModel = new ErrorViewModel();
                return View("Error", errorViewModel);
            }
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            try
            {
                var specialization = await _unitOfWork.SpecializationRepository.GetSpecializationByIdAsync(id);

                await _unitOfWork.SpecializationRepository.DeleteAsync(specialization);
                await _unitOfWork.CompleteAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                var errorViewModel = new ErrorViewModel();
                return View("Error", errorViewModel);
            }
        }
    }
}
