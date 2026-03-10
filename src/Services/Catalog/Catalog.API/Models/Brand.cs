namespace Catalog.API.Models;

public class Brand
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Logo { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string Country { get; set; } = default!;
    public string Website { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
