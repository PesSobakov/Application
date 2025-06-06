namespace Application.Server.Models.CoworkingDatabase
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public List<Booking> Bookings { get; set; } = null!;
    }
}
