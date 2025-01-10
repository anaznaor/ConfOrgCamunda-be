using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Interface;
using WebAPI.Models;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ConferenceController : Controller
    {
        //vidi je triba jos IUserService za nekakvu autentifikaciju
        private readonly ILogger<ConferenceController> _logger;
        private readonly IConferenceService _conferenceService;
        private readonly IMapper _mapper;

        public ConferenceController(ILogger<ConferenceController> logger, IConferenceService conferenceService, IMapper mapper)
        {
            _logger = logger;
            _conferenceService = conferenceService;
            _mapper = mapper;
        }

        [HttpGet("[action]")]
        public async Task<List<ConferenceModel>> GetAllConferences()
        {
            List<Conference> conferences = _conferenceService.GetAll().ToList();
            return _mapper.Map<List<ConferenceModel>>(conferences);
        }

        [HttpGet("[action]/{id:int}")]
        public async Task<ConferenceModel> GetConference(int id)
        {
            Conference conference = _conferenceService.GetConference(id);
            return _mapper.Map<ConferenceModel>(conference);
        }

        //[Authorize(Roles = "Organizer")]
        [HttpPost("[action]")]
        public async Task<SaveRes> CreateConference([FromBody]ConferenceModel conference)
        {
            SaveRes res = new SaveRes() { Message = "Invalid request", Status = SaveStatus.InvalidRequest };
            if(conference != null) 
            {
                var conferenceEntity = _mapper.Map<Conference>(conference);
                _conferenceService.SaveConference(conferenceEntity);
                res.CreatedId = conferenceEntity.Id;
                res.Message = "Save successful";
                res.Status = SaveStatus.Success;
            }
            return res;
        }

        //[Authorize(Roles = "Organizer")]
        [HttpPut("[action]")]
        public async Task<SaveRes> UpdateConference([FromBody] ConferenceModel conference)
        {
            SaveRes res = new SaveRes() { Message = "Invalid request", Status = SaveStatus.InvalidRequest };
            if (conference != null)
            {
                var conferenceEntity = _mapper.Map<Conference>(conference);
                _conferenceService.SaveConference(conferenceEntity);
                res.CreatedId = conferenceEntity.Id;
                res.Message = "Save successful";
                res.Status = SaveStatus.Success;
            }
            return res;
        }
    }
}
