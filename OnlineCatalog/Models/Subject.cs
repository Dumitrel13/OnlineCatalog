using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace OnlineCatalog.Models
{
    public class Subject
    {
        [Key]
        public int SubjectId { get; set; }

        [Required(ErrorMessage = "The name cannot be null")]
        [Display(Name = "Subject Name")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "The name must have between 3 and 20 characters")]
        public string Name { get; set; } = string.Empty;

        [Required]
        public HashSet<JobPeriodSubjects> JobPeriodSubjects { get; set; } = new();

        [ValidateNever]
        public List<Classroom> Classrooms { get; set; } = default!;
    }
}
