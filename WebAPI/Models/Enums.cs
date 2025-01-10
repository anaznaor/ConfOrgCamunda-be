using System;
using System.Collections.Generic;

namespace WebAPI.Models
{
    public enum UserRole
    {
        Organizer = 0,
        ProgramCommittee,
        GuestLecturer,
        Guest,
        Listener,
        Participant
    }

    public enum SaveStatus
    {
        Success,
        Failure,
        InvalidRequest
    }

    public enum NotificationStatus
    {
        Info = 0,
        Warning
    }

    public enum PaperDecision
    {
        Rejected,
        Accepted,
        Correction
    }
}
