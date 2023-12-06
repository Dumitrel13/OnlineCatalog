using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineCatalog.Models;
using OnlineCatalog.Repository.Interfaces;
using OnlineCatalog.ViewModels;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;

namespace OnlineCatalog.Controllers
{

    public class MessageController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public MessageController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IHttpContextAccessor contextAccessor,
            IStringLocalizer<SharedResources> localizer)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _contextAccessor = contextAccessor;
            _localizer = localizer;
        }

        [Authorize(Roles = "Parinte")]
        public async Task<IActionResult> IndexParent(int pupilId)
        {
            try
            {
                ViewData["IsFailedMsgVisible"] = "invisible";
                ViewData["IsCreateMessageVisible"] = "visible";
                var tuple = new Tuple<int, bool>(pupilId, true);
                return View("Index", tuple);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        [Authorize(Roles = "Diriginte")]
        public async Task<IActionResult> IndexClassMaster()
        {
            try
            {
                ViewData["IsCreateMessageVisible"] = "invisible";
                ViewData["IsFailedMsgVisible"] = "invisible";

                var user = await _userManager.GetUserAsync(_contextAccessor.HttpContext.User) as Teacher;
                var tuple = new Tuple<int, bool>(user.Id, false);
                return View("Index", tuple);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        [Authorize(Roles = "Parinte, Elev, Diriginte")]
        [HttpGet]
        public async Task<IActionResult> Details(int messageId)
        {
            try
            {
                var message = await _unitOfWork.MessageRepository.GetMessageById(messageId);

                var addMessageViewModel = new AddMessageViewModel()
                {
                    Message = message,
                    FormattedDate = message.Date.Day + " " +
                                    DateTimeFormatInfo.CurrentInfo.GetMonthName(message.Date.Month) + " " +
                                    message.Date.Year,
                    PupilFullName = message.Pupil.FirstName + " " + message.Pupil.LastName,
                    TeacherFullName = message.Teacher.FirstName + " " + message.Teacher.LastName
                };

                message.IsRead = true;
                await _unitOfWork.MessageRepository.UpdateAsync(message);
                await _unitOfWork.CompleteAsync();


                if (message.ImagePath.IsNullOrEmpty())
                {
                    ViewData["IsBtnVisible"] = "invisible";
                }
                else
                {
                    ViewData["IsBtnVisible"] = "visible";
                }

                return ViewComponent("MessageDetails", addMessageViewModel);
            }
            catch (Exception)
            {
                var errorViewModel = new ErrorViewModel();
                return View("Error", errorViewModel);
            }
        }

        [Authorize(Roles = "Profesor")]
        public async Task<IActionResult> Add(int pupilId)
        {
            try
            {
                var pupil = await _unitOfWork.PupilRepository.GetPupilWithClassroomById(pupilId);
                var user = (Teacher)await _userManager.GetUserAsync(_contextAccessor.HttpContext.User);

                //Check if the classroom of the pupil is found in the user assignments
                var teacherAssignments =
                    await _unitOfWork.TeacherAssignmentRepository
                        .GetCurrentTeacherAssignmentsWithDataForSpecificTeacherAndClass(user.Id,
                            pupil.Classroom.ClassId);

                if (teacherAssignments.IsNullOrEmpty())
                {
                    throw new Exception();
                }

                var addMessageViewModel = new AddMessageViewModel()
                {
                    Message = new Message()
                    {
                        Date = DateTime.Now.Date,
                        Pupil = pupil,
                        Teacher = user
                    },
                    FormattedDate = DateTime.Now.Day + " " +
                                    DateTimeFormatInfo.CurrentInfo.GetMonthName(DateTime.Now.Month) + " " +
                                    DateTime.Now.Year,
                    PupilFullName = pupil.FirstName + " " + pupil.LastName,
                    TeacherFullName = user.FirstName + " " + user.LastName
                };

                return View(addMessageViewModel);
            }
            catch (Exception)
            {
                var errorViewModel = new ErrorViewModel();
                return View("Error", errorViewModel);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddMessageViewModel addMessageViewModel)
        {
            try
            {
                var pupil = await _unitOfWork.PupilRepository.GetPupilWithClassroomById(addMessageViewModel.Message.Pupil.Id);
                var teacher = await _unitOfWork.TeacherRepository.GetTeacherByIdAsync(addMessageViewModel.Message.Teacher.Id);

                addMessageViewModel.Message.Pupil = pupil;
                addMessageViewModel.Message.Teacher = teacher;

                if (!ModelState.IsValid)
                {
                    return View(addMessageViewModel);
                }

                await _unitOfWork.MessageRepository.AddAsync(addMessageViewModel.Message);
                await _unitOfWork.CompleteAsync();

                return RedirectToAction("Index", "Home");
            }
            catch (Exception)
            {
                var errorViewModel = new ErrorViewModel();
                return View("Error", errorViewModel);
            }

        }

        [HttpPost]
        public async Task<IActionResult> Send(AddMessageViewModel addMessageViewModel)
        {
            try
            {
                var pupil = await _unitOfWork.PupilRepository.GetPupilWithClassroomById(addMessageViewModel.Message.Pupil.Id);
                var teacher = await _unitOfWork.TeacherRepository.GetTeacherByIdAsync(addMessageViewModel.Message.Teacher.Id);
                var tuple = new Tuple<int, bool>(pupil.Id, true);

                addMessageViewModel.Message.Pupil = pupil;
                addMessageViewModel.Message.Teacher = teacher;

                if (!ModelState.IsValid)
                {
                    ViewData["IsFailedMsgVisible"] = "visible";
                    ViewData["TopMessage"] = _localizer["Failed to send the message!"];

                    return View("Index", tuple);
                }

                var key = Guid.NewGuid() + "-" + addMessageViewModel.SelectedFile.FileName;
                addMessageViewModel.Message.ImagePath = key;
                addMessageViewModel.Message.IsSentByParent = true;

                var response = await _unitOfWork.ManageFilesRepository.UploadFileAsync(addMessageViewModel.SelectedFile,
                    "onlinecatalogstorage", key);

                if (!response)
                {
                    ViewData["IsFailedMsgVisible"] = "visible";
                    ViewData["TopMessage"] = _localizer["Failed to send the message!"];
                    return View("Index", tuple);
                }

                await _unitOfWork.MessageRepository.AddAsync(addMessageViewModel.Message);
                await _unitOfWork.CompleteAsync();

                ViewData["IsFailedMsgVisible"] = "visible";
                ViewData["TopMessage"] = _localizer["Message send!"];
                return View("Index", tuple);
            }
            catch (Exception)
            {
                var errorViewModel = new ErrorViewModel();
                return View("Error", errorViewModel);
            }
        }

        public async Task<IActionResult> DownloadFile(string bucketName, string key)
        {
            try
            {
                if (key.IsNullOrEmpty())
                {
                    var errorViewModel = new ErrorViewModel();
                    return View("Error", errorViewModel);
                }
                var response = await _unitOfWork.ManageFilesRepository.GetFileByKeyAsync(bucketName, key);

                return File(response.ResponseStream, response.Headers.ContentType, key);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }
    }
}
