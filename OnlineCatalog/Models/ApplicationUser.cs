using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace OnlineCatalog.Models
{
    public class ApplicationUser : IdentityUser<int>
    {
        [Key] public override int Id { get; set; }

        [Required(ErrorMessage = "The first name cannot be null", AllowEmptyStrings = false)]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "The first name must have between 3 and 20 chars")]
        public virtual string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "The last name cannot be null", AllowEmptyStrings = false)]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "The last name must have between 3 and 20 chars")]
        public virtual string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "The address cannot be null", AllowEmptyStrings = false)]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "The address must have between 3 and 20 chars")]
        public virtual string Address { get; set; } = string.Empty;

        [ValidateNever]
        public string Gender { get; set; } = string.Empty;

        [Required(ErrorMessage = "The email cannot be null", AllowEmptyStrings = false)]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "The email must have between 3 and 20 chars")]
        public override string Email { get; set; }

        public bool IsActive { get; set; }
    }
}
