using System.ComponentModel.DataAnnotations;

namespace OnlineCatalog.Models
{
    public class JobPeriodRoles : JobPeriod
    {
        [Required]
        public HashSet<ApplicationRole> Roles { get; set; } = default!;

        [Required] 
        public Teacher Teacher { get; set; } = default!;
    }
}
