namespace _101clup.Api.Models
{
    public class MenuItem
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;
        public string Category { get; set; } = null!;
        public decimal Price { get; set; }

        public bool IsAvailable { get; set; } = true;

        public string? ImageUrl { get; set; }
        public string? Description { get; set; }
    }
}
