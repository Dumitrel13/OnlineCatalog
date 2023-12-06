using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace OnlineCatalog.Models
{
    public class Parent : ApplicationUser
    {
        [ValidateNever]
        public HashSet<Pupil> Pupils { get; set; } = default!;
    }
}
