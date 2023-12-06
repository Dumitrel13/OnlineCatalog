using System.ComponentModel.DataAnnotations;

namespace OnlineCatalog.Models
{
    public class YearSubjectsPlan
    {
        public int YearSubjectsPlanId { get; set; }

        [Required]
        public YearStructure YearStructure { get; set; } = default!;

        [Required]
        public HashSet<Classroom> Classes { get; set; } = default!;

        [Required]
        public HashSet<TeacherAssignment> TeacherAssignments { get; set; } = default!;
    }
}
