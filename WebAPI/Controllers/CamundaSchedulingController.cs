using AutoMapper;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;
using WebAPI.Services;
using WebAPI.Util;
using WebAPI.Util.Dto;

namespace WebAPI.Controllers
{
    public class CamundaSchedulingController : Controller
    {
        private readonly ILogger<CamundaReviewingController> _logger;
        private readonly IConferenceService _conferenceService;
        private readonly IRegistrationService _registrationService;
        private readonly IPaperService _paperService;
        private readonly IUserConfService _userConfService;
        private readonly IMapper _mapper;

        public CamundaSchedulingController(HttpClient httpClient, ILogger<CamundaReviewingController> logger, IConferenceService conferenceService, IRegistrationService registrationService, IPaperService paperService, IMapper mapper, IUserConfService userConfService)
        {
            _logger = logger;
            _conferenceService = conferenceService;
            _registrationService = registrationService;
            _paperService = paperService;
            _userConfService = userConfService;
            _mapper = mapper;
            //_httpClient = httpClient;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> CheckIfSchedulingProcessIsActive()
        {
            var tasks = await CamundaScheduleUtil.GetUserTasksByBusinessKey();
            if(tasks != null) 
            {
                var process = await CamundaScheduleUtil.GetProcessById(tasks.First().PID);
                var conference = _conferenceService.GetConference(process.IdConference);
                List<string> progComm = new List<string>();
                foreach(var p in conference.ProgramCommittee)
                {
                    progComm.Add(_userConfService.GetUser(p.IdUser).Fullname);
                }
                ScheduleConference scheduleConference = new ScheduleConference
                {
                    Title = conference.Title,
                    Theme = conference.Theme,
                    City = conference.City,
                    Country = conference.Country,
                    Hotel = conference.Hotel,
                    ConferenceDate = conference.ConferenceDate,
                    Duration = conference.Duration,
                    ProgramCommittee = progComm,
                    Presentations = new List<SchedulePresentation>()
                };

                List<string> guestLects = new List<string>();
                foreach (var p in conference.GuestLecturers)
                {
                    progComm.Add(_userConfService.GetUser(p.IdUser).Fullname);
                }
                var registrationsWithPapers = conference.Registrations.Where(r => r.IdPaper != null);
                foreach(var r in registrationsWithPapers)
                {
                    var paper = _paperService.GetPaper((int)r.IdPaper);
                    var user = _userConfService.GetUser(r.IdUser).Fullname;
                    SchedulePresentation presentation = new SchedulePresentation
                    {
                        Fullname = user,
                        PaperTitle = paper.Title,
                        IsGuestLecturer = guestLects.Contains(user)
                    };
                    scheduleConference.Presentations.Add(presentation);
                } 

                return Ok(new { Conference = scheduleConference });
            }
            return Ok(new { Conference = "null" });
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> CreateAndSendConferenceSchedule([FromForm] PdfFileDto schedulePdf )
        {
            var tasks = await CamundaScheduleUtil.GetUserTasksByBusinessKey();
            if (tasks != null)
            {
                var process = await CamundaScheduleUtil.GetProcessById(tasks.First().PID);
                var conference = _conferenceService.GetConference(process.IdConference);
                List<string> guests = new List<string>();
                foreach (var r in conference.Registrations)
                {
                    var guest = _userConfService.GetUser(r.IdUser).Email;
                    if(guest != null)
                    {
                        guests.Add(guest);
                    }
                }

                byte[] scheduletBytes = null;
                string fileName = "Schedule.pdf";
                if (schedulePdf.file != null && schedulePdf.file.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await schedulePdf.file.CopyToAsync(memoryStream);
                        scheduletBytes = memoryStream.ToArray();
                    }
                }

                foreach (var guest in guests)
                {
                    await EmailService.Execute(guest, "Conference schedule", "Please find the attached schedule of conference.", scheduletBytes, fileName);
                }
                await CamundaScheduleUtil.SendScheduleToGuests(tasks.First().TID, process.IdConference);
                return Ok();
            }
            
            return Ok("error");
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> CreateAndSendConferenceBookOfPapers()
        {
            var tasks = await CamundaScheduleUtil.GetUserTasksByBusinessKey();
            if (tasks != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    var pdfWriter = new PdfWriter(memoryStream);
                    var pdfDocument = new PdfDocument(pdfWriter);
                    var merger = new PdfMerger(pdfDocument);


                    var process = await CamundaScheduleUtil.GetProcessById(tasks.First().PID);
                    var conference = _conferenceService.GetConference(process.IdConference);
                    var registrationsWithPapers = conference.Registrations.Where(r => r.IdPaper != null);
                    foreach (var r in registrationsWithPapers)
                    {
                        var paper = _paperService.GetPaper((int)r.IdPaper);

                        using (var tempPdfStream = new MemoryStream(paper.Abstract))
                        {
                            var reader = new PdfReader(tempPdfStream);
                            var tempPdfDocument = new PdfDocument(reader);
                            merger.Merge(tempPdfDocument, 1, tempPdfDocument.GetNumberOfPages());
                            tempPdfDocument.Close();
                        }

                        if (paper.FullPaper != null)
                        {
                            using (var tempPdfStream = new MemoryStream(paper.FullPaper))
                            {
                                var reader = new PdfReader(tempPdfStream);
                                var tempPdfDocument = new PdfDocument(reader);
                                merger.Merge(tempPdfDocument, 1, tempPdfDocument.GetNumberOfPages());
                                tempPdfDocument.Close();
                            }
                        }
                    }
                    pdfDocument.Close();

                    var bookOfPapers = memoryStream.ToArray();

                    List<string> guests = new List<string>();
                    foreach (var r in conference.Registrations)
                    {
                        var guest = _userConfService.GetUser(r.IdUser).Email;
                        if (guest != null)
                        {
                            guests.Add(guest);
                        }
                    }

                    foreach (var guest in guests)
                    {
                        //await EmailService.Execute(guest, "Book of papers", "Please find the attached book of papers.", bookOfPapers, "BookOfPapers.pdf");
                    }
                    await CamundaScheduleUtil.SendBookOfPapersToGuests(tasks.First().TID);
                    return File(bookOfPapers, "application/pdf", "BookOfPapers.pdf");
                }
            }
                
            return Ok(null);
        }

    }
}
