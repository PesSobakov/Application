using Application.Server.Models.CoworkingDatabase;

namespace Application.Server.Models.DTOs.GetCoworkings
{
    public class RoomCountDto
    {
        public WorkspaceType WorkspaceType { get; set; }
        public int Rooms { get; set; }
    }
}
