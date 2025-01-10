using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Interface
{
    public class ConferenceModel
    {
        public int Id { get; set; }
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

        public List<GuestLecturerModel> GuestLecturers { get; set; }

        public List<ProgramCommitteeModel> ProgramCommittee { get; set; }
        public List<RegistrationModel>? Registrations { get; set; }
    }

}