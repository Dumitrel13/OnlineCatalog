using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace OnlineCatalog.Models
{
    public class Teacher : ApplicationUser
    {

        public ICollection<JobPeriodRoles>? JobPeriodRoles { get; set; } = new List<JobPeriodRoles>();

        public ICollection<JobPeriodSubjects>? JobPeriodSubjects { get; set; } = new List<JobPeriodSubjects>();

        public ICollection<Specialization>? Specializations { get; set; } = new List<Specialization>();

        [ValidateNever]
        public School? School { get; set; } = default!;

    }
}
