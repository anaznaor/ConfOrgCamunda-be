namespace WebAPI.Models
{
    public class Hotel
    {
        public int IdRoom { get; set; }

        public bool Reservation { get; set; } = false;

        public Registration? Registration { get; set; }

        public int? IdRegistration { get; set; }
    }
}
