using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using OnlineCatalog.Models;

namespace OnlineCatalog.Components
{
    public class TeacherMenu : ViewComponent
    {
        private readonly IStringLocalizer<SharedResources> _localizer;
        private readonly UserManager<ApplicationUser> _userManager;
        public TeacherMenu(IStringLocalizer<SharedResources> localizer, UserManager<ApplicationUser> userManager)
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
                    ControllerName = "Classroom",
                    Action = "MyClassrooms",
                    Name = _localizer["DisplayClassrooms"]
                }
            };

            ViewData["IsHidden"] = "d-none";
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);

                if (roles.Contains("Diriginte"))
                {
                    options.Add(new ComponentModel
                    {
                        ControllerName = "Message",
                        Action = "IndexClassMaster",
                        Name = _localizer["Messages"]
                    });
                }

                if (roles.Contains("Diriginge") || roles.Contains("Profesor"))
                {
                    ViewData["IsHidden"] = "";
                }
            }
            
            
            ViewData["Title"] = _localizer["TeacherOptions"];
            return View("../DefaultMenu/Default", options);
        }
    }
}
