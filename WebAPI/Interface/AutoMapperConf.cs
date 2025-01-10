using AutoMapper;
using WebAPI.Models;

namespace WebAPI.Interface
{
    public class AutoMapperConf : Profile
    {
        public AutoMapperConf()
        {
            CreateMap<Conference, ConferenceModel>()
                .ForMember(d => d.GuestLecturers, s => s.MapFrom(p => p.GuestLecturers))
                .ForMember(d => d.ProgramCommittee, s => s.MapFrom(p => p.ProgramCommittee))
                .ForMember(d => d.Registrations, s => s.MapFrom(p => p.Registrations));
            CreateMap<ConferenceModel, Conference>()
                .ForMember(d => d.GuestLecturers, s => s.MapFrom(p => p.GuestLecturers))
                .ForMember(d => d.ProgramCommittee, s => s.MapFrom(p => p.ProgramCommittee))
                .ForMember(d => d.Registrations, s => s.MapFrom(p => p.Registrations));

            CreateMap<UserConf, UserConfModel>()
                .ForMember(d => d.GuestLecturers, s => s.MapFrom(p => p.GuestLecturers))
                .ForMember(d => d.ProgramCommittees, s => s.MapFrom(p => p.ProgramCommittees))
                .ForMember(d => d.Registrations, s => s.MapFrom(p => p.Registrations));
            CreateMap<UserConfModel, UserConf>()
                .ForMember(d => d.GuestLecturers, s => s.MapFrom(p => p.GuestLecturers))
                .ForMember(d => d.ProgramCommittees, s => s.MapFrom(p => p.ProgramCommittees))
                .ForMember(d => d.Registrations, s => s.MapFrom(p => p.Registrations));

            CreateMap<GuestLecturer, GuestLecturerModel>();
            CreateMap<GuestLecturerModel, GuestLecturer>();

            CreateMap<Hotel, HotelModel>();
            CreateMap<HotelModel, Hotel>();

            CreateMap<Paper, PaperModel>()
                .ForMember(d => d.Reviews, s => s.MapFrom(p => p.Reviews));
            CreateMap<PaperModel, Paper>()
                .ForMember(d => d.Reviews, s => s.MapFrom(p => p.Reviews));

            CreateMap<ProgramCommittee, ProgramCommitteeModel>()
                .ForMember(d => d.Reviews, s => s.MapFrom(p => p.Reviews));
            CreateMap<ProgramCommitteeModel, ProgramCommittee>()
                .ForMember(d => d.Reviews, s => s.MapFrom(p => p.Reviews));

            CreateMap<Registration, RegistrationModel>();
            CreateMap<RegistrationModel, Registration>();

            CreateMap<Review, ReviewModel>();
            CreateMap<ReviewModel, Review>();

            //CreateMap<List<GuestLecturer>, List<GuestLecturerModel>>();
            //CreateMap<List<GuestLecturerModel>, List<GuestLecturer>>();

            //CreateMap<List<ProgramCommittee>, List<ProgramCommitteeModel>>();
            //CreateMap<List<ProgramCommitteeModel>, List<ProgramCommittee>>();

            //CreateMap<List<Review>, List<ReviewModel>>();
            //CreateMap<List<ReviewModel>, List<Review>>();

            //CreateMap<List<Registration>, List<RegistrationModel>>();
            //CreateMap<List<RegistrationModel>, List<Registration>>();

            //CreateMap<List<Schedule>, List<ScheduleModel>>();
            //CreateMap<List<ScheduleModel>, List<Schedule>>();

            //CreateMap<List<Session>, List<SessionModel>>();
            //CreateMap<List<SessionModel>, List<Session>>();

            //CreateMap<List<Registration>, List<RegistrationModel>>();
            //CreateMap<List<RegistrationModel>, List<Registration>>();
        }

    }
}
