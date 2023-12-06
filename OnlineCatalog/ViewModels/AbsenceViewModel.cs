using OnlineCatalog.Models;

namespace OnlineCatalog.ViewModels
{
    public class AbsenceViewModel
    {
        public int PupilId { get; set; }
        public string PupilFullName { get; set; } = string.Empty;
        public string ClassMasterVisibility { get; set; } = string.Empty;
        public List<string> ClassMasterSubjects { get; set; } = new();
        public List<Subject> Subjects { get; set; } = new();
        public List<Absence> Absences { get; set; } = new();
    }
}
