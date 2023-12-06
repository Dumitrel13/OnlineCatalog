using System.ComponentModel.DataAnnotations;

namespace OnlineCatalog.Models
{
    public class YearStructure
    {
        public int YearStructureId { get; set; }

        public DateTime StartingYear { get; set; }
        public DateTime EndingYear { get; set; }
        public bool AllowExam { get; set; }

        [Required]
        public HashSet<Period> Periods { get; set; } = default!;
    }
}
