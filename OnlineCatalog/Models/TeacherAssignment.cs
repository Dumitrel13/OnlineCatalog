using System.ComponentModel.DataAnnotations;

namespace OnlineCatalog.Models
{
    public class TeacherAssignment
    {
        public int TeacherAssignmentId { get; set; }
        public int SubjectHours { get; set; }
        public DateTime StartingDate { get; set; }
        public DateTime EndingDate { get; set; }

        [Required]
        public Teacher Teacher { get; set; } = default!;

        [Required]
        public ApplicationRole ApplicationRole { get; set; } = default!;

        [Required] public Subject? Subject { get; set; }

        [Required] public Classroom? Classroom { get; set; }
    }
}
