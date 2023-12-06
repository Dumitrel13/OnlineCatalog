using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCatalog.Models;
using OnlineCatalog.Repository.Interfaces;

namespace OnlineCatalog.Controllers
{
    [Authorize]
    public class SubjectController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public SubjectController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(Subject subject)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(subject);
                }

                await _unitOfWork.SubjectRepository.AddAsync(subject);
                await _unitOfWork.CompleteAsync();
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
                var currentSubject = await _unitOfWork.SubjectRepository.GetSubjectByIdAsync(id);
                return View(currentSubject);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Subject subject)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(subject);
                }

                await _unitOfWork.SubjectRepository.UpdateAsync(subject);
                await _unitOfWork.CompleteAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                return View(await _unitOfWork.SubjectRepository.GetAllAsync());
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
                var subject = await _unitOfWork.SubjectRepository.GetSubjectByIdAsync(id);
                return View(subject);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            try
            {
                var subject = await _unitOfWork.SubjectRepository.GetSubjectByIdAsync(id);

                await _unitOfWork.SubjectRepository.DeleteAsync(subject);
                await _unitOfWork.CompleteAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception) { return View("Error"); }
        }
    }
}
