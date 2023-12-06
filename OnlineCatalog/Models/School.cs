using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace OnlineCatalog.Models
{
    public class School
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int SchoolId { get; set; }

        [Required(ErrorMessage = "The name cannot be null", AllowEmptyStrings = false)]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "The name must have between 3 and 30 chars")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "The address cannot be null", AllowEmptyStrings = false)]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "The address must have between 3 and 20 chars")]
        public string Address { get; set; } = string.Empty;

        [ValidateNever]
        public HashSet<Teacher> Teachers { get; set; } = default!;

        [ValidateNever]
        public HashSet<Classroom> Classrooms { get; set; } = default!;
    }
}
