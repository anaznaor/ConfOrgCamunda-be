namespace WebAPI.Util.Dto
{
    public class TaskInfo
    {
        public string TID { get; set; }
        public string TaskKey { get; set; }
        public string TaskName { get; set; }
        public string PID { get; set; }
        public int IdConference { get; set; }
        public string? ProgramCommittee { get; set; }
        public string? GuestLecturer { get; set; }
        public string? Guest { get; set; }
        public int? IdPaper { get; set; }
        public string? Coordinator { get; set; }
        public string? Reviewer { get; set; }
        public DateTime StartTime { get; set; }
    }
}
