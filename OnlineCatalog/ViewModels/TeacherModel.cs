using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineCatalog.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;


namespace OnlineCatalog.ViewModels
{
    public class TeacherModel
    {
        public Teacher Teacher { get; set; } = default!;

        [ValidateNever]
        public bool IsPrincipal { get; set; } = false;
        [ValidateNever]
        public string SelectedGender { get; set; } = string.Empty;
        public string IsVisible { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string ButtonText { get; set; } = string.Empty;
        public string ActionName { get; set; } = string.Empty;
        
        public List<SelectListItem> GenderList { get; set; } = new()
        {
            new SelectListItem() { Value = "M", Text = "M" },
            new SelectListItem() { Value = "F", Text = "F" },
            new SelectListItem() { Value = "Other", Text = "Other" },
        };
    }
}
