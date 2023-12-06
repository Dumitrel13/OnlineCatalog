using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using OnlineCatalog.Models;

namespace OnlineCatalog.ViewModels
{
    public class AddMessageViewModel
    {
        public Message Message { get; set; } = default!;
        public string TeacherFullName { get; set; } = string.Empty;
        public string PupilFullName { get; set; } = string.Empty;
        public string FormattedDate { get; set; } = string.Empty;

        [ValidateNever]
        public IFormFile SelectedFile { get; set; } = default!;
    }
}
