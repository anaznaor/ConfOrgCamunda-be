using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Interface;
using WebAPI.Models;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : Controller
    {
        private readonly ILogger<RegistrationController> _logger;
        private readonly IRegistrationService _registrationService;
        private readonly IMapper _mapper;

        public RegistrationController(ILogger<RegistrationController> logger, IRegistrationService registrationService, IMapper mapper)
        {
            _logger = logger;
            _registrationService = registrationService;
            _mapper = mapper;
        }

        [HttpGet("[action]")]
        public async Task<List<RegistrationModel>> GetAllRegistrations()
        {
            List<Registration> registrations = _registrationService.GetAll().ToList();
            return _mapper.Map<List<RegistrationModel>>(registrations);
        }

        [HttpGet("[action]/{id:int}")]
        public async Task<RegistrationModel> GetRegistration(int id)
        {
            Registration registration = _registrationService.GetRegistration(id);
            return _mapper.Map<RegistrationModel>(registration);
        }

        [HttpGet("[action]/{id:int}")]
        public async Task<List<RegistrationModel>> GetEligibleGuests(int id)
        {
            List<Registration> registrations = _registrationService.GetEligibleGuests().ToList();
            return _mapper.Map<List<RegistrationModel>>(registrations);
        }
        [HttpGet("[action]")]
        public async Task<HotelModel> GetAvailableRoom()
        {
            Hotel room = _registrationService.GetAllAvailableRooms().First();
            return _mapper.Map<HotelModel> (room);
        }

        //[Authorize(Roles = "Guest")]
        [HttpPost("[action]")]
        public async Task<SaveRes> CreateRegistration([FromBody] RegistrationModel registration)
        {
            SaveRes res = new SaveRes() { Message = "Invalid request", Status = SaveStatus.InvalidRequest };
            if (registration != null)
            {
                var registrationEntity = _mapper.Map<Registration>(registration);
                _registrationService.SaveRegistration(registrationEntity);
                res.CreatedId = registrationEntity.Id;
                res.Message = "Save successful";
                res.Status = SaveStatus.Success;
            }
            return res;
        }
    }
}
