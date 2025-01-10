namespace WebAPI.Util.Dto
{
    public class RegistrationGL
    {
        public bool accomodation { get; set; }
        public string guest { get; set; }
        public string title { get; set; }

        public IFormFile paperAbstract { get; set; }
    }
}
