using WebAPI.Models;

namespace WebAPI.Services
{
    public interface IConferenceService
    {
        public Conference GetConference(int id);
        public IList<Conference> GetAll();
        public IList<GuestLecturer> GetConferenceGuestLecturers(int id);
        public IList<ProgramCommittee> GetConferenceProgramCommittee(int id);
        public IList<Registration> GetConferenceRegistrations(int id);
        public bool SaveConference(Conference conference);

    }
    public class ConferenceService : IConferenceService
    {
        private IUnitOfWork _work;
        public ConferenceService(IUnitOfWork work)
        {
            _work = work;
        }

        public Conference GetConference(int id)
        {
            Conference conf = _work.ConferenceRepository.Get(id);
            if(conf != null)
            {
                conf.ProgramCommittee = GetConferenceProgramCommittee(id);
                conf.GuestLecturers = GetConferenceGuestLecturers(id);
                conf.Registrations = GetConferenceRegistrations(id);
            }
            
            return conf;
        }

        public IList<Conference> GetAll()
        {
            return _work.ConferenceRepository.GetAll();
        }

        public bool SaveConference(Conference conference)
        {
            if (conference.Id == 0)
            {
                _work.ConferenceRepository.Add(conference);
            }
            else
            {

                foreach (var guestL in conference.GuestLecturers)
                {
                    if (guestL.Id == 0)
                    {
                        _work.GuestLecturerRepository.Add(guestL);
                    }
                    else
                    {
                        guestL.IdConference = conference.Id;
                        _work.GuestLecturerRepository.Update(guestL);
                    }
                }

                foreach (var progComm in conference.ProgramCommittee)
                {
                    if (progComm.Id == 0)
                    {
                        _work.ProgramCommitteeRepository.Add(progComm);
                    }
                    else
                    {
                        progComm.IdConference = conference.Id;
                        _work.ProgramCommitteeRepository.Update(progComm);
                    }
                }

                foreach (var registration in conference.Registrations)
                {
                    if (registration.Id == 0)
                    {
                        _work.RegistrationRepository.Add(registration);
                    }
                    else
                    {
                        registration.IdConference = conference.Id;
                        _work.RegistrationRepository.Update(registration);
                    }
                }

                _work.ConferenceRepository.Update(conference);
            }

            return _work.Save();
        }

        public IList<GuestLecturer> GetConferenceGuestLecturers(int id)
        {
            var conference = _work.ConferenceRepository.Get(id);
            IList<GuestLecturer> guests = new List<GuestLecturer>();
            if(conference != null)
            {
                guests = _work.GuestLecturerRepository.Find(g => g.IdConference == id);
            }
            return guests;
        }

        public IList<ProgramCommittee> GetConferenceProgramCommittee(int id)
        {
            var conference = _work.ConferenceRepository.Get(id);
            IList<ProgramCommittee> progComm = new List<ProgramCommittee>();
            if (conference != null)
            {
                progComm = _work.ProgramCommitteeRepository.Find(g => g.IdConference == id);
                if(progComm != null && progComm.Count > 0)
                {
                    foreach (var p in progComm)
                    {
                        p.Reviews = _work.ReviewRepository.Find(r => r.IdReviewer == p.Id); //vidi jel triba bit idUser a ne Id
                    }
                }
            }
            return progComm;
        }

        public IList<Registration> GetConferenceRegistrations(int id)
        {
            var conference = _work.ConferenceRepository.Get(id);
            IList<Registration> registrations = null;
            if (conference != null)
            {
                registrations = _work.RegistrationRepository.Find(g => g.IdConference == id);
            }
            return registrations;
        }
    }
}
