namespace Application.Server.Models.CoworkingDatabase
{
    public class Workspace
    {
        public int Id { get; set; }
        public int Capacity { get; set; }
        public List<Amenity> Amenities { get; set; } = null!;
        public WorkspaceType WorkspaceType { get; set; }
        public Coworking Coworking { get; set; } = null!;
        public List<Booking> Bookings { get; set; } = null!;
    }
}
