using System.ComponentModel.DataAnnotations;

namespace OnlineCatalog.Models
{
    public class Absence
    {
        public int AbsenceId { get; set; }

        [Required(ErrorMessage="The date cannot be null")]
        public DateTime Date { get; set; }

        public TimeSpan StartingHour { get; set; }

        public TimeSpan EndingHour { get; set; }

        public bool IsJustified { get; set; } = false;

        [Required]
        public Teacher Teacher { get; set; } = default!;

        [Required]
        public Pupil Pupil { get; set; } = default!;

        [Required]
        public Subject Subject { get; set; } = default!;

        
    }
}
