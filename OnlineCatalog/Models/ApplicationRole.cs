using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace OnlineCatalog.Models
{
    public class ApplicationRole : IdentityRole<int>
    {
        [Key]
        public override int Id { get; set; }
        
        [Required(ErrorMessage = "The title cannot be null.", AllowEmptyStrings = false)]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "The title must have between 3 and 20 chars")]
        public override string Name { get; set; } = string.Empty;

        [ValidateNever]
        public HashSet<JobPeriodRoles> JobPeriodRoles { get; set; } = default!;
    }
}
