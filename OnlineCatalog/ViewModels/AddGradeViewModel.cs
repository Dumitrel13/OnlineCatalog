using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineCatalog.Models;

namespace OnlineCatalog.ViewModels
{
    public class AddGradeViewModel
    {
        [BindProperty]
        public Grade Grade { get; set; } = default!;
        public string FormattedDate { get; set; } = string.Empty;
        public string TeacherFullName { get; set; } = string.Empty;
        public string PupilFullName { get; set; } = string.Empty;
        public string SelectedType { get; set; } = string.Empty;
        public List<SelectListItem> Types { get; set; } = default!;
        public string SelectedScore { get; set; } = string.Empty;

        public List<SelectListItem> NumberScores { get; set; } = new()
        {
            new SelectListItem() { Value = "1", Text = "1" },
            new SelectListItem() { Value = "2", Text = "2" },
            new SelectListItem() { Value = "3", Text = "3" },
            new SelectListItem() { Value = "4", Text = "4" },
            new SelectListItem() { Value = "5", Text = "5" },
            new SelectListItem() { Value = "6", Text = "6" },
            new SelectListItem() { Value = "7", Text = "7" },
            new SelectListItem() { Value = "8", Text = "8" },
            new SelectListItem() { Value = "9", Text = "9" },
            new SelectListItem() { Value = "10", Text = "10" },
        };

        public List<SelectListItem> CharacterScores { get; set; } = new()
        {
            new SelectListItem() { Value = "Admis", Text = "Admis" },
            new SelectListItem() { Value = "Respins", Text = "Respins" },
        };
    }
}
