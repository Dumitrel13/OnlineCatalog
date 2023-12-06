using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCatalog.Models;
using OnlineCatalog.Repository.Interfaces;

namespace OnlineCatalog.Controllers
{
    [Authorize(Roles = "Director, Admin")]
    public class RoleController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public RoleController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                return View(await _unitOfWork.RoleRepository.GetAllAsync());
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
                    ViewBag.BtnText = "Add role";
                    ViewBag.Title = "Add a new role";
                    return View();
                }
                else
                {
                    var role = await _unitOfWork.RoleRepository.GetRoleByIdAsync(id);
                    ViewBag.BtnText = "Save changes";
                    ViewBag.Title = "Edit role";
                    return View(role);
                }
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddAndEdit(ApplicationRole role)
        {
            try
            {
                if (role.Id != 0)
                {
                    var actualRole = await _unitOfWork.RoleRepository.GetRoleByIdAsync(role.Id);
                    actualRole.Name = role.Name;
                    actualRole.NormalizedName = role.Name.ToUpperInvariant();
                    await _unitOfWork.RoleRepository.UpdateAsync(actualRole);
                }
                else
                {
                    role.NormalizedName = role.Name.ToUpperInvariant();
                    await _unitOfWork.RoleRepository.AddAsync(role);
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
                var role = await _unitOfWork.RoleRepository.GetRoleByIdAsync(id);
                return View(role);
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
                var role = await _unitOfWork.RoleRepository.GetRoleByIdAsync(id);

                await _unitOfWork.RoleRepository.DeleteAsync(role);
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
