namespace Application.Server.Models.DTOs.GetBooking
{
    public class BookingDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public WorkspaceDto Workspace { get; set; } = null!;
        public int? Seats { get; set; } = null;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
    }
}
