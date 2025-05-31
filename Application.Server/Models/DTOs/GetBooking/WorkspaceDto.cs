using Application.Server.Models.CoworkingDatabase;

namespace Application.Server.Models.DTOs.GetBooking
{
    public class WorkspaceDto
    {
        public int Id { get; set; }
        public int Capacity { get; set; }
        public List<Amenity> Amenities { get; set; } = null!;
        public WorkspaceType WorkspaceType { get; set; }
    }
}
