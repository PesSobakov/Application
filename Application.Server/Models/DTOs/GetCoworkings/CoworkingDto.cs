using Application.Server.Models.CoworkingDatabase;

namespace Application.Server.Models.DTOs.GetCoworkings
{
    public class CoworkingDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Address { get; set; } = null!;
        public List<RoomCountDto> RoomCountDtos { get; set; } = null!;
    }
}
