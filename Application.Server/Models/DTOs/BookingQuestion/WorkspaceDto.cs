using Application.Server.Models.CoworkingDatabase;

namespace Application.Server.Models.DTOs.BookingQuestion
{
    public class WorkspaceDto
    {
        public int Id { get; set; }
        public int Capacity { get; set; }
        public WorkspaceType WorkspaceType { get; set; }
        public CoworkingDto Coworking { get; set; } = null!;

    }
}
