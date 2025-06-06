using Application.Server.Models.CoworkingDatabase;

namespace Application.Server.Models.DTOs
{
    public class EditBookingDto
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public WorkspaceType WorkspaceType { get; set; }
        public int? Seats { get; set; } = null;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
    }
}
