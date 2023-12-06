using System.ComponentModel.DataAnnotations;

namespace OnlineCatalog.Models
{
    public class JobPeriodSubjects : JobPeriod
    {
        [Required]
        public HashSet<Subject> Subjects { get; set; } = default!;
    }
}
