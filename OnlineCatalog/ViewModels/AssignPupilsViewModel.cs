using OnlineCatalog.Models;

namespace OnlineCatalog.ViewModels
{
    public class AssignPupilsViewModel
    {
        public Classroom Classroom { get; set; } = default!;
        public Parent Parent { get; set; } = default!;
        public List<Pupil> Pupils { get; set; } = new();
        public string PupilsIds { get; set; } = string.Empty;
    }
}
