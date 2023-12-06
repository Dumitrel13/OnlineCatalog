using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using OnlineCatalog.Models;

namespace OnlineCatalog.Components
{
    public class AdminMenu : ViewComponent
    {
        private readonly IStringLocalizer<SharedResources> _localizer;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminMenu(IStringLocalizer<SharedResources> localizer, UserManager<ApplicationUser> userManager)
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
                    ControllerName = "Parent",
                    Action = "Index",
                    Name = _localizer["DisplayParents"]
                },
                new()
                {
                    ControllerName = "Pupil",
                    Action = "Index",
                    Name = _localizer["DisplayPupils"]
                },
                new()
                {
                    ControllerName = "Teacher",
                    Action = "Index",
                    Name = _localizer["DisplayTeachers"]
                }
            };

            ViewData["IsHidden"] = "d-none";
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);

                if (roles.Contains("Admin"))
                {
                    ViewData["IsHidden"] = "";
                }
            }

            ViewData["Title"] = _localizer["AdminOptions"];
            return View("../DefaultMenu/Default", options);
        }
    }
}
