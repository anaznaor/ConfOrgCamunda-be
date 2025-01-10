using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;
using WebAPI.Services;
using WebAPI.Util;
using WebAPI.Util.Dto;

namespace WebAPI.Controllers
{
    public class CamundaRegistratingController : Controller
    {
        private readonly ILogger<CamundaReviewingController> _logger;
        private readonly IConferenceService _conferenceService;
        private readonly IRegistrationService _registrationService;
        private readonly IPaperService _paperService;
        private readonly IUserConfService _userConfService;
        private readonly IMapper _mapper;

        public CamundaRegistratingController(HttpClient httpClient, ILogger<CamundaReviewingController> logger, IConferenceService conferenceService, IRegistrationService registrationService, IPaperService paperService, IMapper mapper, IUserConfService userConfService)
        {
            _logger = logger;
            _conferenceService = conferenceService;
            _registrationService = registrationService;
            _paperService = paperService;
            _userConfService = userConfService;
            _mapper = mapper;
            //_httpClient = httpClient;
        }

        [HttpPost("[action]/{guest}")]
        public async Task<IActionResult> StartRegistrationProcess(string guest, [FromBody] int idConference)
        {
            await CamundaRegistratingUtil.StartRegistratingProcess(guest, idConference);
            return Ok();
        }

        [HttpPost("[action]/{guest}")]
        public async Task<IActionResult> ChooseTypeOfRegistration(string guest, [FromBody] bool registerWithPaper)
        {
            await CamundaRegistratingUtil.ChooseTypeOfRegistration(guest, registerWithPaper);
            return Ok();
        }

        [HttpPost("[action]/{guest}")]
        public async Task<IActionResult> InputIdOfReviewedPaper(string guest, [FromBody] int idPaper)
        {
            if(idPaper > 0)
            {
                var paper = _paperService.GetPaper(idPaper);
                if(paper.Decision == PaperDecision.Accepted)
                {
                    await CamundaRegistratingUtil.InputIdOfReviewedPaper(guest, idPaper);
                    return Ok();
                }
            }
            return BadRequest();
        }

        [HttpGet("[action]/{guest}")]
        public async Task<IActionResult> CheckIfRegistrationProcessIsActive(string guest)
        {
            var tasks = await CamundaRegistratingUtil.GetTasks(guest, "GuestRegistrationProcess");
            if(tasks != null && tasks.Count > 0)
            {
                return Ok(true);
            }
            return Ok(false);
        }

        [HttpPost("[action]/{guest}")]
        public async Task<IActionResult> CreateGuestRegistration(string guest, [FromBody] RegistrationGuest registrationGuest)
        {
            var user = _userConfService.GetUser(guest);
            if (user != null)
            {
                user.Fullname = registrationGuest.Fullname;
                user.Sex = registrationGuest.Sex;
                user.Oib = registrationGuest.Oib;
                user.DateOfBirth = registrationGuest.DateOfBirth;
                user.Address = registrationGuest.Address;
                user.City = registrationGuest.City;
                user.Country = registrationGuest.Country;
                user.Title = registrationGuest.Title;
                user.Profession = registrationGuest.Profession;
                user.Company = registrationGuest.Company;
                _userConfService.SaveUser(user);
            }
            var tasks = await CamundaRegistratingUtil.GetTasks(guest, "GuestRegistrationProcess");
            if (tasks.Count > 0)
            {
                var taskId = tasks.First().TID;
                var processId = tasks.First().PID;
                var process = await CamundaRegistratingUtil.GetProcessById(processId, false);
                var conference = _conferenceService.GetConference(process.IdConference);
                var daysTillConferenceTime = (conference.ConferenceDate - DateTime.Now).Days;
                await CamundaRegistratingUtil.CreateGuestRegistration(taskId, guest, (int)process.IdPaper, registrationGuest.Accomodation,
                    daysTillConferenceTime);
            }
            return Ok();
        }

        [HttpPost("[action]/{guest}")]
        public async Task<IActionResult> UploadFeePaymentFile(string guest, [FromForm] PdfFileDto feePayment)
        {
            var user = _userConfService.GetUser(guest);
            var tasks = await CamundaRegistratingUtil.GetTasks(guest, "GuestRegistrationProcess");
            if (tasks.Count > 0)
            {
                var taskId = tasks.First().TID;
                var processId = tasks.First().PID;
                var process = await CamundaRegistratingUtil.GetProcessById(processId, false);
                var conf = _conferenceService.GetConference(process.IdConference);

                Registration registration = null;
                if (process.IdPaper > 0)
                {
                    registration = new Registration
                    {
                        Id = 0,
                        IdConference = process.IdConference,
                        //Conference = _conferenceService.GetConference(process.IdConference),
                        IdUser = user.Id,
                        TimeOfRegistration = DateTime.Today,
                        IdPaper = process.IdPaper
                        //Paper = _paperService.GetPaper((int)process.IdPaper)
                    };
                }
                else
                {
                    registration = new Registration
                    {
                        Id = 0,
                        IdConference = process.IdConference,
                        Conference = _conferenceService.GetConference(process.IdConference),
                        IdUser = user.Id,
                        TimeOfRegistration = DateTime.Today
                    };
                }

                if (process.Accomodation)
                {
                    var room = _registrationService.GetAllAvailableRooms().FirstOrDefault();
                    if (room != null)
                    {
                        room.Reservation = true;
                        registration.IdRoomReservation = room.IdRoom;
                    }
                }

                byte[] abstractBytes = null;
                if (feePayment.file != null && feePayment.file.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await feePayment.file.CopyToAsync(memoryStream);
                        abstractBytes = memoryStream.ToArray();
                    }
                }
                registration.BillPayment = abstractBytes;

                _registrationService.SaveRegistration(registration);

                if(process.IdPaper > 0)
                {
                    var paper = _paperService.GetPaper((int)process.IdPaper);
                    paper.IdRegistration = registration.Id;
                    _paperService.SavePaper(paper);
                }

                await CamundaRegistratingUtil.UploadFeePaymentFile(taskId, registration.Id);
                return Ok(new { IdRegistration = registration.Id });
            }
            return Ok("error");
        }
    }
}
