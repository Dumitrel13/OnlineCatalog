using System.ComponentModel.DataAnnotations;

namespace OnlineCatalog.Models
{
    public class Specialization
    {
        public int SpecializationId { get; set; }

        [Required(ErrorMessage = "The name cannot be null", AllowEmptyStrings = false)]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "The name must have between 3 and 30 chars")]
        public string Name { get; set; } = string.Empty;

        [Required]
        public HashSet<Teacher> Teachers { get; set; } = default!;
    }
}
