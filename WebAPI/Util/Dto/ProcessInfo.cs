namespace WebAPI.Util.Dto
{
    public class ProcessInfo
    {
        public string PID { get; set; }
        public int IdConference { get; set; }
        public string? User { get; set; }  //progComm, guestLect, reviewer
        public string? Guest { get; set; }
        public string? Coordinator { get; set; }
        public List<string>? ProgComms { get; set; }
        public List<string>? GuestLects { get; set; }
        public List<string>? Reviewers { get; set; }
        public int? IdPaper { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public bool Ended { get; set; }
        public bool TimePassed { get; set; }

        public bool Accomodation { get; set; } = false;
        public int? DaysTillConferenceTime { get; set; }
        public string? UserTitle { get; set; }

        public string? PaperSubmissionEnd { get; set; }
        public string? EndOfRegistration { get; set; }
        public int? AverageGrade { get; set; }

    }
}
