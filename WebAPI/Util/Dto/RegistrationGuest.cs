using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Util.Dto
{
    public class RegistrationGuest
    {
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
        public bool Accomodation { get; set; }
    }
}
