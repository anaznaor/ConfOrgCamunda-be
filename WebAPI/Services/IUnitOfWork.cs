using WebAPI.Models;

namespace WebAPI.Services
{
    public interface IUnitOfWork
    {
        IRepository<Conference, Conference> ConferenceRepository { get; }
        IRepository<Hotel, Hotel> HotelRepository { get; }
        IRepository<Paper, Paper> PaperRepository { get; }
        IRepository<Registration, Registration> RegistrationRepository { get; }
        IRepository<Review, Review> ReviewRepository { get; }
        IRepository<UserConf, UserConf> UserConfRepository { get; }
        IRepository<ProgramCommittee, ProgramCommittee> ProgramCommitteeRepository { get; }
        IRepository<GuestLecturer, GuestLecturer> GuestLecturerRepository { get; }
        bool Save();

    }
}
