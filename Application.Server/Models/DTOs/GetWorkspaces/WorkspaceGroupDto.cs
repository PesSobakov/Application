using Application.Server.Models.CoworkingDatabase;

namespace Application.Server.Models.DTOs.GetWorkspaces
{
    public class WorkspaceGroupDto
    {
        public WorkspaceType WorkspaceType { get; set; }
        public List<Amenity> Amenities { get; set; } = null!;
        public List<FreeRoomsDto> FreeRooms { get; set; } = null!;
        public List<BookingDto> Bookings { get; set; } = null!;
    }
}
