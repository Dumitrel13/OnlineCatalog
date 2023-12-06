using System.ComponentModel.DataAnnotations;

namespace OnlineCatalog.Models
{
    public class Grade
    {
        public int GradeId { get; set; }

        [Required(ErrorMessage = "The score cannot be null")]
        public string Score { get; set; } = string.Empty;

        [Required(ErrorMessage = "The date cannot be null")]
        public DateTime Date { get; set; }

        [Required] 
        public GradeType GradeType { get; set; } = default!;

        [Required]
        public Subject Subject { get; set; } = default!;

        [Required]
        public Teacher Teacher { get; set; } = default!;

        [Required]
        public Pupil Pupil { get; set; } = default!;
    }
}
