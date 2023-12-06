using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using OnlineCatalog.Models;

namespace OnlineCatalog.Components
{
    public class PupilMenu : ViewComponent
    {
        private readonly IStringLocalizer<SharedResources> _localizer;
        private readonly UserManager<ApplicationUser> _userManager;

        public PupilMenu(IStringLocalizer<SharedResources> localizer, UserManager<ApplicationUser> userManager)
        {
            _localizer = localizer;
            _userManager = userManager;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var options = new List<ComponentModel>()
            {
                new()
                {
                    ControllerName = "Pupil",
                    Action = "DisplayMyData",
                    Name = _localizer["DisplayMyScholarInformation"]
                }
            };

            ViewData["IsHidden"] = "d-none";
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("Elev"))
                {
                    ViewData["IsHidden"] = "";
                }
            }

            ViewData["Title"] = _localizer["PupilOptions"];
            return View("../DefaultMenu/Default", options);
        }
    }
}
