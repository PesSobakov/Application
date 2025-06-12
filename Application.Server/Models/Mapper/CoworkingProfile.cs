using Application.Server.Models.CoworkingDatabase;
using AutoMapper;

namespace Application.Server.Models.Mapper
{
    public class CoworkingProfile : Profile
    {
        public CoworkingProfile()
        {
            CreateMap<Coworking, DTOs.GetBooking.CoworkingDto>();
            CreateMap<Workspace, DTOs.GetBooking.WorkspaceDto>();
            CreateMap<Booking, DTOs.GetBooking.BookingDto>();
            CreateMap<Coworking, DTOs.BookingQuestion.CoworkingDto>();
            CreateMap<Workspace, DTOs.BookingQuestion.WorkspaceDto>();
            CreateMap<Booking, DTOs.BookingQuestion.BookingDto>();
        }
    }
}
