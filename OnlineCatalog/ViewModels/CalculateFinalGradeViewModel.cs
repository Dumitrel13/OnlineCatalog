using OnlineCatalog.Models;

namespace OnlineCatalog.ViewModels
{
    public class CalculateFinalGradeViewModel
    {
        public List<Grade> Grades { get; set; } = default!;
        public Grade? ExamGrade { get; set; } = null;
        public string FinalGrade { get; set; } = string.Empty;
        public Subject Subject { get; set; } = default!;
        public int PupilId { get; set; }
        public int TeacherId { get; set; }
        public string PupilFullName { get; set; } = string.Empty;
    }
}
