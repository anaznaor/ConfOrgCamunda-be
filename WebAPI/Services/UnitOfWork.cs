using WebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Services
{
    public class UnitOfWork : IDisposable, IUnitOfWork
    {
        private DbContext _context;
        private BaseRepository<Conference, Conference> conferenceRepository;
        private BaseRepository<Hotel, Hotel> hotelRepository;
        private BaseRepository<Paper, Paper> paperRepository;
        private BaseRepository<Registration, Registration> registrationRepository;
        private BaseRepository<Review, Review> reviewRepository;
        private BaseRepository<UserConf, UserConf> userConfRepository;
        private BaseRepository<ProgramCommittee, ProgramCommittee> programCommitteeRepository;
        private BaseRepository<GuestLecturer, GuestLecturer> guestLecturerRepository;

        public IRepository<Conference, Conference> ConferenceRepository
        {
            get
            {
                if (conferenceRepository == null)
                {
                    conferenceRepository = new BaseRepository<Conference, Conference>(_context);
                }
                return conferenceRepository;
            }
        }

        public IRepository<Hotel, Hotel> HotelRepository
        {
            get
            {
                if (hotelRepository == null)
                {
                    hotelRepository = new BaseRepository<Hotel, Hotel>(_context);
                }
                return hotelRepository;
            }
        }

        public IRepository<Paper, Paper> PaperRepository
        {
            get
            {
                if (paperRepository == null)
                {
                    paperRepository = new BaseRepository<Paper, Paper>(_context);
                }
                return paperRepository;
            }
        }

        public IRepository<Registration, Registration> RegistrationRepository
        {
            get
            {
                if (registrationRepository == null)
                {
                    registrationRepository = new BaseRepository<Registration, Registration>(_context);
                }
                return registrationRepository;
            }
        }

        public IRepository<Review, Review> ReviewRepository
        {
            get
            {
                if (reviewRepository == null)
                {
                    reviewRepository = new BaseRepository<Review, Review>(_context);
                }
                return reviewRepository;
            }
        }

        public IRepository<UserConf, UserConf> UserConfRepository
        {
            get
            {
                if (userConfRepository == null)
                {
                    userConfRepository = new BaseRepository<UserConf, UserConf>(_context);
                }
                return userConfRepository;
            }
        }

        public IRepository<ProgramCommittee, ProgramCommittee> ProgramCommitteeRepository
        {
            get
            {
                if (programCommitteeRepository == null)
                {
                    programCommitteeRepository = new BaseRepository<ProgramCommittee, ProgramCommittee>(_context);
                }
                return programCommitteeRepository;
            }
        }

        public IRepository<GuestLecturer, GuestLecturer> GuestLecturerRepository
        {
            get
            {
                if (guestLecturerRepository == null)
                {
                    guestLecturerRepository = new BaseRepository<GuestLecturer, GuestLecturer>(_context);
                }
                return guestLecturerRepository;
            }
        }

        public UnitOfWork(DbContext context)
        {
            _context = context;
        }

        public bool Save()
        {
            _context.SaveChanges();
            return true;
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
