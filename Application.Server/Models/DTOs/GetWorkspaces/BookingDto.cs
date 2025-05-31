namespace Application.Server.Models.DTOs.GetWorkspaces
{
    public class BookingDto
    {
        public int Id { get; set; }
        public int? Seats { get; set; } = null;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
    }
}
