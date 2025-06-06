using Application.Server.Models.CoworkingDatabase;
using AutoMapper;

namespace Application.Server.Models.Mapper
{
    public class CoworkingProfile:Profile
    {
        public CoworkingProfile()
        {
            CreateMap<Workspace, DTOs.GetBooking.WorkspaceDto> ();
            CreateMap<Booking, DTOs.GetBooking.BookingDto> ();
            CreateMap<Workspace, DTOs.GetWorkspaces.WorkspaceDto> ();
            CreateMap<Booking, DTOs.GetWorkspaces.BookingDto> ();
        }
    }
}
