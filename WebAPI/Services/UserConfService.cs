using WebAPI.Models;

namespace WebAPI.Services
{
    public interface IUserConfService
    {
        public UserConf GetUser(int id);
        public UserConf GetUser(string email);
        public IList<UserConf> GetAll();
        public IList<UserConf> GetUsersByRole(UserRole role);
        public bool SaveUser(UserConf userConf);
    }
    public class UserConfService : IUserConfService
    {
        private IUnitOfWork _work;
        public UserConfService(IUnitOfWork work)
        {
            _work = work;
        }

        public UserConf GetUser(int id)
        {
            var user = _work.UserConfRepository.Get(id);
            if (user != null)
            {
                if (user.Role == UserRole.ProgramCommittee)
                {
                    user.ProgramCommittees = _work.ProgramCommitteeRepository.Find(p => p.IdUser == user.Id);
                }
                else
                {
                    user.GuestLecturers = _work.GuestLecturerRepository.Find(p => p.IdUser == user.Id);
                    user.Registrations = _work.RegistrationRepository.Find(r => r.IdUser == user.Id);
                }
            }
            return user;
        }

        public IList<UserConf> GetAll()
        {
            IList<UserConf> users = _work.UserConfRepository.GetAll();
            foreach(var user in users)
            {
                    if (user.Role == UserRole.ProgramCommittee)
                    {
                        user.ProgramCommittees = _work.ProgramCommitteeRepository.Find(p => p.IdUser == user.Id);
                    }
                    else
                    {
                        user.GuestLecturers = _work.GuestLecturerRepository.Find(p => p.IdUser == user.Id);
                        user.Registrations = _work.RegistrationRepository.Find(r => r.IdUser == user.Id);
                    }
            }
            return users;
        }

        public bool SaveUser(UserConf userConf)
        {
            if (userConf.Id == 0)
            {
                _work.UserConfRepository.Add(userConf);
            }
            else
            {
                //foreach(var guestL in userConf.GuestLecturers)
                //{
                //    if(guestL.Id == 0)
                //    {
                //        _work.GuestLecturerRepository.Add(guestL);
                //    }
                //    else
                //    {
                //        guestL.IdUser = userConf.Id;
                //        _work.GuestLecturerRepository.Update(guestL);
                //    }
                //}

                //foreach (var progComm in userConf.ProgramCommittees)
                //{
                //    if (progComm.Id == 0)
                //    {
                //        _work.ProgramCommitteeRepository.Add(progComm);
                //    }
                //    else
                //    {
                //        progComm.IdUser = userConf.Id;

                //        var progCommReviews = _work.ReviewRepository.Find(r => r.IdReviewer == progComm.Id);
                //        var progCommSessions = _work.SessionRepository.Find(r => r.IdChairperson == progComm.Id);

                //        if(progCommSessions.Count() > 0)
                //        {
                //            _work.SessionRepository.RemoveRange(progCommSessions);
                //        }
                //        if (progCommReviews.Count() > 0)
                //        {
                //            _work.ReviewRepository.RemoveRange(progCommReviews);
                //        }

                //        if (progComm.Sessions.Count() > 0)
                //        {
                //            foreach(var s in progComm.Sessions)
                //            {
                //                s.IdChairperson = progComm.Id;
                //            }
                //        }

                //        if (progComm.Reviews.Count() > 0)
                //        {
                //            foreach (var r in progComm.Reviews)
                //            {
                //                r.IdReviewer = progComm.Id;
                //            }
                //        }

                //        _work.ReviewRepository.AddRange(progComm.Reviews);
                //        _work.SessionRepository.AddRange(progComm.Sessions);
                //        _work.ProgramCommitteeRepository.Update(progComm);
                //    }
                //}

                //foreach (var registration in userConf.Registrations)
                //{
                //    if (registration.Id == 0)
                //    {
                //        _work.RegistrationRepository.Add(registration);
                //    }
                //    else
                //    {
                //        registration.IdUser = userConf.Id;
                //        _work.RegistrationRepository.Update(registration);
                //    }
                //}

                //foreach (var session in userConf.Sessions)
                //{
                //    if (session.Id == 0)
                //    {
                //        _work.SessionRepository.Add(session);
                //    }
                //    else
                //    {
                //        if(userConf.Role == UserRole.ProgramCommittee)
                //        {
                //            session.IdChairperson = userConf.Id;
                //        }
                //        else
                //        { 
                //            session.IdLecturer = userConf.Id;
                //        }
                       
                //        _work.SessionRepository.Update(session);
                //    }
                //}

                _work.UserConfRepository.Update(userConf);
            }

            return _work.Save();
        }

        public UserConf GetUser(string email)
        {
            var user = _work.UserConfRepository.Find(u => u.Email == email).FirstOrDefault();
            //if(user != null)
            //{
            //    if (user.Role == UserRole.ProgramCommittee)
            //    {
            //        user.ProgramCommittees = _work.ProgramCommitteeRepository.Find(p => p.IdUser == user.Id);
            //    }
            //    else
            //    {
            //        user.GuestLecturers = _work.GuestLecturerRepository.Find(p => p.IdUser == user.Id);
            //        user.Registrations = _work.RegistrationRepository.Find(r => r.IdUser == user.Id);
            //    }
            //}
            return user;
        }

        public IList<UserConf> GetUsersByRole(UserRole role)
        {
            return GetAll().Where(u => u.Role == role).ToList();
        }
    }
}
