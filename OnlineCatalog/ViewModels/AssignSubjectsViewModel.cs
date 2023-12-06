using OnlineCatalog.Models;

namespace OnlineCatalog.ViewModels
{
    public class AssignSubjectsViewModel
    {
        public Classroom Classroom { get; set; } = default!;

        public string SubjectsId { get; set; } = string.Empty;

        public List<Subject> Subjects { get; set; } = default!;

        public string IsReadOnly { get; set; } = string.Empty;
    }
}
