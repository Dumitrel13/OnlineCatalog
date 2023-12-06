using System.ComponentModel.DataAnnotations;

namespace OnlineCatalog.Models
{
    public class FailedSubject
    {
        public int FailedSubjectId { get; set; }
        public DateTime ReexaminationDate { get; set; }
        public float ReexaminationScore { get; set; }

        [Required]
        public Classroom Class { get; set; } = default!;

        [Required]
        public Subject Subject { get; set; } = default!;

        [Required]
        public Pupil Pupil { get; set; } = default!;
    }
}
