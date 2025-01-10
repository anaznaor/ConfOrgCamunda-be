using WebAPI.Models;

namespace WebAPI.Services
{
    public interface IRegistrationService
    {
        public Registration GetRegistration(int id);
        public IList<Registration> GetAll();
        public IList<Registration> GetAllWithPapers();
        public IList<Registration> GetAllPayed();
        public IList<Registration> GetEligibleGuests();
        public IList<Hotel> GetAllAvailableRooms();
        public bool SaveRegistration(Registration registration);

    }
    public class RegistrationService : IRegistrationService
    {
        private IUnitOfWork _work;
        public RegistrationService(IUnitOfWork work)
        {
            _work = work;
        }

        public IList<Registration> GetAll()
        {
            return _work.RegistrationRepository.GetAll();
        }

        public IList<Hotel> GetAllAvailableRooms()
        {
            return _work.HotelRepository.Find(r => r.IdRegistration == null);
        }

        public IList<Registration> GetAllPayed()
        {
            return _work.RegistrationRepository.Find(r => r.BillPayment != null);
        }

        public IList<Registration> GetEligibleGuests()
        {
            return _work.RegistrationRepository.Find(r => r.BillPayment != null && r.Paper != null && r.Paper.Decision == PaperDecision.Accepted);
        }

        public IList<Registration> GetAllWithPapers()
        {
            return _work.RegistrationRepository.Find(r => r.Paper != null);
        }

        public Registration GetRegistration(int idConference)
        {
            return _work.RegistrationRepository.Get(idConference);
        }

        public bool SaveRegistration(Registration registration)
        {
            if (registration.Id == 0)
            {
                _work.RegistrationRepository.Add(registration);
            }
            else
            {
                if(registration.Conference.Id == 0)
                {
                    _work.ConferenceRepository.Add(registration.Conference);
                }
                else
                {
                    if (!registration.Conference.Registrations.Contains(registration))
                    {
                        registration.Conference.Registrations.Add(registration);
                        _work.ConferenceRepository.Update(registration.Conference);
                    }
                }


                if (registration.IdUser != null)
                {
                    var user = _work.UserConfRepository.Find(u => u.Id == registration.IdUser).First();
                    if (user != null)
                    {
                        registration.User = user;
                        if(user.Registrations == null)
                        {
                            user.Registrations = new List<Registration>();
                        }
                        user.Registrations.Add(registration);
                        _work.UserConfRepository.Update(registration.User);
                    }
                }

                if (registration.IdRoomReservation != null)
                {
                    var room = _work.HotelRepository.Find(u => u.IdRoom == registration.IdRoomReservation).First();
                    if (room != null)
                    {
                        registration.Room = room;
                        room.Reservation = true;
                        room.IdRegistration = registration.Id;
                        _work.HotelRepository.Update(registration.Room);
                    }
                }

                if (registration.IdPaper != null)
                {
                    var paper = _work.PaperRepository.Find(u => u.Id == registration.IdPaper).First();
                    if (paper != null && registration.Paper == null )
                    {
                        registration.Paper = paper;
                        paper.Registration = registration;
                        _work.PaperRepository.Update(paper);
                    }
                }

                //if (registration.Paper.IdRegistration == 0)
                //{
                //    _work.PaperRepository.Add(registration.Paper);
                //}
                //else
                //{
                //    registration.Paper.IdRegistration = registration.Id;
                //    _work.PaperRepository.Update(registration.Paper);
                //}
                //_work.RegistrationRepository.Update(registration);
            }

            return _work.Save();
        }
    }
}