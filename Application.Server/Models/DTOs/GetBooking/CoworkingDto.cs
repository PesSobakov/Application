namespace Application.Server.Models.DTOs.GetBooking
{
    public class CoworkingDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Address { get; set; } = null!;
    }
}
