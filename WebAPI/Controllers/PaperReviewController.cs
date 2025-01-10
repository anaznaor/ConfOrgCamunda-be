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
    public class PaperReviewController : Controller
    {
        private readonly ILogger<PaperReviewController> _logger;
        private readonly IPaperService _paperService;
        private readonly IMapper _mapper;

        public PaperReviewController(ILogger<PaperReviewController> logger, IPaperService paperService, IMapper mapper)
        {
            _logger = logger;
            _paperService = paperService;
            _mapper = mapper;
        }

        [HttpGet("[action]")]
        public async Task<List<PaperModel>> GetPapers()
        {
            List<Paper> papers = _paperService.GetAllPapers().ToList();
            return _mapper.Map<List<PaperModel>>(papers);
        }

        [HttpGet("[action]")]
        public async Task<List<PaperModel>> GetEligiblePapers()
        {
            List<Paper> papers = _paperService.GetEligiblePapers().ToList();
            return _mapper.Map<List<PaperModel>>(papers);
        }

        [HttpGet("[action]/{id:int}")]
        public async Task<List<PaperModel>> GetConferencePapers(int id)
        {
            List<Paper> papers = _paperService.GetConferencePapers(id).ToList();
            return _mapper.Map<List<PaperModel>>(papers);
        }

        [HttpGet("[action]/{id:int}")]
        public async Task<PaperModel> GetPaper(int id)
        {
            Paper paper = _paperService.GetPaper(id);
            return _mapper.Map<PaperModel>(paper);
        }

        //[Authorize(Roles = "Organizer")]
        [HttpPost("[action]")]
        public async Task<SaveRes> CreatePaper([FromBody] PaperModel paper)
        {
            SaveRes res = new SaveRes() { Message = "Invalid request", Status = SaveStatus.InvalidRequest };
            if (paper != null)
            {
                var paperEntity = _mapper.Map<Paper>(paper);
                _paperService.SavePaper(paperEntity);
                res.CreatedId = paperEntity.Id;
                res.Message = "Save successful";
                res.Status = SaveStatus.Success;
            }
            return res;
        }

        //[Authorize(Roles = "Organizer")]
        [HttpPut("[action]")]
        public async Task<SaveRes> UpdatePaper([FromBody] PaperModel paper)
        {
            SaveRes res = new SaveRes() { Message = "Invalid request", Status = SaveStatus.InvalidRequest };
            if (paper != null)
            {
                var paperEntity = _mapper.Map<Paper>(paper);
                _paperService.SavePaper(paperEntity);
                res.CreatedId = paperEntity.Id;
                res.Message = "Save successful";
                res.Status = SaveStatus.Success;
            }
            return res;
        } 
        
        [HttpGet("[action]/{id}")]
        public async Task<ReviewModel> GetReview(int id)
        {
            Review review = _paperService.GetReview(id);
            return _mapper.Map<ReviewModel>(review);
        }

        [HttpGet("[action]/{id}")]
        public async Task<List<ReviewModel>> GetPaperReviews(int id)
        {
            List<Review> reviews = _paperService.GetPaperReviews(id).ToList();
            return _mapper.Map<List<ReviewModel>>(reviews);
        }

        //[Authorize(Roles = "ProgramCommittee")]
        [HttpPost("[action]")]
        public async Task<SaveRes> CreateReview([FromBody] ReviewModel review)
        {
            SaveRes res = new SaveRes() { Message = "Invalid request", Status = SaveStatus.InvalidRequest };
            if (review != null)
            {
                var reviewEntity = _mapper.Map<Review>(review);
                _paperService.SaveReview(reviewEntity);
                res.CreatedId = reviewEntity.Id;
                res.Message = "Save successful";
                res.Status = SaveStatus.Success;
            }
            return res;
        }
    }
}
