using System.ComponentModel.DataAnnotations;

namespace OnlineCatalog.Models
{
    public class ClassType
    {
        public int ClassTypeId { get; set; }

        [Required(ErrorMessage = "The grade cannot be null", AllowEmptyStrings = false)]
        [StringLength(30, MinimumLength = 4, ErrorMessage = "The type must have between 4 and 30 chars")]
        public string Type { get; set; } =string.Empty;
    }
}
