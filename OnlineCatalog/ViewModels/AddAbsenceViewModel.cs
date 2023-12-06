using OnlineCatalog.Models;

namespace OnlineCatalog.ViewModels
{
    public class AddAbsenceViewModel
    {
        public Absence Absence { get; set; } = default!;
        public string FormattedDate { get; set; } = string.Empty;
        public string TeacherFullName { get; set; } = string.Empty;
        public string PupilFullName { get; set; } = string.Empty; 
    }
}
