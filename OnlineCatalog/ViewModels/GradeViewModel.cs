using OnlineCatalog.Models;

namespace OnlineCatalog.ViewModels
{
    public class GradeViewModel
    {
        public List<Grade> Grades { get; set; } = default!;
        public int PupilId { get; set; }
        public string PupilFullName { get; set; } = string.Empty;
        public List<string> AvailableSubjects { get; set; } = new();
        public int TeacherId { get; set; }
    }
}
