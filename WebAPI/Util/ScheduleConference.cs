namespace WebAPI.Util
{
    public class ScheduleConference
    {
        public string Title { get; set; } 

        public string Theme { get; set; } 

        public string City { get; set; } 

        public string Country { get; set; }

        public string Hotel { get; set; } 

        public DateTime ConferenceDate { get; set; }

        public int Duration { get; set; }

        public List<string> ProgramCommittee { get; set;}
        public List<SchedulePresentation> Presentations { get; set; }
    }

    public class SchedulePresentation
    {
        public string Fullname { get; set; }
        public string PaperTitle { get; set; }
        public bool IsGuestLecturer { get; set; }
    }
}
