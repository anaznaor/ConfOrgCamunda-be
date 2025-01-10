using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebAPI.Interface;
using WebAPI.Models;
using WebAPI.Services;
using WebAPI.Util;
using WebAPI.Util.Dto;

namespace WebAPI.Controllers
{
    public class CamundaReviewingController : Controller
    {
        private readonly ILogger<CamundaReviewingController> _logger;
        private readonly IConferenceService _conferenceService;
        private readonly IRegistrationService _registrationService;
        private readonly IPaperService _paperService;
        private readonly IUserConfService _userConfService;
        private readonly IMapper _mapper;

        public CamundaReviewingController(HttpClient httpClient, ILogger<CamundaReviewingController> logger, IConferenceService conferenceService, IRegistrationService registrationService, IPaperService paperService, IMapper mapper, IUserConfService userConfService)
        {
            _logger = logger;
            _conferenceService = conferenceService;
            _registrationService = registrationService;
            _paperService = paperService;
            _userConfService = userConfService;
            _mapper = mapper;
            //_httpClient = httpClient;
        }

        [HttpPost("[action]/{idConference}")]
        public async Task<IActionResult> StartReviewingProcess([FromBody] string guest, int idConference)
        {
            var coordinatorPC = _conferenceService.GetConference(idConference).ProgramCommittee.ToList().First();
            var coordinator = _userConfService.GetUser(coordinatorPC.IdUser).Email;
            var processInstanceId = await CamundaReviewingUtil.StartReviewingProcess(guest, coordinator, idConference);
            return Ok();
        }

        [HttpGet("[action]/{guest}")]
        public async Task<IActionResult> CheckIfReviewingProcessIsActive(string guest)
        {
            var tasks = await CamundaRegistratingUtil.GetTasks(guest, "ReviewingProcess");
            if (tasks != null && tasks.Count > 0)
            {
                return Ok(true);
            }
            return Ok(false);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> UploadPaper([FromForm] PaperUpload paperUpload)
        {
            var tasks = await CamundaReviewingUtil.GetTasks(paperUpload.Guest);
            if (tasks != null && tasks.Count > 0)
            {
                var taskId = tasks.First().TID;
                if (paperUpload.PaperAbstract != null && paperUpload.PaperAbstract.Length > 0 && paperUpload.Title != null)
                {
                    byte[] abstractBytes = null;
                    using (var memoryStream = new MemoryStream())
                    {
                        await paperUpload.PaperAbstract.CopyToAsync(memoryStream);
                        abstractBytes = memoryStream.ToArray();
                    }

                    Paper paper = new Paper { Id = 0, Title = paperUpload.Title, Abstract = abstractBytes };
                    _paperService.SavePaper(paper);
                    await CamundaReviewingUtil.UploadPaper(paper.Id, paperUpload.Guest, taskId, 0);
                    return Ok(new { IdPaper = paper.Id });
                }
                else if (paperUpload.FullPaper != null && paperUpload.FullPaper.Length > 0)
                {
                    byte[] fullPaperBytes = null;
                    using (var memoryStream = new MemoryStream())
                    {
                        await paperUpload.FullPaper.CopyToAsync(memoryStream);
                        fullPaperBytes = memoryStream.ToArray();
                    }

                    var NoOfUploads = 1;
                    var process = await CamundaReviewingUtil.GetProcessById(tasks.First().PID, false);
                    var paperId = process.IdPaper;
                    var paper = _paperService.GetPaper((int)paperId);
                    if(paper.FullPaper != null)
                    {
                        NoOfUploads++;
                    }
                    paper.FullPaper = fullPaperBytes;
                    _paperService.SavePaper(paper);
                    await CamundaReviewingUtil.UploadPaper(paper.Id, paperUpload.Guest, taskId, NoOfUploads);
                    return Ok(new { IdPaper = paper.Id });
                }
            }
            return Ok("error");
        }

        [HttpGet("[action]/{user}")]
        public async Task<IActionResult> GetCoordinatorsTasks( string user)
        {
            var processes = await CamundaReviewingUtil.GetProcess("ReviewingProcess", false);
            if (processes != null && processes.Count > 0)
            {
                var papers = new List<PaperDto>();
                foreach (var process in processes)
                {
                    var checkIfCoordinator = process.Coordinator == user;
                    var tasks = await CamundaReviewingUtil.GetTasksByPID(user, process.PID);
                    var taskId = tasks != null ? tasks.Find(t => t.TaskName == "Preliminary check of paper")?.TID : null;
                    if (checkIfCoordinator && process.IdPaper != null && taskId != null) 
                    {
                        var paper = _paperService.GetPaper((int)process.IdPaper);
                        if (paper.Decision != PaperDecision.Accepted)
                        {
                            var paperDto = new PaperDto
                            {
                                ProcessId = process.PID,
                                IdPaper = paper.Id,
                                Title = paper.Title,
                                Abstract = Convert.ToBase64String(paper.Abstract),
                                FullPaper = paper.FullPaper != null ? Convert.ToBase64String(paper.FullPaper) : null
                            };
                            papers.Add(paperDto);
                        }
                    } 
                }
                return Ok(new { Papers = papers });
            }
            return Ok(null);
        }

        [HttpPost("[action]/{processId}")]
        public async Task<IActionResult> PreliminaryCheckOfPaper([FromBody] bool preliminaryCheck, string processId)
        {
            await CamundaReviewingUtil.PreliminaryCheckOfPaper(preliminaryCheck, processId);
            return Ok(new {preliminaryCheck});
        }

        [HttpGet("[action]/{idConference}")]
        public async Task<IActionResult> GetReviewerCandidates(int idConference)
        {
            var progComms = _conferenceService.GetConference(idConference).ProgramCommittee.ToList();
            ListOfEmails emails = new ListOfEmails();
            emails.Emails = new List<string>();
            foreach (var p in progComms) 
            {
                var user = _userConfService.GetUser(p.IdUser);
                emails.Emails.Add(user.Email);
            }
            return Ok(emails);
        }

        [HttpPost("[action]/{processId}")]
        public async Task<IActionResult> AssignePaperReviewers([FromBody] ListOfEmails reviewers, string processId)
        {
            await CamundaReviewingUtil.AssignePaperReviewers(reviewers.Emails, processId);
            return Ok();
        }

        [HttpGet("[action]/{user}")]
        public async Task<IActionResult> GetReviewersTasks(string user)
        {
            var processes = await CamundaReviewingUtil.GetProcess("ReviewingProcess", true);
            if (processes != null && processes.Count > 0)
            {
                var papers = new List<PaperDto>();
                foreach (var process in processes)
                {
                    var checkIfReviewer = process.Reviewers?.Contains(user);
                    var tasks = await CamundaReviewingUtil.GetTasksByPID(user, process.PID);
                    var taskId = tasks != null ? tasks.Find(t => t.TaskName == "Create review")?.TID : null;
                    if (checkIfReviewer == true && process.IdPaper != null && taskId != null)
                    {
                        var paper = _paperService.GetPaper((int)process.IdPaper);
                        var paperDto = new PaperDto
                        {
                            ProcessId = process.PID,
                            IdPaper = paper.Id,
                            Title = paper.Title,
                            Abstract = Convert.ToBase64String(paper.Abstract),
                            FullPaper = paper.FullPaper != null ? Convert.ToBase64String(paper.FullPaper) : null
                        };
                        papers.Add(paperDto);
                    }
                }
                return Ok(new { Papers = papers });
            }
            return Ok(null);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> CreateReview([FromBody] ReviewDto reviewDto)
        {
            var process = await CamundaReviewingUtil.GetProcessById(reviewDto.ProcessId, true);
            var tasks = await CamundaReviewingUtil.GetTasksByPID(reviewDto.Reviewer, reviewDto.ProcessId);
            if (tasks != null && process != null)
            {
                var taskId = tasks.Find(t => t.TaskName == "Create review")?.TID;
                if(taskId != null) 
                {
                    var user = _userConfService.GetUser(reviewDto.Reviewer);
                    var reviewer = _conferenceService.GetConference(process.IdConference).ProgramCommittee
                        .ToList()
                        .Where(r => r.IdConference == process.IdConference && r.IdUser == user.Id)
                        .FirstOrDefault();
                    Review review = new Review
                    {
                        Id = 0,
                        IdPaper = reviewDto.IdPaper,
                        IdReviewer = reviewer.Id,
                        Grade = reviewDto.Grade,
                        Description = reviewDto.Description
                    };
                    _paperService.SaveReview(review);
                    await CamundaReviewingUtil.CreateReview(taskId, reviewDto.IdPaper, reviewDto.Reviewer, review.Id);
                }
            }
            return Ok();
        }

    }
}