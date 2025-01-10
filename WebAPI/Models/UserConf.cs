using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Models
{

    public class UserConf
    {
        public UserConf() { }

        public UserConf(string email, string fullname)
        {
            this.Email = email;
            this.Fullname = fullname;
        }
        public int Id { get; set; }
        [Column(TypeName = "NVARCHAR(50)")]
        public string Fullname { get; set; }

        public string Sex { get; set; }
        [Column(TypeName = "NVARCHAR(11)")]
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

        public IList<Registration>? Registrations { get; set; }

        public IList<ProgramCommittee>? ProgramCommittees { get; set; }
        public IList<GuestLecturer>? GuestLecturers { get; set; }
    }
}
