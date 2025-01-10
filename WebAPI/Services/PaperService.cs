using AutoMapper;
using WebAPI.Models;
using WebAPI.Util.Dto;

namespace WebAPI.Services
{
    public interface IPaperService
    {
        public Paper GetPaper(int id);
        public Paper GetRegistrationPaper(int idRegistration);
        public IList<Paper> GetAllPapers();
        public IList<Paper> GetEligiblePapers();
        public Review GetReview(int idReview);
        public IList<Review> GetPaperReviews(int idPaper);
        public IList<Paper> GetConferencePapers(int idConference);
        public bool SavePaper(Paper paper);
        public bool SaveReview(Review review);
        public bool DeleteReviews(List<Review> reviews);
    }
    public class PaperService : IPaperService
    {
        private IUnitOfWork _work;
        public PaperService(IUnitOfWork work)
        {
            _work = work;
        }

        public IList<Paper> GetConferencePapers(int idConference)
        {
            var conference = _work.ConferenceRepository.Get(idConference);
            IList<Paper> papers = null;
            if (conference != null)
            {
                papers = _work.PaperRepository.Find(g => g.Registration.Conference == conference);
            }
            return papers;
        }

        public IList<Review> GetPaperReviews(int idPaper)
        {
            var paper = _work.PaperRepository.Get(idPaper);
            IList<Review> reviews = new List<Review>();
            if (paper != null)
            {
                reviews = _work.ReviewRepository.Find(g => g.Paper == paper || g.IdPaper == paper.Id);
            }
            return reviews;
        }

        public IList<Paper> GetAllPapers()
        {
            return _work.PaperRepository.GetAll();
        }

        public Paper GetPaper(int id)
        {
            var paper = _work.PaperRepository.Get(id);
            var reviews = GetPaperReviews(id);
            if(reviews != null)
            {
                paper.Reviews = reviews;
            }
            return paper;
        }

        public IList<Paper> GetEligiblePapers()
        {
            return _work.PaperRepository.Find(r => r.Decision == PaperDecision.Accepted);
        }

        public Paper GetRegistrationPaper(int idRegistration)
        {
            var registration = _work.RegistrationRepository.Get(idRegistration);
            Paper paper = null;
            if (registration != null)
            {
                paper = _work.PaperRepository.Find(p => p.Registration == registration || p.IdRegistration == registration.Id).FirstOrDefault();
            }
            return paper;
        }

        public Review GetReview(int idReview)
        {
            return _work.ReviewRepository.Get(idReview);
        }

        public bool SavePaper(Paper paper)
        {
            if (paper.Id == 0)
            {
                _work.PaperRepository.Add(paper);
            }
            else
            {
                //if (paper.IdRegistration != null)
                //{
                //    var registration = _work.RegistrationRepository.Find(r => r.Id == paper.IdRegistration).First();
                //    if (registration != null)
                //    {
                //        paper.Registration = registration;
                //        if (registration.Paper == null)
                //        {
                //            registration.Paper = paper;
                //            _work.RegistrationRepository.Update(registration);
                //        }  
                //    }
                //}

                //if (paper.Registration.Id == 0)
                //{
                //    _work.RegistrationRepository.Add(paper.Registration);
                //}
                //else
                //{
                //    paper.Registration.IdPaper = paper.Id;
                //    _work.RegistrationRepository.Update(paper.Registration);
                //}

                //if (paper.Session?.Id == 0)
                //{
                //    _work.SessionRepository.Add(paper.Session);
                //}
                //else
                //{
                //    paper.Session.IdPaper = paper.Id;
                //    _work.SessionRepository.Update(paper.Session);
                //}
                //if (paper.Reviews != null && paper.Reviews.Count > 0)
                //{
                //    foreach (var review in paper.Reviews)
                //    {
                //        if (review.Id == 0)
                //        {
                //            _work.ReviewRepository.Add(review);
                //        }
                //        else
                //        {
                //            review.IdPaper = paper.Id;
                //            _work.ReviewRepository.Update(review);
                //        }
                //    }
                //}
                _work.PaperRepository.Update(paper);
            }

            return _work.Save();
        }

        public bool SaveReview(Review review)
        {
            if (review.Id == 0)
            {
                _work.ReviewRepository.Add(review);
            }
            else
            {
                if(review.IdPaper > 0)
                {
                    var paper = _work.PaperRepository.Find(p => p.Id == review.IdPaper).First();
                    if (paper != null)
                    {
                        review.Paper = paper;
                        if (!paper.Reviews.Contains(review))
                        {
                            paper.Reviews.Add(review);
                            _work.PaperRepository.Update(paper);
                        }
                    }
                }

                if (review.IdReviewer > 0)
                {
                    var reviewer = _work.ProgramCommitteeRepository.Find(p => p.Id == review.IdReviewer).First();
                    if (reviewer != null)
                    {
                        review.Reviewer = reviewer;
                        if (!reviewer.Reviews.Contains(review))
                        {
                            reviewer.Reviews.Add(review);
                            _work.ProgramCommitteeRepository.Update(reviewer);
                        }
                    }
                }

                _work.ReviewRepository.Update(review);
            }

            return _work.Save();
        }

        public bool DeleteReviews(List<Review> reviews)
        {
            _work.ReviewRepository.RemoveRange(reviews);
            return _work.Save();
        }
    }
}
