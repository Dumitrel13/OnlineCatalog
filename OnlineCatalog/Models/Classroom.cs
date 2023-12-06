using System.ComponentModel.DataAnnotations;

namespace OnlineCatalog.Models
{
    public class Classroom
    {
        [Key]
        public int ClassId { get; set; }
        public int Grade { get; set; }//5, 6, 7

        [Required(ErrorMessage = "The grade cannot be null", AllowEmptyStrings = false)]
        [StringLength(2, MinimumLength = 1, ErrorMessage = "The grade must have between 1 and 2 chars")]
        public string Group { get; set; } = string.Empty;//a, b, c

        //[Required]
        //public ClassType ClassroomType { get; set; } = default!;

        [Required]
        public School School { get; set; } = default!;

        [Required]
        public List<Pupil> Pupils { get; set; } = default!;

        [Required] 
        public List<Subject> Subjects { get; set; } = default!;

        [Required] 
        public List<TeacherAssignment> TeacherAssignments { get; set; } = default!;

    }
}
