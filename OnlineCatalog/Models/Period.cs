using System.ComponentModel.DataAnnotations;

namespace OnlineCatalog.Models
{
    public class Period
    {
        public int PeriodId { get; set; }

        [Required(ErrorMessage = "The title cannot be null", AllowEmptyStrings = false)]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "The title must have between 5 and 20 chars")]
        public string Title { get; set; } = string.Empty;

        public DateTime Start { get; set; }
        public DateTime? End { get; set; }

    }
}
