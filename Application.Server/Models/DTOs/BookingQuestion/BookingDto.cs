namespace Application.Server.Models.DTOs.BookingQuestion
{
    public class BookingDto
    {
        public int Id { get; set; }
        public WorkspaceDto Workspace { get; set; } = null!;
        public int Seats { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
    }
}
