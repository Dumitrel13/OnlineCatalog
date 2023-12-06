using OnlineCatalog.Models;

namespace OnlineCatalog.ViewModels
{
    public class DisplayAssignmentsViewModel
    {
        public List<TeacherAssignment> TeacherAssignments { get; set; } = new();
        public int TeacherId { get; set; }
        public string TeacherFullName { get; set; } = string.Empty;
    }
}
