using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using OnlineCatalog.Models;

namespace OnlineCatalog.Components
{
    public class PrincipalMenu : ViewComponent
    {
        private readonly IStringLocalizer<SharedResources> _localizer;
        private readonly UserManager<ApplicationUser> _userManager;

        public PrincipalMenu(IStringLocalizer<SharedResources> localizer, UserManager<ApplicationUser> userManager)
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
                    ControllerName = "YearStructure",
                    Action = "Index",
                    Name = _localizer["DisplayYearStructures"]
                },
                new()
                {
                    ControllerName = "Teacher",
                    Action = "Index",
                    Name = _localizer["DisplayTeachers"]
                },
                new()
                {
                    ControllerName = "Classroom",
                    Action = "Index",
                    Name = _localizer["DisplayClassrooms"]
                },
                new()
                {
                    ControllerName = "Subject",
                    Action = "Index",
                    Name = _localizer["DisplaySubjects"]
                },
                new()
                {
                    ControllerName = "School",
                    Action = "Index",
                    Name = _localizer["DisplaySchoolDetails"]
                },
                new()
                {
                    ControllerName = "Classroom",
                    Action = "PromoteAllClassroomsDisplay",
                    Name = _localizer["PromoteClassrooms"]
                }
            };

            ViewData["IsHidden"] = "d-none";
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);

                if (roles.Contains("Director"))
                {
                    ViewData["IsHidden"] = "";
                }
            }

            ViewData["Title"] = _localizer["PrincipalOptions"];
            return View("../DefaultMenu/Default", options);
        }
    }
}
