using System;
using System.Collections.Generic;

namespace WebAPI.Interface
{
    public class ScheduleModel
    {
        public int Id { get; set; }

        public int IdConference { get; set; }

        public List<SessionModel> Sessions { get; set; }
    }

    public class SessionModel
    {
        public int Id { get; set; }

        public int IdSchedule { get; set; }

        public DateOnly Date { get; set; }

        public TimeOnly TimeStart { get; set; }

        public TimeOnly TimeEnd { get; set; }

        public int IdChairperson { get; set; }

        public int IdLecturer { get; set; }

        public int IdPaper { get; set; }
    }

}
