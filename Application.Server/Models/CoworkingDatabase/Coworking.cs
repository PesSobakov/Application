namespace Application.Server.Models.CoworkingDatabase
{
    public class Coworking
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Address { get; set; } = null!;
        public List<Workspace> Workspaces { get; set; } = null!;
    }
}
