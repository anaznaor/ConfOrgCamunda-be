using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebAPI.Models;

namespace WebAPI.Interface
{

    public class UserConfModel
    {
        public int Id { get; set; }
        public string Fullname { get; set; }

        public string Sex { get; set; }
        public string Oib { get; set; }

        public DateOnly DateOfBirth { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public string Title { get; set; }

        public string Profession { get; set; }

        public string Company { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public UserRole Role { get; set; }

        public List<SessionModel>? Sessions { get; set; }

        public List<RegistrationModel>? Registrations { get; set; }

        public List<ProgramCommitteeModel>? ProgramCommittees { get; set; }
        public List<GuestLecturerModel>? GuestLecturers { get; set; }
    }
}
