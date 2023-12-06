using System.ComponentModel.DataAnnotations;

namespace OnlineCatalog.Models
{
    public class SchoolarYear
    {
        public int SchoolarYearId { get; set; }

        [Required]
        public YearStructure YearStructure { get; set; } = default!;

        [Required]
        public YearSubjectsPlan SubjectsPlan { get; set; } = default!;
    }
}
