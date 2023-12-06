using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineCatalog.Models;

namespace OnlineCatalog.ViewModels
{
    public class PupilViewModel
    {
        public string SelectedClassroom { get; set; } = string.Empty;
        public List<SelectListItem> Classrooms { get; set; } = new();
        public Pupil? Pupil{ get; set; } = default!;
        public string Title { get; set; } = string.Empty;
        public string ButtonText { get; set; } = string.Empty;
        public string ActionName { get; set; } = string.Empty;
        public string IsVisible { get; set; } = string.Empty;

        [ValidateNever]
        public string SelectedGender { get; set; } = string.Empty;
        public List<SelectListItem> GenderList { get; set; } = new()
        {
            new SelectListItem() { Value = "M", Text = "M" },
            new SelectListItem() { Value = "F", Text = "F" },
            new SelectListItem() { Value = "Other", Text = "Other" },
        };
    }
}
