using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineCatalog.Models;

namespace OnlineCatalog.ViewModels
{
    public class TeacherAssignmentViewModel
    {
        public TeacherAssignment TeacherAssignment { get; set; } = default!;

        public YearStructure YearStructure { get; set; } = default!;

        public string TeachersFullName { get; set; } = string.Empty;

        public string ClassroomName { get; set; } = string.Empty;

        public List<SelectListItem> Roles { get; set; } = new();

        public string SelectedRole { get; set; } = string.Empty;

        public List<SelectListItem> Subjects { get; set; } = new();

        public string SelectedSubject { get; set; } = string.Empty;

        public List<SelectListItem> Classrooms { get; set; } = new();

        public string  SelectedClassroom { get; set; } = string.Empty;
    }
}
