using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineCatalog.Models;
using OnlineCatalog.Repository.Interfaces;
using OnlineCatalog.ViewModels;

namespace OnlineCatalog.Components
{
    public class DisplayMessage : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public DisplayMessage(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }
        public async Task<IViewComponentResult> InvokeAsync(int personId, bool isRead)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var currentYear = await _unitOfWork.YearStructureRepository.GetCurrentYearStructure();
            var messageViewModel = new MessageViewModel();

            
            if (user != null)
            {
                //diriginte
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("Diriginte"))
                {
                    user = user as Teacher;
                    var messages = await _unitOfWork.MessageRepository
                        .GetMessagesForSpecificYearByTeacherId(user.Id, currentYear.StartingYear,
                            currentYear.EndingYear);

                    messageViewModel.PupilFullName = user.FirstName + " " + user.LastName;
                    messageViewModel.Messages = messages.Where(m => m.IsRead == isRead && m.IsSentByParent == true).ToList();
                }
                else
                {
                    //parinte

                    try
                    {
                        var pupil = await _unitOfWork.PupilRepository.GetPupilWithClassroomByIdForParent(personId,
                            user.Id);
                        var messages = await _unitOfWork.MessageRepository.GetMessagesForSpecificYearByPupilId(personId,
                            currentYear.StartingYear, currentYear.EndingYear);


                        messageViewModel.PupilFullName = pupil.FirstName + " " + pupil.LastName;
                        messageViewModel.Messages =
                            messages.Where(m => m.IsRead == isRead && m.IsSentByParent == false).ToList();
                    }
                    catch (Exception)
                    {
                        var errorViewModel = new ErrorViewModel()
                        {
                            RequestId = HttpContext.Request.HttpContext.TraceIdentifier
                        };
                        return View("~/Views/Shared/Error.cshtml", errorViewModel);
                    }

                }
            }
            return View("../Messages/Display", messageViewModel);
        }
    }
}
