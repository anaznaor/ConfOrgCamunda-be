

namespace WebAPI.Interface
{
    public class RegistrationModel
    {
        public int Id { get; set; }

        public int IdConference { get; set; }

        public int IdUser { get; set; }

        public DateTime TimeOfRegistration { get; set; }

        public int? IdRoomReservation { get; set; }

        public bool BillPayment { get; set; }
        public int IdPaper { get; set; }
    }
}
