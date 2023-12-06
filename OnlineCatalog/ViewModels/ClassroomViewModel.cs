using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineCatalog.Models;

namespace OnlineCatalog.ViewModels
{
    public class ClassroomViewModel
    {
        public string SelectedGrade { get; set; } = string.Empty;
        public List<SelectListItem> Grades { get; set; } = new()
        {
            new SelectListItem(){Value = "1", Text = "1"},
            new SelectListItem(){Value = "2", Text = "2"},
            new SelectListItem(){Value = "3", Text = "3"},
            new SelectListItem(){Value = "4", Text = "4"},
            new SelectListItem(){Value = "5", Text = "5"},
            new SelectListItem(){Value = "6", Text = "6"},
            new SelectListItem(){Value = "7", Text = "7"},
            new SelectListItem(){Value = "8", Text = "8"},
        };

        public string SelectedGroup { get; set; } = string.Empty;
        public List<SelectListItem> Groups { get; set; } = new()
        {
            new SelectListItem(){Value = "1", Text = "A"},
            new SelectListItem(){Value = "2", Text = "B"},
            new SelectListItem(){Value = "3", Text = "C"},
        };

        [ValidateNever]
        public Classroom Classroom { get; set; } = new();

        [ValidateNever] 
        public string ActionName { get; set; } = string.Empty;
    }
}
