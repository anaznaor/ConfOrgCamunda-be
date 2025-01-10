using WebAPI.Interface;
using WebAPI.Models;

namespace WebAPI.Util.Dto
{
    public class ConferenceDto
    {
        public int Id { get; set; } = 0;
        public string Title { get; set; } = null!;

        public string Theme { get; set; } = null!;

        public string City { get; set; } = null!;

        public string Country { get; set; } = null!;

        public string Hotel { get; set; } = null!;

        public DateTime ConferenceDate { get; set; }

        public int Duration { get; set; }

        public DateTime PaperSubmissionStart { get; set; }
        public DateTime PaperSubmissionEnd { get; set; }

        public DateTime LastDateOfRegistration { get; set; }
    }
}
