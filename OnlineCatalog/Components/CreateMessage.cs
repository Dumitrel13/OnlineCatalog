using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineCatalog.Models;
using OnlineCatalog.Repository.Interfaces;
using OnlineCatalog.ViewModels;

namespace OnlineCatalog.Components
{
    public class CreateMessage : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public CreateMessage(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(int pupilId)
        {
            try
            {


                var user = await _userManager.GetUserAsync(HttpContext.User);
                Pupil pupil = null;

                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles.Contains("Parinte"))
                    {
                        pupil = await _unitOfWork.PupilRepository.GetPupilWithClassroomByIdForParent(pupilId, user.Id);
                    }
                }

                if (pupil == null)
                {
                    throw new Exception();
                }


                //get all teacher assignments from the pupil's classroom
                var classroomAssignments = await _unitOfWork.TeacherAssignmentRepository
                    .GetTeacherAssignmentsForSpecificClass(pupil.Classroom.ClassId);
                var teachers = classroomAssignments.Select(ca => ca.Teacher).ToList();

                var classMasterRole = await _unitOfWork.RoleRepository.GetRoleByNameAsync("Diriginte");

                Teacher classMaster = new();

                foreach (var teacher in teachers.Where(teacher => teacher.JobPeriodRoles
                             .Any(x => x.Roles.Contains(classMasterRole))))
                {
                    classMaster = teacher;
                    break;
                }

                var addMessageViewModel = new AddMessageViewModel()
                {
                    Message = new Message()
                    {
                        Date = DateTime.Now.Date,
                        Pupil = pupil,
                        Teacher = classMaster
                    },
                    FormattedDate = DateTime.Now.Day + " " +
                                    DateTimeFormatInfo.CurrentInfo.GetMonthName(DateTime.Now.Month) + " " +
                                    DateTime.Now.Year,
                    PupilFullName = pupil.FirstName + " " + pupil.LastName,
                    TeacherFullName = classMaster.FirstName + " " + classMaster.LastName
                };

                return View("../Messages/Create", addMessageViewModel);
            }
            catch (Exception)
            {
                var errorViewModel = new ErrorViewModel()
                {
                    RequestId = HttpContext.Request.HttpContext.TraceIdentifier
                };
                return View("~/Views/Shared/Error.cshtml", errorViewModel);
            }
        }
    }
}
