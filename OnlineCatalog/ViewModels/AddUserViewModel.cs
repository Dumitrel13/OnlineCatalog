using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineCatalog.Models;

namespace OnlineCatalog.ViewModels
{
    public class AddUserViewModel
    {
        public ApplicationUser User { get; set; } = default!;
        public string SelectedGender { get; set; } = string.Empty;
        public string IsVisible { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string ButtonText { get; set; } = string.Empty;
        public string ActionName { get; set; } = string.Empty;
        public string ControllerName { get; set; } = string.Empty;

        public List<SelectListItem> GenderList { get; set; } = new()
        {
            new SelectListItem() { Value = "M", Text = "M" },
            new SelectListItem() { Value = "F", Text = "F" },
            new SelectListItem() { Value = "Other", Text = "Other" },
        };
    }
}
