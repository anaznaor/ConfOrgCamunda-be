using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Interface;
using WebAPI.Models;
using WebAPI.Services;
using WebAPI.Util;
using WebAPI.Util.Dto;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]/")]
    public class CamundaConfOrgController : Controller
    {
        private readonly ILogger<CamundaConfOrgController> _logger;
        private readonly IConferenceService _conferenceService;
        private readonly IRegistrationService _registrationService;
        private readonly IPaperService _paperService;
        private readonly IUserConfService _userConfService;
        private readonly IMapper _mapper;

        public CamundaConfOrgController(HttpClient httpClient, ILogger<CamundaConfOrgController> logger, IConferenceService conferenceService, IRegistrationService registrationService, IPaperService paperService, IMapper mapper, IUserConfService userConfService)
        {
            _logger = logger;
            _conferenceService = conferenceService;
            _registrationService = registrationService;
            _paperService = paperService;
            _userConfService = userConfService;
            _mapper = mapper;
            //_httpClient = httpClient;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> StartConfOrg()
        {
            var processInstanceId = await CamundaConfOrgUtil.StartConfOrgProcess();
            await CamundaConfOrgUtil.GetTask(processInstanceId);
            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> CreateConference([FromBody] ConferenceDto conference)
        {
            Conference conferenceEntity = new Conference
            {
                Id = 0,
                Title = conference.Title,
                Theme = conference.Theme,
                City = conference.City,
                Country = conference.Country,
                Hotel = conference.Hotel,
                ConferenceDate = conference.ConferenceDate,
                PaperSubmissionStart = conference.PaperSubmissionStart,
                PaperSubmissionEnd = conference.PaperSubmissionEnd,
                LastDateOfRegistration = conference.LastDateOfRegistration,
                GuestLecturers = new List<GuestLecturer>(),
                ProgramCommittee = new List<ProgramCommittee>(),
                Registrations = new List<Registration>()
            };
            _conferenceService.SaveConference(conferenceEntity);
            await CamundaConfOrgUtil.CreateConference(conferenceEntity.Id);
            return Ok(new {IdConference = conferenceEntity.Id});
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> SelectionProgComm([FromBody] ListOfEmails progComms)
        {
            await CamundaConfOrgUtil.SelectionProgComm(progComms.Emails);
            await CamundaConfOrgUtil.SendInviteForProgramCommittee(progComms.Emails);
            return Ok();
        }

        [HttpGet("[action]/{user}")]
        public async Task<IActionResult> GetInvite(string user)
        {
            var invitationTasks = await CamundaConfOrgUtil.GetTasks(user);
            if(invitationTasks != null && invitationTasks.Count > 0)
            {
                var process = await CamundaConfOrgUtil.GetProcessById(invitationTasks.First().PID);
                var conference = _conferenceService.GetConference(process.IdConference);
                return Ok(conference);
            }
            return Ok(null);
        }

        [HttpPost("[action]/{idConference}")]
        public async Task<IActionResult> SendAnswerProgComm(int idConference, [FromBody] Answer answer) //multi-instance
        {
            if (answer.answer)
            {
                var conference = _conferenceService.GetConference(idConference);
                var user = _userConfService.GetUser(answer.user);
                if (user != null)
                {
                    conference.ProgramCommittee.Add(new ProgramCommittee { Id = 0, IdConference = idConference, Conference = conference, IdUser = user.Id });
                }
                _conferenceService.SaveConference(conference);
            }
            await CamundaConfOrgUtil.SendAnswerProgComm(answer.answer, answer.user, idConference);
            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> SelectionGuestLect([FromBody] ListOfEmails guestsL)
        {
            await CamundaConfOrgUtil.SelectionGuestLecturers(guestsL.Emails);
            await CamundaConfOrgUtil.SendInviteForGuestLecturers(guestsL.Emails);
            return Ok();
        }

        [HttpPost("[action]/{idConference}")]
        public async Task<IActionResult> SendAnswerGuestLect(int idConference, [FromBody] Answer answer)
        {
            if (answer.answer)
            {
                var conference = _conferenceService.GetConference(idConference);
                var user = _userConfService.GetUser(answer.user);
                if (user != null)
                {
                    conference.GuestLecturers.Add(new GuestLecturer { Id = 0, IdConference = idConference, Conference = conference, IdUser = user.Id });
                }
                _conferenceService.SaveConference(conference);
            }
            await CamundaConfOrgUtil.SendAnswerGuestLect(answer.answer, answer.user, idConference);
            return Ok();
        }

        [HttpPost("[action]/{idConference}")]  //multi-instance
        public async Task<IActionResult> CreateRegistrationGL(int idConference, [FromForm] RegistrationGL registrationGl)
        {
            var user = _userConfService.GetUser(registrationGl.guest);
            Registration registration = new Registration
            {
                Id = 0,
                IdConference = idConference,
                Conference = _conferenceService.GetConference(idConference),
                IdUser = user.Id,
                TimeOfRegistration = DateTime.Today
            };
            if (registrationGl.accomodation)
            {
                var room = _registrationService.GetAllAvailableRooms().FirstOrDefault();
                if (room != null)
                {
                    room.Reservation = true;
                    registration.IdRoomReservation = room.IdRoom;
                }
            }
            _registrationService.SaveRegistration(registration);
            byte[] abstractBytes = null;
            if (registrationGl.paperAbstract != null && registrationGl.paperAbstract.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await registrationGl.paperAbstract.CopyToAsync(memoryStream);
                    abstractBytes = memoryStream.ToArray();
                }
            }

            Paper paper = new Paper { Id = 0, IdRegistration = registration.Id, Title = registrationGl.title, Abstract = abstractBytes, Decision = PaperDecision.Accepted };
            _paperService.SavePaper(paper);
            registration.IdPaper = paper.Id;
            _registrationService.SaveRegistration(registration);
            await CamundaConfOrgUtil.CreateRegistration(registration.Id, paper.Id, registrationGl.guest);
            return Ok(new { IdRegistration = registration.Id, IdPaper = paper.Id });
        }

        [HttpPost("[action]/{idConference}")]
        public async Task<IActionResult> SendCallForPaperSubmission(int idConference)
        {
            var userGuests = _userConfService.GetAll()
                                             .Where(u => u.Role == UserRole.Guest)
                                             .Select(u => u.Email)
                                             .ToList();

            var conference = _conferenceService.GetConference(idConference);

            foreach (var guest in userGuests)
            {
                await EmailService.Execute(guest, "The invite for conference registration", $"Please register on site: http://localhost:3000/{idConference}/guests/{guest}/registration-prompt till {conference.LastDateOfRegistration.Date}.");
                await EmailService.Execute(guest, "The call for paper submission", $"Please upload your paper on site: http://localhost:3000/{idConference}/guests/{guest}/upload-abstract from {conference.PaperSubmissionStart.Date} to {conference.PaperSubmissionEnd.Date}.");
            }

            await CamundaConfOrgUtil.SendCallForPaperSubmissionAndRegistration();
            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> CheckIfAllUsersSendAnswers()
        {
            var processProgComm = await CamundaConfOrgUtil.GetActiveProcess("InviteProgramCommitteeProcess");
            var processGuestLect = await CamundaConfOrgUtil.GetActiveProcess("InviteGuestLecturersProcess");
            if (processProgComm == "null") 
            {
                return Ok("error");
            }
            if(processProgComm != "")
            {
                var state = await CamundaConfOrgUtil.GetStates("InviteProgramCommitteeProcess");
                if(state == "beforeSend") 
                    return Ok("beforeSend");
            }
            if (processGuestLect == "null")
            {
                return Ok("sending");
            }
            if (processGuestLect != "")
            {
                var state = await CamundaConfOrgUtil.GetStates("InviteGuestLecturersProcess");
                if (state == "beforeSend")
                    return Ok("beforeSend");
                else
                    return Ok("afterSend");
            }
            return Ok("afterSend");
        }
    }
}
