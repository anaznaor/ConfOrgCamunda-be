using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Models
{
    public class Conference
    {
        public int Id { get; set; }
        [Column(TypeName = "NVARCHAR(100)")]
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

        public IList<GuestLecturer> GuestLecturers { get; set; }

        public IList<ProgramCommittee> ProgramCommittee { get; set; }
        public IList<Registration>? Registrations { get; set; }
    }

}