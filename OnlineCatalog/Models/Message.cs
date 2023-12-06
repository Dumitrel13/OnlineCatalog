using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace OnlineCatalog.Models
{
    public class Message
    {
        public int MessageId { get; set; }

        [Required(ErrorMessage = "The title cannot be null", AllowEmptyStrings = false)]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "The title must have between 5 and 20 chars")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "ContentErrorMessage", AllowEmptyStrings = false)]
        [StringLength(200, MinimumLength = 10, ErrorMessage = "ContentErrorMessage")]
        public string Content { get; set; } = string.Empty;

        [Required]
        public bool IsRead { get; set; } = false;

        [Required] 
        public bool IsSentByParent { get; set; } = false;

        public string? ImagePath { get; set; } = string.Empty;

        [ValidateNever]
        public DateTime Date { get; set; }

        [ValidateNever]
        public Teacher Teacher { get; set; } = default!;

        [ValidateNever]
        public Pupil Pupil { get; set; } = default!;
    }
}
