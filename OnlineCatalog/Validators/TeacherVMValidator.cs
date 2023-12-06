using Microsoft.IdentityModel.Tokens;
using OnlineCatalog.Models;
using OnlineCatalog.Repository.Interfaces;
using OnlineCatalog.ViewModels;

namespace OnlineCatalog.Validators
{
    public static class TeacherVMValidator
    {
        public static void ValidationForEndingDate(ref TeacherViewModel teacherViewModel)
        {
            if (!teacherViewModel.Teacher.JobPeriodRoles.IsNullOrEmpty() && !teacherViewModel.SelectedRoles.IsNullOrEmpty())
            {
                teacherViewModel.Teacher.JobPeriodRoles.Last().EndingDate = DateTime.Now;
            }

            if (!teacherViewModel.Teacher.JobPeriodSubjects.IsNullOrEmpty() && !teacherViewModel.SelectedSubjects.IsNullOrEmpty())
            {
                teacherViewModel.Teacher.JobPeriodSubjects.Last().EndingDate = DateTime.Now;
            }
        }

        public static void ValidationForSelectedItems(ref TeacherViewModel teacherViewModel, IUnitOfWork unitOfWork)
        {
            if (!teacherViewModel.SelectedRoles.IsNullOrEmpty())
            {
                teacherViewModel.Teacher.JobPeriodRoles.Add(new JobPeriodRoles()
                {
                    StartingDate = teacherViewModel.JobPeriodRoles.StartingDate,
                    Roles = teacherViewModel.SelectedRoles
                        .Select(x => unitOfWork.RoleRepository.GetRoleByIdAsync(x).Result).ToHashSet()
                });
            }
            if (!teacherViewModel.SelectedSubjects.IsNullOrEmpty())
            {
                teacherViewModel.Teacher.JobPeriodSubjects.Add(new JobPeriodSubjects()
                {
                    StartingDate = teacherViewModel.JobPeriodRoles.StartingDate,
                    Subjects = teacherViewModel.SelectedSubjects
                        .Select(x => unitOfWork.SubjectRepository.GetSubjectByIdAsync(x).Result).ToHashSet()
                });
            }

            if (!teacherViewModel.SelectedSpecializations.IsNullOrEmpty())
            {
                teacherViewModel.Teacher.Specializations = teacherViewModel.SelectedSpecializations.Select(x =>
                    unitOfWork.SpecializationRepository.GetSpecializationByIdAsync(x).Result).ToList(); ;
            }
        }
    }
}
