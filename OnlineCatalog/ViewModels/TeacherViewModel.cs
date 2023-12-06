using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineCatalog.Models;

namespace OnlineCatalog.ViewModels
{
    public class TeacherViewModel
	{
		public Teacher Teacher { get; set; } = default!;
        public string Password { get; set; } = string.Empty;
		
        [ValidateNever]
        public JobPeriodRoles JobPeriodRoles { get; set; } = default!;
        
        [ValidateNever]
        public JobPeriodSubjects JobPeriodSubjects { get; set; } = default!;

        [ValidateNever]
        public List<int> SelectedRoles { get; set; } = new();

        [ValidateNever]
        public SelectList Roles { get; set; } = default!;

        [ValidateNever]
        public List<int> SelectedSubjects { get; set; } = new();

        [ValidateNever]
        public SelectList Subjects { get; set; } = default!;

        [ValidateNever]
        public Specialization Specialization { get; set; } = default!;

        [ValidateNever]
        public SelectList Specializations { get; set; } = default!;

        [ValidateNever]
        public List<int> SelectedSpecializations { get; set; } = new();

	}
}
