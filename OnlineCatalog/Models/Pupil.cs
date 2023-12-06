using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Newtonsoft.Json;

namespace OnlineCatalog.Models
{
    public class Pupil : ApplicationUser
    {
        [Required(ErrorMessage = ("The starting date cannot be null"))]
        public DateTime StartingDate { get; set; }
        public DateTime? EndingDate { get; set; }

        [ValidateNever]
        public HashSet<Parent> Parents { get; set; } = default!;

        [ValidateNever][JsonIgnore] public Classroom? Classroom { get; set; }
    }
}
