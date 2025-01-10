using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Models
{
    public class Registration
    {
        public int Id { get; set; }

        public int IdConference { get; set; }
        public Conference Conference { get; set; }

        public int IdUser { get; set; }
        public UserConf User { get; set; }

        public DateTime TimeOfRegistration { get; set; }

        public int? IdRoomReservation { get; set; }

        public Hotel Room { get; set; }
        [Column(TypeName = "VARBINARY(MAX)")]
        public byte[]? BillPayment { get; set; }
        public int? IdPaper { get; set; }

        public Paper Paper { get; set; }
    }
}
